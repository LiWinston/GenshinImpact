using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Muryotaisu
{
    public class MuryotaisuController : MonoBehaviour
    {
        private Animator animator;

        public float speed = 2; // Walking speed
        public float jumpSpeed = 2; // Jump speed
        public float gravity = 1; //gravity

        public float rotas = 5; // Speed of rotation

        public float startKocchi = 2; // Distance to camera for Kocchiflag firing <●><●>

        float second; // Time Measurement

        int key = 0;
        string state;
        string prevState;

        private CharacterController controller;
        private Vector3 moveDirection = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {

            // Smile
            if (Input.GetKey("q"))
            {
                animator.SetBool("smileFlag", true);
            } else {
                animator.SetBool("smileFlag", false);
            }

            // Kocchiminna <●><●>
            Transform mypos = this.transform;
            Vector3 Apos = mypos.position; 

            Transform campos = Camera.main.transform;
            Vector3 Bpos = campos.position; 

            float dist = Vector3.Distance(Apos, Bpos);

            if (dist < startKocchi)
            {
                animator.SetBool("kocchiFlag", true);
            } else {
                animator.SetBool("kocchiFlag", false);
            }

            if (controller.isGrounded)
            {
                // Switching idle motions
                second += Time.deltaTime;

                if (Input.GetKeyDown("space"))
                {
                    animator.SetBool("jumpFlag", true);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", false);
                } else if ((Input.GetKey("up")) || (Input.GetKey("right")) || (Input.GetKey("down")) || (Input.GetKey("left"))|| Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s") || Input.GetKey("a"))
                {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", true);
                    animator.SetBool("idleFlag", false);
                } else if (second >= 15)
                {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", false);
                    animator.SetTrigger("idleBFlag");
                    second = 0;
                } else {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", true);
                }

                if (Input.GetKey("up") || Input.GetKey("w"))
                {
                    float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 180);
                    if (angleDiff == 0)
                    {
                        controller.Move (this.gameObject.transform.forward * speed * Time.deltaTime);
                    } else if (angleDiff < -1f)
                    {
                        transform.Rotate(0, rotas * -1, 0);
                    } else if (angleDiff > 1f)
                    {
                        transform.Rotate(0, rotas * 1, 0);
                    } else {
                        transform.rotation = Quaternion.Euler(0.0f, 180, 0.0f);
                    }
                }

                if (Input.GetKey("right") || Input.GetKey("d"))
                {
                    float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, -90);
                    if (angleDiff == 0)
                    {
                        controller.Move (this.gameObject.transform.forward * speed * Time.deltaTime);
                    } else if (angleDiff < -1f) 
                    {
                        transform.Rotate( 0,rotas * -1, 0);
                    } else if (angleDiff > 1f) 
                    {
                        transform.Rotate( 0,rotas * 1, 0);
                    } else 
                    {
                        transform.rotation = Quaternion.Euler(0.0f, -90, 0.0f);
                    }
                }

                if (Input.GetKey("down") || Input.GetKey("s")) 
                {
                    float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 0);
                    if (angleDiff == 0) 
                    {
                        controller.Move (this.gameObject.transform.forward * speed * Time.deltaTime);
                    } else if (angleDiff < -1f) 
                    {
                        transform.Rotate( 0,rotas * -1, 0);
                    } else if (angleDiff > 1f) 
                    {
                        transform.Rotate( 0,rotas * 1, 0);
                    } else 
                    {
                        transform.rotation = Quaternion.identity;
                    }
                }

                if (Input.GetKey("left") || Input.GetKey("a")) 
                {
                    float angleDiff = Mathf.DeltaAngle(transform.localEulerAngles.y, 90);
                    //Debug.Log($"left: {angleDiff}");
                    if (angleDiff == 0) 
                    {
                        controller.Move (this.gameObject.transform.forward * speed * Time.deltaTime);
                    } else if (angleDiff < -1f) 
                    {
                        transform.Rotate( 0,rotas * -1, 0);
                    } else if (angleDiff > 1f) 
                    {
                        transform.Rotate( 0,rotas * 1, 0);
                    } else 
                    {
                        transform.rotation = Quaternion.Euler(0.0f, 90, 0.0f);
                    }
                }
    
                if (Input.GetKeyDown("space"))
                {
                    moveDirection.y = jumpSpeed;
                }

            }

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

        }
    }

}