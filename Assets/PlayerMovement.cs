using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Move related section
    public CharacterController controller;
    public Transform cam;
    public float speed = 4;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 velocity;

    public Animator anim;
    
    // Jump
    public float gravity = -9.81f;
    public float jumpForce = 3f;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask whatIsGrounded;
    private bool isGrounded;

    private void Start()
    {
        controller.detectCollisions = true;
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Walk();
    }

    private void Walk()
    {
        
        
        // Take player Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal,0f,vertical).normalized;

        // If moving
        if (direction.magnitude >= 0.1)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 8;
                anim.SetInteger("MoveStates",2);
            }
            else
            {
                speed = 4;
                anim.SetInteger("MoveStates",1);
            }
            
            // Rotate when moving including camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            // Smooth rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0,angle,0);

            // Move player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (speed * Time.deltaTime));
        }
        else
        {
            anim.SetInteger("MoveStates",0);
        }
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            anim.SetBool("Jump",false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            anim.SetBool("Jump",true);
            velocity.y = (float)Math.Sqrt(jumpForce * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
}
