using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCast : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    void Start()
    {
        var childTransform = transform.Find("Model"); 
        if (childTransform != null)
        {
            animator = childTransform.GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("找不到Animator组件！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            animator.SetTrigger("Cast");
        }
    }
}
