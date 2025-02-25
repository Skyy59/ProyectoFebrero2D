using System.Collections;
using Unity.Hierarchy;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerContr : MonoBehaviour
{
    #region VARIABLES
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] TrailRenderer tr;
    [SerializeField] Animator cAnimator;

    //movement
    private float horz;
    private bool facingRight;

    [Header("Jump")]
    public float speed = 8f;
    public float jumpingPower = 16f;

    

    [Header("Dash")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    private bool dash = true;
    private bool isDashing;


    [Header("WallSlide & WallJump")]
    [SerializeField] private float wallJumpingTime = 0.2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Space(5)]
    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    [Header("CoyoteTime")]
    public float coyoteTime = 0.2f;
    private float cTimeCoutner;
    [Header("JumpBuffer")]
    public float jumpBufferTime = 0.2f;
    private float jBufferCounter;
    

    [Header("Checks")]
    [SerializeField] private Transform GCheck;
    [SerializeField] private Vector2 GCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private LayerMask Ground;
    [SerializeField] private Transform WCheck;
    [SerializeField] private Vector2 WCheckSize = new Vector2(0.08f, 0.7f);
    [SerializeField] private LayerMask Walls;

    #endregion
    void Update()
    {
        #region INPUTS
        horz = Input.GetAxisRaw("Horizontal");
        #endregion

        #region DASH
        if (isDashing)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash)
        {
            StartCoroutine(Dash());
        }
        #endregion

        #region METHODS

        JumpBuffer();
        CoyoteTime();
        WallJump();
        WallSlide();
        Jump();
        Animations();

        if(!isWallJumping)
        {
            Flip();
        }

        #endregion

        #region CHECKS
        if (IsGrounded())
        {
            cAnimator.SetBool("Jump", false);
            cAnimator.SetBool("Fall", false);
        }
        else
        {
            cAnimator.SetBool("Jump", true);
        }

        if (!IsGrounded() && rb.linearVelocity.y < 0)
        {
            cAnimator.SetBool("Fall", true);
            cAnimator.SetBool("Jump", false);
        }


        if (IsWalled() == true)
        {
            cAnimator.SetBool("WallStick", true );
            cAnimator.SetBool("Jump", false);
        }
        else
        {
            cAnimator.SetBool("WallStick", false);
        }

        if (isWallJumping == true)
        {
            cAnimator.SetBool("isWallJumping", true);
            cAnimator.SetBool("Jump", false);
            cAnimator.SetBool("Fall", false);
        }
        else
        {
            cAnimator.SetBool("isWallJumping", false);
        }
        #endregion 
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (!isWallJumping)
            rb.linearVelocity = new Vector2(horz * speed, rb.linearVelocity.y);
    }

    
    #region MECHANICS 
    void Jump()
    {


            if (jBufferCounter > 0f && cTimeCoutner > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jBufferCounter = 0f;
                

             
            }


            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

                cTimeCoutner = 0f;
               
            }



       

    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpingTime;
            

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpCounter = 0f;
            

            if (transform.localScale.x != wallJumpingDirection)
            {
                facingRight = !facingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

    }

    private void StopWallJumping()
    {
        isWallJumping = false;
        
    }
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horz != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
     
        }
        else
        {
            isWallSliding = false;
            
        }
    }

    void CoyoteTime()
    {
        if (IsGrounded())
        {
            cTimeCoutner = coyoteTime;
            
        }
        else
        {
            cTimeCoutner -= Time.deltaTime;
        }
    }

    void JumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jBufferCounter = jumpBufferTime;
            
        }
        else
        {
            jBufferCounter -= Time.deltaTime;
        }
    }

    private IEnumerator Dash()
    {
        cAnimator.SetBool("Jump", false);
        cAnimator.SetBool("isDashing", true);
        dash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        cAnimator.SetBool("isDashing", false);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        dash = true;
    }

    #endregion


    #region CHECKS
    void Flip()
    {
        if (facingRight && horz > 0f || !facingRight && horz < 0f)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


    //Checks if box overlaps with the ground layer and returns a bool
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(GCheck.position, GCheckSize, 0, Ground);
        

    }



    //Checks if box overlaps with the wall layer and returns a bool
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(WCheck.position, 0.2f, Walls);
    }

    void Animations()
    {
        cAnimator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));


    }
    #endregion


    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GCheck.position, GCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WCheck.position, WCheckSize);
    }
    #endregion
}
