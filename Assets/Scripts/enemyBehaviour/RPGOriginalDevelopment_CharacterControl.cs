using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum CameraType
{
    Orbit,
    OrbitLock,
    Fixed,
    FixedLock,
}

[System.Serializable]


public class RPGOriginalDevelopment_CharacterControl : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    
    public bool isGrounded = true;
    public float airTime = 0;
    
   
    
    void Start()
    {
        previousFramePosition = transform.position;
    }
    
    
    
    public Animator animatorController;
    public void AnimationControl()
    {
        animatorController.SetFloat("Sideways", Vector3.Dot(transform.right, thisFramePositionOffset) * (1 / Time.deltaTime), 0.2f, Time.deltaTime);
        animatorController.SetFloat("ForwardBackward", Vector3.Dot(transform.forward, thisFramePositionOffset) * (1 / Time.deltaTime), 0.2f, Time.deltaTime);
        
        if (isGrounded)
        {
            airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;
        }
        
        animatorController.SetFloat("AirTime", airTime);
        animatorController.SetBool("IsGrounded", isGrounded);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
    
    public void TakeDamage(float damageValue)
    {
        print("Taking damage: " + damageValue);
    }
    
    public Vector3 previousFramePosition;
    public Vector3 thisFramePositionOffset;
    
    void Update()
    {
        thisFramePositionOffset = transform.position - previousFramePosition;
        // AnimationControl();
        previousFramePosition = transform.position;
    }
}
