using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    //Player rigidbody
    public Rigidbody2D rb;

    //Movement variables
    public float speed;
    private float Move;
    private bool facingRight = true;

    //Dynamic jump variables
    public float buttonTime;
    public float jumpHeight;
    float jumpTime;
    bool jumping;

    //Grounded variables
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool grounded;

    //Wall jump and slide variables
    private bool isSliding;
    private float slidingSpeed = 2f;
    public LayerMask wallLayer;
    public Transform wallCheck;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 8f);


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Flip()
    {
        //Flips player
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }


    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !grounded && Move != 0f)
        {
            isSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slidingSpeed, float.MaxValue));
        }
        else
        {
            isSliding = false;
        }
    }

    private void WallJump()
    {
        if (isSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                Flip();
            }

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks whether player is grounded
        grounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.0f, 0.03f), CapsuleDirection2D.Horizontal, 0, groundLayer);

        //Takes player input
        Move = Input.GetAxis("Horizontal");

        //Jump handling
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jumping = true;
            jumpTime = 0;
        }
        if (jumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            jumpTime += Time.deltaTime;
        }
        if (Input.GetButtonUp("Jump") | jumpTime > buttonTime)
        {
            jumping = false;
        }

        //Flips player sprite if movement direction changes
        if(!facingRight && Move > 0f && !isWallJumping)
        {
            Flip();
        }
        else if (facingRight && Move < 0f && !isWallJumping)
        {
            Flip();
        }

        WallSlide();
        WallJump();
    }

    private void FixedUpdate()
    {
        //Moves player
        if(!isSliding && !isWallJumping)
        {
            rb.velocity = new Vector2(speed * Move, rb.velocity.y);
        }
        
    }
}
