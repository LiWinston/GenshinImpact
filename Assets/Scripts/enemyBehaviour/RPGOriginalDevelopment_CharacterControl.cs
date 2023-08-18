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
public class CameraPositionData
{
    public string name;
    public CameraType type;
    public List<Transform> cameraPositions;
    public float thirdPersonCameraScrollSpeed = 0.1f;
    [HideInInspector]
    public float positionInterpolation;
    
    public Vector3 RefreshCurrentPosition(float changeValue, Camera camera, Transform cameraParent)
    {
        switch (type)
        {
            case CameraType.Orbit:
                return RefreshOrbitPosition(changeValue, camera);
            case CameraType.OrbitLock:
                cameraParent.localEulerAngles = Vector3.zero;
                return RefreshOrbitPosition(changeValue, camera);
            case CameraType.Fixed:
                camera.transform.rotation = cameraPositions[0].rotation;
                return cameraPositions[0].position;
            case CameraType.FixedLock:
                cameraParent.localEulerAngles = Vector3.zero;
                camera.transform.rotation = cameraPositions[0].rotation;
                return cameraPositions[0].position;
        }
        return Vector3.zero;
    }
    
    public Vector3 RefreshOrbitPosition(float changeValue, Camera camera)
    {
        float distanceTemp = 0;
        positionInterpolation += (thirdPersonCameraScrollSpeed * changeValue);
        positionInterpolation = Mathf.Max(0, positionInterpolation);
        
        for (int i = 1; i < cameraPositions.Count; i++)
        {
            float currentDistance = Vector3.Distance(cameraPositions[i].position, cameraPositions[i - 1].position);
            distanceTemp += currentDistance;
            
            if (positionInterpolation <= distanceTemp)
            {
                camera.transform.rotation = Quaternion.Lerp(cameraPositions[i - 1].rotation, cameraPositions[i].rotation, (positionInterpolation - (distanceTemp - currentDistance)) / currentDistance);
                return Vector3.Lerp(cameraPositions[i - 1].position, cameraPositions[i].position, (positionInterpolation - (distanceTemp - currentDistance)) / currentDistance);
            }
        }
        
        positionInterpolation = distanceTemp;
        return cameraPositions[cameraPositions.Count - 1].position;
    }
}

public class RPGOriginalDevelopment_CharacterControl : MonoBehaviour
{
    [FormerlySerializedAs("camera")] public Camera Chara_camera;
    public Rigidbody playerRigidbody;
    public Transform cameraParent;
    
    public CameraPositionData[] cameraPositions;
    public int currentCameraPosition = 0;
    public float forwardForce = 100;
    public float jumpForce = 800;
    public float mouseRotationSpeed = 2;
    
    public float cameraRollAngle = 0;
    public float minCameraRollAngleLimit = -100, maxCameraRollAngleLimit = 100;
    
    public bool isGrounded = true;
    public float airTime = 0;
    
    public float moveForceTimer = 0.05f;
    public float moveForceTimerCounter = 0.05f;
    
    void Start()
    {
        previousFramePosition = transform.position;
    }
    
    public void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            currentCameraPosition++;
            if (currentCameraPosition >= cameraPositions.Length)
            {
                currentCameraPosition = 0;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            currentCameraPosition--;
            if (currentCameraPosition < 0)
            {
                currentCameraPosition = cameraPositions.Length - 1;
            }
        }
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * mouseRotationSpeed));
            
            cameraRollAngle += Input.GetAxis("Mouse Y") * mouseRotationSpeed;
            
            if (cameraRollAngle >= minCameraRollAngleLimit && cameraRollAngle <= maxCameraRollAngleLimit)
            {
                cameraParent.Rotate(Vector3.left * (Input.GetAxis("Mouse Y") * mouseRotationSpeed));
            }
            else
            {
                cameraRollAngle = Mathf.Clamp(cameraRollAngle, minCameraRollAngleLimit, maxCameraRollAngleLimit);
            }
        }
        
        Chara_camera.transform.position = Vector3.Lerp(Chara_camera.transform.position, 
                                                        cameraPositions[currentCameraPosition].RefreshCurrentPosition(
                                                            Input.mouseScrollDelta.y, Chara_camera, cameraParent), 
                                                        0.5f);
        
        moveForceTimerCounter -= Time.deltaTime;
        
        if (moveForceTimerCounter <= 0)
        {
            moveForceTimerCounter += moveForceTimer;
            
            if (Input.GetKey(KeyCode.W))
            {
                playerRigidbody.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                playerRigidbody.AddForce(transform.forward * (-forwardForce * 0.7f), ForceMode.Impulse);
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                playerRigidbody.AddForce(transform.right * -forwardForce, ForceMode.Impulse);
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                playerRigidbody.AddForce(transform.right * forwardForce, ForceMode.Impulse);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                playerRigidbody.AddForce(transform.up * jumpForce);
                isGrounded = false;
            }
        }
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
        UserInput();
        AnimationControl();
        previousFramePosition = transform.position;
    }
}
