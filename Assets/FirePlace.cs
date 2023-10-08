using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior.Effect;

public class FirePlace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name); // 输出进入触发器的物体名称
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Damage applied to: " + other.gameObject.name); // 输出应用伤害的物体名称
            StartCoroutine(ContinuousDamage.MakeContinuousDamage(other.gameObject, 200));
        }
    }

}
