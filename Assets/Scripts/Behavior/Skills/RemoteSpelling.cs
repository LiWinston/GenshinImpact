using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using IPoolable = Utility.IPoolable;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Behavior.Skills
{
    [System.Serializable]
    public class RemoteSpelling: MonoBehaviour
    {
        [Header("NameOfSkill - 技能名称")]
        public String Name = "Undefined";
    
        [InspectorLabel("生成--GenerationInfo")]
        public GameObject prefab;
        [SerializeField] private float maxExistTime = 10f;
        internal LayerMask castingLayer;
        [SerializeField] protected Vector3 generatingOffset = Vector3.up * 0.4f;
    

        [InspectorLabel("ObjectPool--对象池")]
        [SerializeField]private int defaultCapacity = 10;
        [SerializeField]private int maxCapacity = 10;
        protected ObjectPool<GameObject> _throwingsPool;
        public int countAll;
        public int countActive;
        public int countInactive;
    
        [InspectorLabel("Trying Cast Position -- 落点瞄准")]
        private GameObject SkillPreview;
        private LineRenderer lineRenderer;
        private bool isCasting;
        private Coroutine castingCoroutine;
        public float castingDistance = 7f; // 施法最大距离
        private Vector3 hitTarget;
        protected RemoteThrowingsBehavior throwingsBehavior;
        protected PlayerController _playerController;
        private SpellCast _spellCast;
        private bool canCast = false;

        [InspectorLabel("Skill Customization -- 技能自定义")]
        [SerializeField]
        protected string animatorTriggerName;
        [SerializeField] protected float animationGap = 0.4f;
        [SerializeField] private KeyCode key = KeyCode.F;
        [SerializeField] private Color validColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;

        [InspectorLabel("Throwing Customization -- 投掷自定义")]
        [SerializeField] public bool isUpdatedWithLevel = false;
    
        private void Start(){
            
            _spellCast = GetComponent<SpellCast>();
            _playerController = GetComponent<PlayerController>();
            _throwingsPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
                true, defaultCapacity, maxCapacity);
            castingLayer = LayerMask.GetMask("Wall", "Floor");
            throwingsBehavior = prefab.GetComponent<RemoteThrowingsBehavior>();

            if (throwingsBehavior.positionalCategory ==
                RemoteThrowingsBehavior.PositionalCategory.ImmediatelyInPosition)
            {
                string randomName;
                do
                {
                    randomName = UnityEngine.Random.Range(0, 100000).ToString();
                } while (GameObject.Find(randomName) != null);
                SkillPreview = new GameObject(randomName);
            }
            SkillPreview.transform.SetParent(this.transform);
            
            // 初始化 LineRenderer
            lineRenderer = SkillPreview.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
        }

        private GameObject CreateFunc(){
            // if(countAll < maxCapacity){
            //     GameObject throwing = Instantiate(prefab, transform.position, Quaternion.identity);
            //     actionOnGet(throwing);   //Try a new way of coding to solve the problem of non-destruction
            //     throwing.GetComponent<IPoolable>().SetPool(_throwingsPool);
            //     // throwing.GetComponent<RemoteThrowingsBehavior>().actionOnGet();
            //     return throwing.GameObject();
            // }
            // else
            // {
            //     _throwingsPool.Dispose();
            //     return _throwingsPool.Get();
            // }
            GameObject throwing = Instantiate(prefab, transform.position, Quaternion.identity);
            actionOnGet(throwing);
            throwing.GetComponent<IPoolable>().SetPool(_throwingsPool);
            return throwing.GameObject();
        }

        private void actionOnGet(GameObject obj){
            obj.GetComponent<IPoolable>().actionOnGet();
        }

        private void actionOnRelease(GameObject obj)
        {
            obj.GetComponent<IPoolable>().actionOnRelease();
            obj.SetActive(false);
        }

        void actionOnDestroy(GameObject obj)
        {
            // Destroy(obj);
        }
    
        private void Update()
        {
            countAll = _throwingsPool.CountAll;
            countActive = _throwingsPool.CountActive;
            countInactive = _throwingsPool.CountInactive;
            if (throwingsBehavior.positionalCategory == RemoteThrowingsBehavior.PositionalCategory.Throwing)
            {
                if (Input.GetKeyDown(key))
                {
                    if(isUpdatedWithLevel) StartCoroutine(Throw_LevelUpdated());
                    else
                    {
                        StartCoroutine(Throw());
                    }
                }
            }
            if (throwingsBehavior.positionalCategory == RemoteThrowingsBehavior.PositionalCategory.ImmediatelyInPosition)
            {
                if (Input.GetKeyDown(key))
                {
                    BeginAiming();
                }
                else if (Input.GetKeyUp(key))
                {
                    if (castingCoroutine != null)
                    {
                        StopCoroutine(castingCoroutine);
                    }
                    StartCoroutine(EndAimingAndCast());
                }
            }
        }

        private IEnumerator Throw_LevelUpdated(){
            _playerController.GetAnimator().SetTrigger(animatorTriggerName = "Throw");
            _playerController.isCrouching = false;
            _playerController.rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(animationGap);

            if (throwingsBehavior.startAudioClip != null)
            {
                SoundEffectManager.Instance.PlaySound(throwingsBehavior.startAudioClip, _playerController.swordObject);
            }
            
            int maxThrowingsCount = 30;
            int minThrowingsCount = 1;
            float maxAngle = 30f;
            float minAngle = 0f;

            int playerLevel = _playerController.state.GetCurrentLevel(); 
            int swordCount = Mathf.Clamp(playerLevel / 2, minThrowingsCount, maxThrowingsCount); // Calculate the number based on player level
            float angleIncrement = (maxAngle - minAngle) / (swordCount - 1); // Calculate angle increment

            if (swordCount == 1)
            {
                var th = _throwingsPool.Get();
                th.transform.position = _playerController.swordObject.transform.position;
                th.transform.forward = transform.forward;
                th.GetComponent<Rigidbody>().velocity = transform.forward * throwingsBehavior.throwingSpeed;
            }
            else
            {
                for (int i = 0; i < swordCount; i++)
                {
                    var throwStuff = _throwingsPool.Get();
                    throwStuff.transform.position = _playerController.swordObject.transform.position;

                    // 计算剑气的角度
                    float angle = minAngle + i * angleIncrement - (maxAngle - minAngle) / 2f;
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * _playerController.transform.forward;
                    throwStuff.transform.forward = direction;

                    Rigidbody rb = throwStuff.GetComponent<Rigidbody>();
                    rb.velocity = direction * throwingsBehavior.throwingSpeed;
                }
            }
        }

        protected IEnumerator Throw(){
            _playerController.GetAnimator().SetTrigger(animatorTriggerName = "Throw");
            _playerController.isCrouching = false;
            _playerController.rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(animationGap);
            var th = _throwingsPool.Get();
            if (throwingsBehavior.startAudioClip != null)
            {
                SoundEffectManager.Instance.PlaySound(throwingsBehavior.startAudioClip, _playerController.swordObject);
            }
            th.transform.position = _playerController.swordObject.transform.position;
            th.transform.forward = transform.forward;
            th.GetComponent<Rigidbody>().velocity = transform.forward * throwingsBehavior.throwingSpeed;
            
        }

        protected void BeginAiming()
        {
            if (isCasting) return;
            isCasting = true;
            castingCoroutine = StartCoroutine(ImmediateCastAimingLogic());
        }

        protected IEnumerator EndAimingAndCast()
        {
            SkillPreview.SetActive(false);
            if (!isCasting) yield break;
            if (canCast)
            {
                _playerController.GetAnimator().SetTrigger(animatorTriggerName);
                yield return new WaitForSeconds(animationGap);
                
                var throwStuff = _throwingsPool.Get();
                throwStuff.transform.position = hitTarget + generatingOffset;
                throwStuff.transform.rotation = transform.rotation;
                
                if (throwingsBehavior.startAudioClip != null)
                {
                    SoundEffectManager.Instance.PlaySound(throwingsBehavior.startAudioClip, throwStuff);
                }
            }
            isCasting = false;
        }

        protected IEnumerator ImmediateCastAimingLogic()
        {
            SkillPreview.SetActive(true);
            //--Its hard to make monster skill compatible with player skill here, give up.
            // var castTrans = _playerController
            //     ? _spellCast.spellingPartTransform.position
            //     : transform.position + Vector3.up * 0.5f;
            var castTrans = _playerController.mycamera.transform.position;
            while (isCasting)
            {
                if (Physics.Raycast(castTrans, PlayerController.Instance.mycamera.transform.forward,
                        out RaycastHit hit, castingDistance, castingLayer))
                {
                    hitTarget = hit.point;
                    SkillPreview.transform.position = hitTarget;
                    Vector3 playerToHitVector = castTrans - hit.point;
                    float triggerRange = throwingsBehavior.triggerRange;
                    DrawCircle(triggerRange, validColor, playerToHitVector.normalized * 0.08f, 30);
                    canCast = true;
                }
                else
                {
                    // out of spelling range, draw red circle
                    SkillPreview.transform.position = new Vector3(castTrans.x, 0.5f, castTrans.z);

                    // Ground height
                    float groundHeight = transform.position.y;

                    // Calculate the radius, considering the Pythagorean Theorem
                    float radius = Mathf.Sqrt(castingDistance * castingDistance - (castTrans.y - groundHeight) * (castTrans.y - groundHeight));

                    DrawCircle(radius, invalidColor, Vector3.zero, 100);
                    canCast = false;
                }
                yield return null;
                // yield return new WaitForSeconds(0.04f);
            }
        }
    
    
    
        private void DrawCircle(float radius, Color color, Vector3 offset, int segments = 100)
        {
            lineRenderer.startColor = color; // update color
            lineRenderer.endColor = color;

            lineRenderer.positionCount = segments + 1;
            lineRenderer.useWorldSpace = false;

            float deltaTheta = (2f * Mathf.PI) / segments;
            float theta = 0f;

            for (int i = 0; i < segments + 1; i++)
            {
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);

                Vector3 pos = new Vector3(x, 0, z) + offset; // Apply offset vector to avoid being buried underneath ground
                lineRenderer.SetPosition(i, pos);

                theta += deltaTheta;
            }
        }
    }
}