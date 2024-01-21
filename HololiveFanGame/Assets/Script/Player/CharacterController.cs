using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [Header("Player Movement")]
    private float horizontal;
    private float moveSpeed = 8f;
    private bool isFacingRight = true;

    [Header("Jumping")]
    private bool canDoubleJump;
    private float jumpPower = 20f;

    [Header("Dashing")]
    private float dashPower = 60f;
    private float dashCD = 0.5f;
    private bool canDash;
    private bool isDashing;
    private float dashTime = 0.1f;
    
    

    private Rigidbody2D rb;
    private Transform groundCheck;
    private TrailRenderer tr;
    [SerializeField] private LayerMask groundLayer;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        groundCheck = transform.GetChild(0).transform;
        canDash = true;
        canDoubleJump = true;
    }
    void Update()
    {
        if(isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        Flip();

        if(isGrounded() && !canDoubleJump){
            canDoubleJump = true;  
        } 

        if(Input.GetButtonDown("Jump") && isGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }else if (Input.GetButtonDown("Jump") && !isGrounded() && canDoubleJump){
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            canDoubleJump = false;
        }

        if (Input.GetButtonDown("Dash") && canDash){
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate() {
        if(isDashing) return;
        
        // movement
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

    }

    // Custom  functions
    private bool isGrounded(){
        bool tempGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return tempGrounded;
    }
    
    // shows which direction the player is facing
    private void Flip(){
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f){
            isFacingRight =  !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    
    private IEnumerator Dash(){
        canDash = false;
        isDashing = true;

        // temp change gravity
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // dashing towards direction faced
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
    
}