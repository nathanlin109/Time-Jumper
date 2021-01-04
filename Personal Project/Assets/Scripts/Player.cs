using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private enum Direction { Left = -1, Right = 1 }

    // Player fields
    public bool isGrounded;
    public float jumpForce;
    public float speed;
    private float maxSpeed;
    public bool isDead;
    public bool isDeadFromFall;
    public bool isDeadInsidePlatform;
    private Rigidbody2D playerRigidBody;

    // Wall slide
    public bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed;

    // Wall jump
    private bool isWallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;
    private Direction direction;
    public GameObject platformManager;
    public bool isInWallJumpSection; //(stop platforms)

    // Coyote time
    public float coyoteTime;
    private float coyoteJumpTimer;
    private bool releasedJumpInAir;

    // Charge jump
    private float chargeTimer;
    public float maxChargeTimer;
    public float chargeJumpMultiplier;
    public float chargeJumpDrag;

    // Distinguish charge jump from normal jump
    private bool jumpHeld;
    public float holdJumpDelay;
    private float holdJumpDelayTimer;

    // Coyote charge jump
    private bool pressedChargeJumpInAir;
    private float coyoteChargeJumpTimer;
    public bool shouldChargeJump;

    // Time switch mask
    public float expandMultiplier;
    private bool shouldExpandMask;
    public int maxMaskSize;
    private GameObject sceneManager;

    // For resetting player position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Bool for time switch
    private bool timeSwitchOnCooldown;
    private bool canTimeSwitch;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        // General player vars
        speed = 0;
        maxSpeed = platformManager.GetComponent<PlatformManager>().speed;
        isDead = false;
        isDeadInsidePlatform = false;
        isDeadFromFall = false;
        isGrounded = true;
        direction = Direction.Right;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();

        // Wall jump/slide
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        isInWallJumpSection = false; //(stop platforms)

        // Coyote time
        coyoteJumpTimer = 0;
        releasedJumpInAir = false;

        // Charge jump
        chargeTimer = 0;
        holdJumpDelayTimer = 0;
        jumpHeld = false;
        pressedChargeJumpInAir = false;
        shouldChargeJump = false;
        

        // Time switch mask
        shouldExpandMask = false;
        sceneManager = GameObject.Find("SceneManager");

        // initial positions
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;

        // Time Switch bool initialization
        timeSwitchOnCooldown = false;
        canTimeSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        if (isDead == false)
        {
            Inputs();
            WallSlide();
            CoyoteTimeJump();
            CoyoteTimeChargeJump();
        }
        else
        {
            // Stops player from moving when dead (except from falling)
            if (isDeadFromFall == false)
            {
                playerRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        ExpandTimeSwitchMask();
    }

    // For moving the player rigidbody
    private void FixedUpdate()
    {
        MoveDuringWallJumpSections();
        /*Debug.Log("TOTAL SPEED: " + (playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed));
        Debug.Log("PLAYER SPEED: " + (playerRigidBody.velocity.x));*/
    }

    // Key inputs
    private void Inputs()
    {
        if (GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused == false)
        {
            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Checks if player pressed space in air (used for charge jump, not normal jumps)
                if (!isGrounded && !isTouchingWall)
                {
                    pressedChargeJumpInAir = true;
                }
                else
                {
                    pressedChargeJumpInAir = false;
                }

                jumpHeld = false;
            }
            // Handles charge and normal jump (when let go of space)
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                // Normal jump (no hold)
                if (!jumpHeld)
                {
                    // Handles normal and wall jumps
                    NormalAndWallJump();

                    if (!isGrounded && !isTouchingWall)
                    {
                        releasedJumpInAir = true;
                    }
                }
                // Charge jump
                else
                {
                    // Handles charge jump
                    if (coyoteChargeJumpTimer <= coyoteTime || shouldChargeJump)
                    {
                        ChargeJump();
                        shouldChargeJump = false;
                    }
                }

                // Resets hold timer
                jumpHeld = false;
                holdJumpDelayTimer = 0;

                // Resets coyote charge jump
                coyoteChargeJumpTimer = 0;
                pressedChargeJumpInAir = false;

                // Resets charging timer
                chargeTimer = 0;
            }

            //  Charge jump (when holding)
            if (Input.GetKey(KeyCode.Space))
            {
                holdJumpDelayTimer += Time.deltaTime;

                // Only slows player down if they are going to charge jump
                if (holdJumpDelayTimer >= holdJumpDelay)
                {
                    // Slows player down
                    if (coyoteChargeJumpTimer <= coyoteTime || shouldChargeJump)
                    {
                        if (isGrounded == true)
                        {
                            transform.Translate(Vector2.left * chargeJumpDrag * Time.deltaTime);
                            shouldChargeJump = true;
                        }
                        else if (isWallSliding)
                        {
                            shouldChargeJump = true;
                        }
                    }

                    // Increments charge timer
                    if (chargeTimer <= maxChargeTimer)
                    {
                        chargeTimer += Time.deltaTime;
                    }
                    jumpHeld = true;
                }
            }

            // Time Switch
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!timeSwitchOnCooldown && canTimeSwitch)
                {
                    Invoke("ResetTimeSwitchCooldown", .35f);
                    shouldExpandMask = true;
                    sceneManager.GetComponent<SceneMan>().ChangeTimeState();
                    timeSwitchOnCooldown = true;
                }
            }
        }
    }

    // Normal and wall jump
    private void NormalAndWallJump()
    {
        // Normal jump
        if (isGrounded == true)
        {
            // Jump
            playerRigidBody.AddForce(new Vector2(0, jumpForce));

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
        // Wall jump
        else if (isWallSliding == true)
        {
            isWallJumping = true;

            // Switches direction
            if (direction == Direction.Right)
            {
                direction = Direction.Left;
            }
            else
            {
                direction = Direction.Right;
            }

            // Resets player velocity and wall jumps
            playerRigidBody.velocity = Vector2.zero;
              playerRigidBody.AddForce(
                new Vector2(xWallForce * (int)direction -
                platformManager.GetComponent<PlatformManager>().speed,
                yWallForce));

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
    }

    // Charge jump
    private void ChargeJump()
    {
        if (isGrounded == true)
        {
            // Charge jump
            playerRigidBody.AddForce(new Vector2(chargeTimer * chargeJumpMultiplier,
                jumpForce + chargeTimer * chargeJumpMultiplier));

            // Resets coyote charge jump timer
            pressedChargeJumpInAir = false;
            coyoteChargeJumpTimer = 0;
        }
        else if (isWallSliding == true)
        {
            isWallJumping = true;

            // Switches direction
            if (direction == Direction.Right)
            {
                direction = Direction.Left;
            }
            else
            {
                direction = Direction.Right;
            }

            // Resets player velocity and wall jumps
            playerRigidBody.velocity = Vector2.zero;

            // Wall jump (changes x speed depending on the platform speed)
            // Platform speed will usually be 0, but player can wall jump while it's 
            // slowing/speeding up to a stop/movespeed
            playerRigidBody.AddForce(
                new Vector2(xWallForce * (int)direction -
                platformManager.GetComponent<PlatformManager>().speed,
                yWallForce + chargeTimer * chargeJumpMultiplier * 4));

            // Resets coyote charge jump timer
            pressedChargeJumpInAir = false;
            coyoteChargeJumpTimer = 0;
        }
    }

    // Wall slide
    private void WallSlide()
    {
        // Checks if player is wall sliding
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Slides player down
        if (isWallSliding)
        {
            // Clamps the player's y velocity
            // Only do this if player's falling to prevent it from dragging on jump
            if (playerRigidBody.velocity.y <= 0)
            {
                // Slides down at slower speed when charge jumping
                if (jumpHeld)
                {
                    playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed / 10, float.MaxValue));
                }
                // Slides down at normal speed
                else
                {
                    playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
                }
            }
        }
    }

    // Moves player during wall jump sections when grounded
    private void MoveDuringWallJumpSections()
    {
        if (isInWallJumpSection == true && isWallJumping == false && isTouchingWall == false)
        {
            /*speed = Mathf.MoveTowards(speed,
                maxSpeed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);*/

            speed = maxSpeed - platformManager.GetComponent<PlatformManager>().speed;

            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed >= maxSpeed &&
                playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed < maxSpeed &&
                playerRigidBody.velocity.x > 0)
            {
                speed = maxSpeed - playerRigidBody.velocity.x - platformManager.GetComponent<PlatformManager>().speed;
            }

            // Moves player to the right
            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed <= maxSpeed)
            {
                //transform.Translate(Vector2.right * speed * Time.deltaTime);
                Vector3 newVelocity = playerRigidBody.velocity;
                newVelocity.x += speed;
                playerRigidBody.velocity = newVelocity;
            }
        }
        else if (isInWallJumpSection == false && isWallJumping == false && speed != 0)
        {
            /*speed = 0;

            speed = Mathf.MoveTowards(speed,
                0,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);*/

            speed = maxSpeed - platformManager.GetComponent<PlatformManager>().speed;

            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed >= maxSpeed &&
                playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed < maxSpeed &&
                playerRigidBody.velocity.x > 0)
            {
                speed = maxSpeed - playerRigidBody.velocity.x - platformManager.GetComponent<PlatformManager>().speed;
            }

            // Moves player to the right
            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed <= maxSpeed)
            {
                //transform.Translate(Vector2.right * speed * Time.deltaTime);
                Vector3 newVelocity = playerRigidBody.velocity;
                newVelocity.x += speed;
                playerRigidBody.velocity = newVelocity;
            }
        }
    }

    // Coyote time for jumping
    private void CoyoteTimeJump()
    {
        if (releasedJumpInAir)
        {
            coyoteJumpTimer += Time.deltaTime;

            // Resets timer
            if (coyoteJumpTimer >= coyoteTime)
            {
                releasedJumpInAir = false;
                coyoteJumpTimer = 0;
            }
            else
            {
                // Handles normal and wall jumps
                NormalAndWallJump();
            }
        }
    }

    // Coyote time for charge jumping
    private void CoyoteTimeChargeJump()
    {
        if (pressedChargeJumpInAir)
        {
            coyoteChargeJumpTimer += Time.deltaTime;
        }
    }

    // Expands time switch mask
    private void ExpandTimeSwitchMask()
    {
        if (shouldExpandMask)
        {
            gameObject.transform.Find("TimeSwitchMask").transform.localScale +=
                new Vector3(expandMultiplier * Time.deltaTime, expandMultiplier * Time.deltaTime, 0);

            // Resets variables after finished expanding
            if (gameObject.transform.Find("TimeSwitchMask").transform.localScale.x >= maxMaskSize)
            {
                shouldExpandMask = false;

                // Switches which platforms should be rendered inside and outside mask
                sceneManager.GetComponent<SceneMan>().SwapTimeMasks();

                // Resets mask
                gameObject.transform.Find("TimeSwitchMask").transform.localScale = new Vector3(.5f, .5f, 0f);
            }
        }
    }

    // Collision stay listener
    void OnCollisionStay2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            direction = Direction.Right;
            isWallJumping = false;
        }
        // Collision with wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    // Collision exit listener
    void OnCollisionExit2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
        // Collision with wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    // Enters but don't collide
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collision with collider indicating end of level. We switch the scene here.
        if (collision.gameObject.CompareTag("EndLevelZone"))
        {
            // Saves the latest level player is at
            if (SceneManager.GetActiveScene().buildIndex + 1 > PlayerPrefs.GetInt("latestLevel", 1))
            {
                PlayerPrefs.SetInt("latestLevel", SceneManager.GetActiveScene().buildIndex + 1);
            }

            SceneManager.LoadScene(4, LoadSceneMode.Single);
        }

        // Collision with stop camera (wall jumping)
        else if (collision.gameObject.CompareTag("StopCamera"))
        {
            isInWallJumpSection = true;
        }

        // Entering time switch zone. Should enable the ability to time switch
        else if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = true;
        }

        // Entering death zone. Should kill the player and trigger reset method
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            isDead = true;
            isDeadFromFall = true;
        }

        // Colliding with spikes. Should kill the player and trigger reset method
        else if (collision.gameObject.CompareTag("Spike"))
        {
            isDead = true;
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            isDead = true;
        }
        else if (collision.gameObject.CompareTag("InsidePlatformKill"))
        {
            isDead = true;
            isDeadInsidePlatform = true;
        }
    }

    // Exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Collision with stop camera (wall jumping)
        if (collision.gameObject.CompareTag("StopCamera"))
        {
            isInWallJumpSection = false;
        }

        // Exiting time switch zone. Should disable the ability to time switch
        else if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Entering time switch zone. Should enable the ability to time switch
        if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = true;
        }
    }

    // Resets cooldown for Time Switch
    private void ResetTimeSwitchCooldown()
    {
        timeSwitchOnCooldown = false;
    }

    // Resets player positions
    public void Reset()
    {
        speed = 0;
        isDead = false;
        isDeadFromFall = false;
        isGrounded = true;
        isInWallJumpSection = false;
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        direction = Direction.Right;
        coyoteJumpTimer = 0;
        releasedJumpInAir = false;
        chargeTimer = 0;
        holdJumpDelayTimer = 0;
        jumpHeld = false;
        pressedChargeJumpInAir = false;
        coyoteChargeJumpTimer = 0;
        shouldChargeJump = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
        timeSwitchOnCooldown = false;
        canTimeSwitch = false;
        shouldExpandMask = false;
        isDeadInsidePlatform = false;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}