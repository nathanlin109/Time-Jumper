using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum Direction { Left = -1, Right = 1 }

    // Player fields
    private bool isGrounded;
    public float jumpForce;
    private float speed;
    public float maxSpeed;
    public bool isDead;
    private Rigidbody2D playerRigidBody;

    // For stopping camera (when wall jump section happens)
    // Use the stop camera prefab instead of checking if wall jumping b/c
    // player will exit prefab area when done, but we don't know when to move camera again 
    // if they wall jump multiple times
    public bool shouldStopPlatforms;

    // Wall slide
    private bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed;

    // Wall jump
    private bool isWallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;
    private Direction direction;
    public GameObject platformManager;

    // Coyote time
    public float coyoteTime;
    private float coyoteJumpTimer;
    private bool pressedJumpInAir;

    // Charge jump
    private float chargeTimer;
    private float chargeDelayTimer;
    private bool jumpHeld;
    public float chargeDelay;

    // For resetting player position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        // General player vars
        speed = 0;
        isDead = false;
        isGrounded = true;
        direction = Direction.Right;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();

        // For death
        shouldStopPlatforms = false;

        // Wall jump/slide
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;

        // Coyote time
        coyoteJumpTimer = 0;
        pressedJumpInAir = false;

        // Charge jump
        chargeTimer = 0;
        chargeDelayTimer = 0;

        // initial positions
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        Inputs();
        moveDuringWallJump();
        CoyoteTimeJump();
    }

    // Key inputs
    private void Inputs()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                playerRigidBody.AddForce(new Vector2(0, jumpForce));
                pressedJumpInAir = false;
            }
            else
            {
                pressedJumpInAir = true;
            }
        }

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
                playerRigidBody.velocity = new Vector2(
                    playerRigidBody.velocity.x,
                    Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
            }

            // Checks if player tries to wall jump
            if (Input.GetKeyDown(KeyCode.Space))
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

                pressedJumpInAir = false;
            }
        }

        // Charge Jump
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * .5f * Time.deltaTime);
        }
    }

    // Moves player during wall jump sections when grounded
    private void moveDuringWallJump()
    {
        if (shouldStopPlatforms && !isWallJumping && !isTouchingWall)
        {
            speed = Mathf.MoveTowards(speed,
                maxSpeed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);

            // Moves player to the right
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    // Coyote time for jumping
    private void CoyoteTimeJump()
    {
        if (pressedJumpInAir)
        {
            coyoteJumpTimer += Time.deltaTime;

            // Resets timer
            if (coyoteJumpTimer >= coyoteTime)
            {
                pressedJumpInAir = false;
                coyoteJumpTimer = 0;
            }
            else
            {
                // Jumps player once they've hit the ground and withing coyote time
                if (isGrounded)
                {
                    playerRigidBody.AddForce(new Vector2(0, jumpForce));

                    // Resets timer
                    pressedJumpInAir = false;
                    coyoteJumpTimer = 0;
                }
                else if (isWallSliding)
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

                    // Resets timer
                    pressedJumpInAir = false;
                    coyoteJumpTimer = 0;
                }
            }
        }
    }

    // Collision enter listener
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            direction = Direction.Right;
            isWallJumping = false;
        }
        // Collision with enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isDead = true;
        }
        // Collision with wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
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
        // Collision with enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isDead = true;
        }
        // Collision with wall
        if (collision.gameObject.CompareTag("Wall"))
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    // Enters but don't collide
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collision with stop camera (wall jumping)
        if (collision.gameObject.CompareTag("StopCamera"))
        {
            shouldStopPlatforms = true;
        }
    }

    // Exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Collision with stop camera (wall jumping)
        if (collision.gameObject.CompareTag("StopCamera"))
        {
            shouldStopPlatforms = false;
        }
    }

    // Resets player positions
    public void Reset()
    {
        speed = 0;
        isDead = false;
        isGrounded = true;
        shouldStopPlatforms = false;
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        direction = Direction.Right;
        coyoteJumpTimer = 0;
        pressedJumpInAir = false;
        chargeTimer = 0;
        chargeDelayTimer = 0;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}