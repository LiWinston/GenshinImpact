using System.Collections;
using AttributeRelatedScript;
using Behavior;
using Behavior.Skills;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Used to control the icon in the HUD for Player's Skills
    /// 分为两类，一类是随按键按下松开变化即可，一类带有延时，且停止时间需访问PlayerController、State、SpellCast、RemoteSpelling
    public class IconController: MonoBehaviour
    {
        Image image;

        enum IconType
        {
            Normal,
            Delayed
        }
        [SerializeField]IconType iconType = IconType.Normal;
        //Normal类Icon 明暗变化所根据的变量名所来自的玩家组件，如：component - playerController、state等
        [SerializeField]MonoBehaviour component;
        //Delayed类Icon 明暗变化所根据的变量，如：冰冻状态下的冰冻图标
        // [SerializeField]var variable;
        
        
        PlayerController playerController;
        void Awake()
        {
            image = transform.GetComponentInChildren<Image>();
            playerController = PlayerController.Instance;
        }

        private void Start()
        {
            if (image == null)
            {
                image = transform.GetComponentInChildren<Image>();
            }
            if (playerController == null)
            {
                playerController = PlayerController.Instance;
            }
        }

        private void Update()
        {
            
        }
    }
}