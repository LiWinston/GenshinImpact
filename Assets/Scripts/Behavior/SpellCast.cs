using System.Collections;
using System.Collections.Generic;
using AttributeRelatedScript;
using Behavior.Effect;
using Behavior.Health;
using UnityEngine;
using Utility;

namespace Behavior
{
    public class SpellCast : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] internal Transform spellingPartTransform; // 施法的手
        [SerializeField] internal Transform innerSpellingTransform; // 施法的腰子
        [SerializeField] private float spellRange = 1.6f;
        private State state;
        private EffectTimeManager _effectTimeManager;


        private void Awake(){
            _effectTimeManager = GetComponent<EffectTimeManager>();
        }

        void Start()
        {
            state = GetComponent<State>();
            if (spellingPartTransform == null)
            {
                Debug.LogError("Weapon Transform 未指定，请在 Inspector 中将 Weapon 物体拖放到该字段中！");
            }

            if (innerSpellingTransform == null) innerSpellingTransform = Find.FindDeepChild(transform, "spine_03");
            var childTransform = transform.Find("Model");
            if (childTransform != null)
            {
                animator = childTransform.GetComponent<Animator>();
            }

            if (animator == null)
            {
                Debug.LogError("找不到 Animator 组件！");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                PlayerController.Instance.isCrouching = false;
                animator.SetTrigger("Cast");
                CastSpell();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayerController.Instance.isCrouching = false;
                animator.SetTrigger("ULT");
                CastUlt();
            }
        
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(state.isJZZ) return;
                PlayerController.Instance.isCrouching = false;
                animator.SetTrigger("Cast");
                StartJZZ();
            }
        }

        private void StartJZZ()
        {
            if (!state.ConsumeEnergy(state.maxEnergy * 0.2f)) return;
            SoundEffectManager.Instance.PlaySound(new List<string>(){"Music/音效/法术/JZZ1","Music/音效/法术/JZZ2"}, gameObject);
            _effectTimeManager.CreateEffectBar("JZZ", Color.cyan, 7f);
            // GameObject.Find("Canvas").GetComponent<EffectTimeManager>().CreateEffectBar("JZZ", Color.cyan, 7f);
            state.isJZZ = true;
            var d = 7f;
            ParticleSystem JZZ = Resources.Load<ParticleSystem>("JZZ");
            if(JZZ == null) Debug.LogError("NO JZZ");
            var jzzi = Instantiate(JZZ, innerSpellingTransform);
            jzzi.Play();
            Coroutine c = StartCoroutine(StopJZZAfterDuration(d));
            StartCoroutine(ObservePower(d, c, jzzi));
        }

        //JZZ observer, once power too low exit JZZ mode
        private IEnumerator ObservePower(float f, Coroutine c, ParticleSystem pts)
        {
            float timer = 0.0f;
    
            while (state.isJZZ && timer < f)
            {
                timer += Time.deltaTime;

                for (float waitTime = 0; waitTime < 0.5f; waitTime += Time.deltaTime)
                {
                    yield return null;
                }

                if (state.CurrentPower < 10)
                {
                    state.isJZZ = false;
                    StopCoroutine(c);
                    _effectTimeManager.StopEffect("JZZ");
                    if (pts != null)
                    {
                        Destroy(pts.gameObject);
                    }
                }

                yield return null;
            }
            _effectTimeManager.StopEffect("JZZ");
            // GameObject.Find("Canvas").GetComponent<EffectTimeManager>().StopEffect("JZZ");
            Destroy(pts.gameObject);
            yield return null;
        }


        private IEnumerator StopJZZAfterDuration(float t)
        {
            // ParticleEffectManager.Instance.PlayParticleEffect("JZZ", innerSpellingTransform.gameObject, Quaternion.identity, Color.clear, Color.clear,t);
            yield return new WaitForSeconds(t);
            // 停止金钟罩效果
            state.isJZZ = false;
            _effectTimeManager.StopEffect("JZZ");
            // GameObject.Find("Canvas").GetComponent<EffectTimeManager>().StopEffect("JZZ");
        }

        private void CastSpell()
        {
            //TODO:更新此机制。
            if (!state.ConsumeEnergy(state.CurrentDamage))
            {
                return;
            };
            // 检查是否成功获取了 Weapon 物体的引用
            if (spellingPartTransform != null)
            {
                ParticleEffectManager.Instance.PlayParticleEffect("Spell", spellingPartTransform.gameObject,
                    Quaternion.identity,
                    Color.white, Color.white, 1f);
            }
            else
            {
                Debug.LogError("无法播放特效，因为 Weapon Transform 未指定！");
            }
            
            SoundEffectManager.Instance.PlaySound("Music/音效/法术/极寒", spellingPartTransform.gameObject);
            // 检测在法术范围内的敌人 TODO:??? Layer就不行==要GetMask
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, spellRange, LayerMask.GetMask("Enemy"));
            // Debug.LogWarning("检测到 "+hitEnemies.Length + "Enemy");
            // Collider[] hitEnemies = Physics.OverlapSphere(playerPosition, spellRange);
            foreach (Collider enemy in hitEnemies)
            {
                // 检查是否敌人
                if (enemy.CompareTag("Enemy"))
                {
                    // 获取敌人的 HealthSystem 组件
                    HealthSystem enemyHealth = enemy.GetComponent<HealthSystemComponent>().GetHealthSystem();

                    if (enemyHealth != null)
                    {
                        // 对敌人造成伤害
                        float damageAmount = state.CurrentDamage;
                        enemyHealth.Damage(damageAmount);
                    
                        // 计算持续掉血的总量（20％的伤害）
                    
                        float freezeRemainingTime = 3f + state.GetCurrentLevel() * 0.2f / 10f;
                        float continuousDamageAmount = damageAmount * 0.2f;
                        enemy.GetComponent<MonsterBehaviour>().ActivateFreezeMode(freezeRemainingTime, continuousDamageAmount);
                    
                        // 播放特效
                        if (spellingPartTransform != null)
                        {
                            Transform spineTransform = Find.FindDeepChild(enemy.transform, "spine_01");
                            ParticleEffectManager.Instance.PlayParticleEffect("HitBySpell", (spineTransform != null ? spineTransform : enemy.transform).gameObject, 
                                Quaternion.identity, Color.red, Color.black, freezeRemainingTime);
                        }
                    }
                }
            }
        }

        // 协程来实现持续掉血

    
        private void CastUlt()
        {
            if (!state.ConsumeEnergy(state.CurrentDamage * 1.5f))
            {
                return;
            };
            
            SoundEffectManager.Instance.PlaySound("Music/音效/法术/ULT", spellingPartTransform.gameObject);
            
            // 检查是否成功获取了 Weapon 物体的引用
            if (spellingPartTransform != null)
            {
                ParticleEffectManager.Instance.PlayParticleEffect("ULT", spellingPartTransform.gameObject,
                    Quaternion.identity,
                    Color.white, Color.white, 1.2f);
            }
            ParticleEffectManager.Instance.PlayParticleEffect("ULT1", innerSpellingTransform.gameObject,
                Quaternion.identity,
                Color.white, Color.white, 3f);
            // 获取玩家的位置
            Vector3 playerPosition = transform.position;

            // 检测在法术范围内的敌人
            Collider[] hitEnemies = Physics.OverlapSphere(playerPosition, spellRange);
            List<Collider> enemies = new List<Collider>();
            foreach (Collider enemy in hitEnemies)
            {
                // 检查是否敌人
                if (enemy.CompareTag("Enemy"))
                {
                    enemies.Add(enemy);
                    // 获取敌人的 HealthSystem 组件
                    HealthSystem enemyHealth = enemy.GetComponent<HealthSystemComponent>().GetHealthSystem();
                    if (enemyHealth != null)
                    {
                        // 对敌人造成伤害
                        enemyHealth.Damage(state.CurrentDamage * 0.5f);
                        // 播放特效
                        if (spellingPartTransform != null)
                        {
                            Transform spineTransform = Find.FindDeepChild(enemy.transform, "spine_01");
                            ParticleEffectManager.Instance.PlayParticleEffect("HitByUlt", (spineTransform != null ? spineTransform : enemy.transform).gameObject, 
                                Quaternion.identity, Color.white, Color.blue, 1.8f);
                        }
                    }
                }
            }
            StartCoroutine(MindControl(enemies));
        }

        IEnumerator MindControl(List<Collider> enemies)
        {
            yield return new WaitForSeconds(1.4f);
            foreach (var e in enemies)
            {
                if (state.ConsumeEnergy(0.02f*state.CurrentEnergy))
                {
                    e.GetComponent<MonsterBehaviour>().ActivateSelfKillMode(10);
                }
            }
        }
    }
}