using System.Collections;
using System.Linq;
using AttributeRelatedScript;
using Behavior;
using Behavior.Skills;
using ItemSystem.Generate;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public Text timerText;
        private float startTime;
        private bool gameEnded;
        public Transform bossRoom;
        public Transform lookat;
        private bool isFinalBattle = false;
        public float ElapsedTime;
        public float RemainingTime;
        public AudioSource BGM;

        public GameObject stuffGenerator;

        private void Start()
        {
            gameEnded = false;
            ElapsedTime = 0f;
            // 获取当前场景加载的时间
            startTime = Time.timeSinceLevelLoad;
            if(!lookat) lookat = GameObject.Find("SM_Prop_Table_04").transform;
            if (Random.Range(0f, 1f) < 0.5f)
            {
                BGM.clip = Resources.Load<AudioClip>("Music/幻境_战斗");
            }
            else
            {
                BGM.clip = Resources.Load<AudioClip>("Music/史诗");
            }
            BGM.Play();

            if(!stuffGenerator) stuffGenerator = GameObject.Find("Stuff生成");
        }

        private void Update()
        {
            if (!gameEnded)
            {
                // 计算从场景加载开始经过的时间
                ElapsedTime = Time.timeSinceLevelLoad - startTime;

                // 计算剩余时间
                RemainingTime = 320 - ElapsedTime;

                // 更新倒计时文本
                UpdateTimerText(RemainingTime);

                if (ElapsedTime >= 240 || Input.GetKey("`")) // 240秒 = 4分钟
                {
                    if (!isFinalBattle)
                    {
                        //Stop Generating normal mst
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "MST")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "蓝包_EnergySupplyItem")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "血包_HealthSupplyItem")!.maxCapacity = 0;
                        stuffGenerator.GetComponents<Component>().OfType<PrefabGenerator>().FirstOrDefault(pg => pg.prefab.name == "DamageIncreaseItem")!.maxCapacity = 0;

                        PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "牧野流星")!.isCosumingEnegyProportionally = false;
                        // PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "魂牵梦萦")!.isAmountUpdatedWithLevel = true;
                        // PlayerController.Instance.GetComponents<Component>().OfType<RemoteSpelling>().FirstOrDefault(rs => rs.Name == "魂牵梦萦")!.maxAngle_SingleSide = 10f;
                        
                        var bouncePersistentReverie = PlayerController.Instance.GetComponents<Component>()
                            .OfType<RemoteSpelling>()
                            .FirstOrDefault(rs => rs.Name == "魂牵梦萦");

                        if (bouncePersistentReverie != null)
                        {
                            bouncePersistentReverie.isAmountUpdatedWithLevel = true;
                            bouncePersistentReverie.maxAngle_SingleSide = 5f;
                        }

                        
                        PlayerController.Instance.GetComponent<SpellCast>().JZZCostRate = 0.12f;
                        
                        GameObject boosPrfb = Resources.Load<GameObject>("Prefab/BossSpawner");
                        if (boosPrfb == null) Debug.LogError("NO BossGenerator");
                        var boosPrfbi = Instantiate(boosPrfb, lookat.position + Vector3.up, Quaternion.identity);
                        
                        // 触发决战事件，将玩家传送至指定位置
                        PlayerController.Instance.ShowPlayerHUD("Final battle is coming!");
                        StartCoroutine(TeleportPlayerToFloorLarge());
                        PlayerController.Instance.state.CurrentHealth = PlayerController.Instance.state.maxHealth;
                        PlayerController.Instance.state.CurrentEnergy = PlayerController.Instance.state.maxEnergy;
                        PlayerController.Instance.state.CurrentPower = PlayerController.Instance.state.maxPower;
                        
                    }
                    
                    if (ElapsedTime >= 320) // 300秒 = 5分钟
                    {
                        ElapsedTime = -1f;
                        // 游戏胜利，加载WinScene场景
                        LoadWinScene();
                    }
                }
            }
        }

        private void UpdateTimerText(float remainingTime)
        {
            int minutes = Mathf.FloorToInt((remainingTime - 60) / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            if (remainingTime >= 80)
            {
                timerText.text = "Countdown to the decisive battle: " + minutes.ToString("00") + " Min " + seconds.ToString("00") + " s";
            }
            else
            {
                timerText.text = "Hold On Final: " + seconds.ToString("00") + " s";
            }
        }

        private IEnumerator TeleportPlayerToFloorLarge()
        {
            isFinalBattle = true;
            
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
            BGM.clip = Resources.Load<AudioClip>("Music/沙场");
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
            // 游戏胜利，加载WinScene场景
            SceneManager.LoadScene("WinScene");
            gameEnded = true;
        }
    }
}
