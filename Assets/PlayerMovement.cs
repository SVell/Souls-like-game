using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Move related section
    [Header("Move")]
    public CharacterController controller;
    public Transform cam;
    public float speed = 4;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool canMove = true;
    private bool moving = false;
    private bool isSprinting;
    public float sprintTime = 0;
    public float sprintOffset = 1;


    public Animator anim;
    
    // Jump
    [Header("Jump")]
    public float gravity = -9.81f;
    public float jumpForce = 3f;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask whatIsGrounded;
    private bool isGrounded;
    
    // Dash
    [Header("Dash")]
    private float dashTime;
    public float dashOffset;
    public float dashSpeed;
    private bool isDashing = false;
    
    // Attack 
    [Header("Attack")]
    List<String> attackAnims = new List<string>(new string[]{"Attack1","Attack2"});
    public int comboNum = 0;
    public float reset;
    private float resetTime = 1.2f;
    private float attackTime;
    public float attackOffset = 3f;
    public Transform attackPos;
    public LayerMask whatIsEnemy;

    [Header("Other")] 
    private PlayerStats playerStats;

    private void Start()
    {
        controller.detectCollisions = true;
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats.stamina <= 0)
        {
            sprintTime = sprintOffset;
        }
        else
        {
            sprintTime -= Time.deltaTime;
        }

        if (!isDashing && !isSprinting && playerStats.stamina > 40)
        {
            playerStats.stamina += 0.1f;
        }
        else
        {
            playerStats.stamina += 0.07f;
        }
        if (canMove)
        {
            Jump();
            Walk();
        }

        if (!moving)
        {
            Attack();
        }
        
        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
            if (!isDashing)
            {
                if (Input.GetKey(KeyCode.LeftShift) && playerStats.stamina > 0 && sprintTime < 0)
                {
                    speed = 8;
                    playerStats.stamina -= 0.1f;
                    anim.SetInteger("MoveStates",2);
                    isSprinting = true;
                }
                else
                {
                    isSprinting = false;
                    speed = 4;
                    anim.SetInteger("MoveStates",1);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.LeftAlt) && dashTime < 0 && playerStats.stamina > 40)
            {
                playerStats.stamina -= 40;
                isDashing = true;
                dashTime = dashOffset;
                speed = dashSpeed;
                anim.SetInteger("MoveStates",3);
            }
            
            dashTime -= Time.deltaTime;
            
            if (dashTime < dashOffset - 0.6f)
            {
                isDashing = false;
            }
            
            // Rotate when moving including camera rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            // Smooth rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0,angle,0);

            // Move player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (speed * Time.deltaTime));
            moving = true;
        }
        else
        {
            anim.SetInteger("MoveStates",0);
            moving = false;
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
            moving = true;
            anim.SetBool("Jump",true);
            velocity.y = (float)Math.Sqrt(jumpForce * -2f * gravity);
        }
    }
    

    void Attack()
    {
        if (Input.GetButtonDown("Fire1") && comboNum  < 2 && attackTime < 0 && playerStats.stamina > 20)
        {
            attackTime = attackOffset / 6;
            playerStats.stamina -= 40;
            StartCoroutine(DealDanage());
            anim.SetTrigger(attackAnims[comboNum]);
            comboNum++;
            reset = 0f;
            canMove = false;
        }

        reset += Time.deltaTime;
        
        if (reset > resetTime)
        {
            anim.SetTrigger("Reset");
            canMove = true;
            comboNum = 0;
        }

        if (comboNum == 2)
        {
            attackTime = attackOffset;
            resetTime = 3f;
            comboNum = 0;
            anim.SetTrigger("Reset");
        }
        else
        {
            resetTime = 1f;
        }

        attackTime -= Time.deltaTime;
    }

    IEnumerator DealDanage()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Attack");
    }
    
}
