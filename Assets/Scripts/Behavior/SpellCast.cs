using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellCast : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform spellingPartTransform; // 序列化字段，用于拖放 Weapon 物体

    
    
    void Start()
    {
        if (spellingPartTransform == null)
        {
            Debug.LogError("Weapon Transform 未指定，请在 Inspector 中将 Weapon 物体拖放到该字段中！");
        }

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
            animator.SetTrigger("Cast");

            // 检查是否成功获取了 Weapon 物体的引用
            if (spellingPartTransform != null)
            {
                ParticleEffectManager.Instance.PlayParticleEffect("Spell", spellingPartTransform.gameObject, Quaternion.identity,
                    Color.white, Color.white, 1f);
            }
            else
            {
                Debug.LogError("无法播放特效，因为 Weapon Transform 未指定！");
            }
        }
    }
}