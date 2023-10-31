using System.Collections;
using System.Linq;
using AttributeRelatedScript;
using Behavior;
using Behavior.Skills;
using ItemSystem.Generate;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class GameSceneManager : MonoBehaviour
    {
        public Text timerText;
        private float startTime;
        private bool gameEnded;
        public Transform bossRoom;
        public Transform lookat;
        private bool isFinalBattle = false;
        [FormerlySerializedAs("ElapsedTime")] public float _realElapsedTime;
        public float RemainingTime;
        public AudioSource BGM;

        public GameObject stuffGenerator;
        [SerializeField] private float totalGameSeconds = 310;
        [SerializeField] private float finalBattleSeconds = 75;
        private float finalBattleStartTimeStamp = 0f;

        private static GameSceneManager instance;
        private string BGMSeries = "Horror";
        private bool hasActiveKeyBindings;
        
        public static GameSceneManager Instance
        {
            get
            {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSceneManager>();
                
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("GameManager");
                    instance = gameObject.AddComponent<GameSceneManager>();
                }
            }

            return instance;
            }
        }

        public bool IsFinalBattle
        {
            get => isFinalBattle;
            set { isFinalBattle = value; }
        }

        private void Start()
        {
            gameEnded = false;
            _realElapsedTime = 0f;
            // 获取当前场景加载的时间
            startTime = Time.timeSinceLevelLoad;
            if(!lookat) lookat = GameObject.Find("SM_Prop_Table_04").transform;
            var random = Random.Range(0f, 1f);
            if (random < 0.33f)
            {
                BGM.clip = Resources.Load<AudioClip>("Music/haunted_house");
                BGMSeries = "Horror";
            }
            else if (random < 0.66f)
            {
                BGM.clip = Resources.Load<AudioClip>("Music/史诗");
                BGMSeries = "Epic";
            }
            else if (random < 1f)
            {
                BGM.clip = Resources.Load<AudioClip>("Music/幻境");
                BGMSeries = "Fantasy";
            }
            BGM.Play();

            if(!stuffGenerator) stuffGenerator = GameObject.Find("Stuff生成");
            hasActiveKeyBindings = false;
        }
        

        
        private void Update()
        {
            if (!hasActiveKeyBindings)
            {
                UIManager.Instance.ShowMessage2("Press TAB to Hide/Show Key Bindings!");
                IconManager.ShowKeyBinding();
                hasActiveKeyBindings = true;
            }
            if (!gameEnded)
            {
                // update remaining time
                if (!IsFinalBattle)
                {
                    // calculate elasped time since level is loaded
                    _realElapsedTime = Time.timeSinceLevelLoad - startTime;
                    RemainingTime = totalGameSeconds - _realElapsedTime;
                    UpdateTimerText(RemainingTime - finalBattleSeconds);
                }
                else
                {
                    RemainingTime = finalBattleSeconds - (Time.timeSinceLevelLoad - finalBattleStartTimeStamp);
                    UpdateTimerText(RemainingTime);
                }
                if (RemainingTime <= 0f)
                {
                    // _realElapsedTime = -1f;
                    // 游戏胜利，加载WinScene场景
                    LoadWinScene();
                }

                if (_realElapsedTime >= totalGameSeconds - finalBattleSeconds || Input.GetKey("`"))
                {
                    if (!IsFinalBattle)
                    {
                        //Stop Generating normal mst
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "MST")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "蓝包_EnergySupplyItem")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "血包_HealthSupplyItem")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "DamageIncreaseItem")!.maxCapacity = 0;

                        PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "MeadowMeteor")!.isCosumingEnegyProportionally = false;
                        // PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "魂牵梦萦")!.isAmountUpdatedWithLevel = true;
                        // PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "魂牵梦萦")!.maxAngle_SingleSide = 10f;
                        
                        var bouncePersistentReverie = PlayerController.Instance.GetComponents<Component>()
                            .OfType<RemoteSpelling>()
                            .FirstOrDefault(rs => rs.Name == "PersistentReverie");

                        if (bouncePersistentReverie != null)
                        {
                            bouncePersistentReverie.isAmountUpdatedWithLevel = true;
                            bouncePersistentReverie.maxAngle_SingleSide = 5f;
                            bouncePersistentReverie.isCosumingEnegyProportionally = false;
                            bouncePersistentReverie.prefab.GetComponent<RemoteThrowingsBehavior>()._energyCost = 800f;
                        }

                        
                        PlayerController.Instance.GetComponent<SpellCast>().JZZCostRate = 0.12f;
                        
                        GameObject boosPrfb = Resources.Load<GameObject>("Prefab/BossSpawner");
                        if (boosPrfb == null) Debug.LogError("NO BossGenerator");
                        var boosPrfbi = Instantiate(boosPrfb, lookat.position + Vector3.up, Quaternion.identity);
                        
                        // End of Game
                        PlayerController.Instance.ShowPlayerHUD("The Elder is awakening!!");
                        StartCoroutine(TeleportPlayerToFloorLarge());
                        PlayerController.Instance.state.CurrentHealth = PlayerController.Instance.state.maxHealth;
                        PlayerController.Instance.state.CurrentEnergy = PlayerController.Instance.state.maxEnergy;
                        PlayerController.Instance.state.CurrentPower = PlayerController.Instance.state.maxPower;
                        
                    }
                }
            }
        }

        private void UpdateTimerText(float remainingTime)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            string timeStr = minutes.ToString("00") + " Min " + seconds.ToString("00") + " s";
            // if (remainingTime >= finalBattleSeconds)
            if(!IsFinalBattle)
            {
                timerText.text = "Survive!! " + timeStr + " to Elder awakes";
            }
            else
            {
                timerText.text = "Survive for " + timeStr + "OR Kill the Elder!";
            }
        }

        private IEnumerator TeleportPlayerToFloorLarge()
        {
            IsFinalBattle = true;
            // finalBattleTimer = finalBattleSeconds;
            finalBattleStartTimeStamp = Time.timeSinceLevelLoad;
                
            ParticleSystem transfer = Resources.Load<ParticleSystem>("Liberate_04.1_Darkness");
            if (transfer == null) Debug.LogError("NO transfer");
            var transferi = new ParticleSystem[8];
            PlayerController playerController = PlayerController.Instance;
            for (int x = 0; x < 8; ++x)
            {
                transferi[x] = Instantiate(transfer, playerController.transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(2f);
            for (int x = 0; x < 8; ++x)
            {
                transferi[x].Stop();
                Destroy(transferi[x].gameObject);
            }
            playerController.transform.position = bossRoom.position + Vector3.up * 2f;
            
            // PlayerController.Instance.transform.forward =lookat.position - PlayerController.Instance.transform.position;
            Vector3 targetDirection = lookat.transform.position - playerController.transform.position;
            targetDirection.y = 0f; // 将Y轴分量置零，以确保只在水平面上旋转
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            // 旋转到目标方向
            playerController.transform.rotation = targetRotation;
            
            if(!BGM) BGM = GameObject.Find("BGM").GetComponent<AudioSource>();
            BGM.clip = BGMSeries switch
                {
                    "Horror" => Resources.Load<AudioClip>("Music/final_boss"),
                    "Epic" => Resources.Load<AudioClip>("Music/史诗_战斗"),
                    "Fantasy" => Resources.Load<AudioClip>("Music/幻境_战斗"),
                    _ => BGM.clip
                };
            // BGM.clip = Resources.Load<AudioClip>("Music/final_boss");
            BGM.Play();
            UIManager.Instance.UIMessage_2MSG.Clear();
            UIManager.Instance.ShowMessage2("Meadow Meteor Energy Cost Reduced");
            yield return new WaitForSeconds(1.2f);
            UIManager.Instance.ShowMessage2("golden bell Energy Cost Reduced");
            yield return new WaitForSeconds(1.1f);
            UIManager.Instance.ShowMessage2("Persistent Reverie Amount Increased");
            yield return new WaitForSeconds(1.05f);
            PlayerController.Instance.ShowPlayerHUD("Survive in the Boss Battle!");
        }

        private void LoadWinScene()
        {
            // loading win scene if boss is defeated
            SceneManager.LoadScene("WinScene");
            gameEnded = true;
        }
    }
}
