using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    //Components
    public Rigidbody2D rb;

    //Gravedad
    [Header("Gravedad")]
    public float gravityStr;
    public float gravityScale;
    public float fallgravMultiplier;
    public float maxFallSpeed;
    public float fastFallGravityMult;
    public float maxFastFallSpeed;

    //Variables controlando las acciones del jugador
    public bool isFacingRight { get; private set; }
    public bool isJumping { get; private set; }
    public bool isWallJumping { get; private set; }
    public bool isDashing { get; private set; }
    public bool isSliding { get; private set; }


    //Temporizadores
    public float lastOnGround { get; private set; }
    public float lastOnWall { get; private set; }
    public float lastOnWallRight { get; private set; }
    public float lastOnWallLeft { get; private set; }

    //Jump
    [Header("Jump")]
    private bool isJump;
    private bool isJumpFalling;
    private float jumpForce;
    public float jumpHeight;
    public float jumpTimeToApex;

    //Both Jumps
    [Header("Both Jumps")]
    public float jumpCutGravityMult; //Multiplicador para aumentar la gravedad si el player el jugador deja de pulsar el boton de salto mientras salta
    [Range(0f, 1)] public float jumpHangGravityMult;
    public float jumpHangTimeThreshold;
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    //Wall Jump
    [Header("WallJump")]
    private float wallJumpStartT;
    private int lastWallJumpDirection;
    public Vector2 wallJumpForce;
    [Range(0f, 1f)] public float wallJumpRunLerp;
    [Range(0f, 1f)] public float wallJumpTime;
    public bool turnOnWallJump;

    //WallSlide
    [Header("WallSlide")]
    public float slideSpeed;
    public float slideAccel;

    //Assists
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Periodo de gracia despues de caer de una plataforma
    [Range(0.01f, 0.5f)] public float jumpBufferTime; //Periodo de gracia donde el salto se hara cuando cumpla los requerimientos (grounded)


    [Header("Move & Run")]
    public float runMaxSpeed;
    public float runAccel;
    public float runAccelAmount;
    public float runDeccel;
    public float runDeccelAmount;
    [Space(5)]
    [Range(0f, 1f)] public float accelInAir;
    [Range(0f, 1f)] public float deccelInAir;
    [Space(5)]
    public bool conserveMomentum = true;

    //Dash
    [Header("Dash")]
    private bool dashAtt;
    public int dashAmount;
    public float dashSpeed;
    public float sleepTime;
    [Space(5)]
    public float dashAttackTime;
    [Space(5)]
    public float dashEndTime;
    public Vector2 dashEndSpeed; //Relentiza al jugador
    [Range(0f, 1f)] public float dashEndRunLerp;
    [Space(5)]
    public float dashRefillTime;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;
    private int dashesleft;
    private bool dashrefill;
    private Vector2 lastDashDirection;

    //Inputs
    private Vector2 move;

    public float lastPressedJumpTime { get; private set; }
    public float lastPressedDashTime { get; private set; }

    //Checks
    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.50f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform frontWallCheck;
    [SerializeField] private Transform backWallCheck;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);

    //Layers
    [Header("Layer")]
    [SerializeField] private LayerMask Environment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        SetGravityScale(gravityScale);
        isFacingRight = true;
    }

    private void Update()
    {
        #region TIMERS
        lastOnGround -= Time.deltaTime;
        lastOnWall -= Time.deltaTime;
        lastOnWallRight -= Time.deltaTime;
        lastOnWallLeft -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        if (move.x != 0)
            CheckDirectionToFace(move.x > 0);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
        {
            OnDashInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!isDashing && !isJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, Environment)) //checks if set box overlaps with ground
            {
                

                lastOnGround = coyoteTime; //if so sets the lastGrounded to coyoteTime
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheck.position, wallCheckSize, 0, Environment) && isFacingRight)
                    || (Physics2D.OverlapBox(backWallCheck.position, wallCheckSize, 0, Environment) && !isFacingRight)) && !isWallJumping)
                lastOnWallRight = coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheck.position, wallCheckSize, 0, Environment) && !isFacingRight)
                || (Physics2D.OverlapBox(backWallCheck.position, wallCheckSize, 0, Environment) && isFacingRight)) && !isWallJumping)
                lastOnWallLeft = coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            lastOnWall = Mathf.Max(lastOnWallLeft, lastOnWallRight);
        }
        #endregion

        #region JUMP CHECKS
        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;

            isJumpFalling = true;
        }

        if (isWallJumping && Time.time - wallJumpStartT > wallJumpTime)
        {
            isWallJumping = false;
        }

        if (lastOnGround > 0 && !isJumping && !isWallJumping)
        {
            isJump = false;

            isJumpFalling = false;
        }

        if (!isDashing)
        {
            //Jump
            if (CanJump() && lastPressedJumpTime > 0)
            {
                isJumping = true;
                isWallJumping = false;
                isJump = false;
                isJumpFalling = false;
                Jump();

                
            }
            //WALL JUMP
            else if (CanWallJump() && lastPressedJumpTime > 0)
            {
                isWallJumping = true;
                isJumping = false;
                isJump = false;
                isJumpFalling = false;

                wallJumpStartT = Time.time;
                lastWallJumpDirection = (lastOnWallRight > 0) ? -1 : 1;

                WallJump(lastWallJumpDirection);
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && lastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            Sleep(sleepTime);

            //If not direction pressed, dash forward
            if (move != Vector2.zero)
                lastDashDirection = move;
            else
                lastDashDirection = isFacingRight ? Vector2.right : Vector2.left;



            isDashing = true;
            isJumping = false;
            isWallJumping = false;
            isJump = false;

            StartCoroutine(nameof(StartDash), lastDashDirection);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((lastOnWallLeft > 0 && move.x < 0) || (lastOnWallRight > 0 && move.x > 0)))
            isSliding = true;
        else
            isSliding = false;
        #endregion

        #region GRAVITY
        if (!dashAtt)
        {
            //Higher gravity if we've released the jump input or are falling
            if (isSliding)
            {
                SetGravityScale(0);
            }
            else if (rb.linearVelocity.y < 0 && move.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(gravityScale * fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFastFallSpeed));
            }
            else if (isJump)
            {
                //Higher gravity if jump button released
                SetGravityScale(gravityScale * jumpCutGravityMult);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
            }
            else if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < jumpHangTimeThreshold)
            {
                SetGravityScale(gravityScale * jumpHangGravityMult);
            }
            else if (rb.linearVelocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(gravityScale * fallgravMultiplier);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //Handle Run
        if (!isDashing)
        {
            if (isWallJumping)
                Run(wallJumpRunLerp);
            else
                Run(1);
        }
        else if (dashAtt)
        {
            Run(dashEndRunLerp);
        }

        //Handle Slide
        if (isSliding)
            Slide();
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            isJump = true;
    }

    public void OnDashInput()
    {
        lastPressedDashTime = dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = move.x * runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (lastOnGround > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.linearVelocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (conserveMomentum && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGround < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGround = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = jumpForce;
        if (rb.linearVelocity.y < 0)
            force -= rb.linearVelocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGround = 0;
        lastOnWallRight = 0;
        lastOnWallLeft = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= rb.linearVelocity.x;

        if (rb.linearVelocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.linearVelocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        lastOnGround = 0;
        lastPressedDashTime = 0;

        float startTime = Time.time;

        dashAmount--;
        dashAtt = true;

        SetGravityScale(0);

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= dashAttackTime)
        {
            rb.linearVelocity = dir.normalized * dashSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTime = Time.time;

        dashAtt = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        SetGravityScale(gravityScale);
        rb.linearVelocity = dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= dashEndTime)
        {
            yield return null;
        }

        //Dash over
        isDashing = false;
    }

    //Short period before the player is able to dash again
    private IEnumerator RefillDash(int amount)
    {
        //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        dashrefill = true;
        yield return new WaitForSeconds(dashRefillTime);
        dashrefill = false;
        dashesleft = Mathf.Min(dashAmount, dashesleft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        //We remove the remaining upwards Impulse to prevent upwards sliding
        if (rb.linearVelocity.y > 0)
        {
            rb.AddForce(-rb.linearVelocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = slideSpeed - rb.linearVelocity.y;
        float movement = speedDif * slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return lastOnGround > 0 && !isJumping;
    }

    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWall > 0 && lastOnGround <= 0 && (!isWallJumping ||
             (lastOnWallRight > 0 && lastWallJumpDirection == 1) || (lastOnWallLeft > 0 && lastWallJumpDirection == -1));
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.linearVelocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.linearVelocity.y > 0;
    }

    private bool CanDash()
    {
        if (!isDashing && dashesleft < dashAmount && lastOnGround > 0 && !dashrefill)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesleft > 0;
    }

    public bool CanSlide()
    {
        if (lastOnWall > 0 && !isJumping && !isWallJumping && !isDashing && lastOnGround <= 0)
            return true;
        else
            return false;
    }
    #endregion

    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStr = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStr / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAccel) / runMaxSpeed;
        runDeccelAmount = (50 * runDeccel) / runMaxSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStr) * jumpTimeToApex;

        #region Variable Ranges
        runAccel = Mathf.Clamp(runAccel, 0.01f, runMaxSpeed);
        runDeccel = Mathf.Clamp(runDeccel, 0.01f, runMaxSpeed);
        #endregion
    }

}
