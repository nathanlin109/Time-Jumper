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

    // For resetting player position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        speed = 0;
        isDead = false;
        isGrounded = true;
        shouldStopPlatforms = false;
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        direction = Direction.Right;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();
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
    }

    // Key inputs
    private void Inputs()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidBody.AddForce(new Vector2(0, jumpForce));
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

                playerRigidBody.AddForce(new Vector2(xWallForce * (int)direction, yWallForce));
            }
        }
    }

    // Moves player during wall jump sections when grounded
    private void moveDuringWallJump()
    {
        if (shouldStopPlatforms && isGrounded && !isTouchingWall)
        {
            speed = Mathf.MoveTowards(speed,
                maxSpeed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);

            // Moves enemy to the right
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    // Collision enter listener
    void OnCollisionStay2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            direction = Direction.Right;
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
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}