﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    private bool releasedJumpInAir;

    // Charge jump
    private float chargeTimer;
    public float maxChargeTimer;
    public float chargeJumpMultiplier;

    // Distinguish charge jump from normal jump
    private bool jumpHeld;
    public float holdJumpDelay;
    private float holdJumpDelayTimer;

    // Coyote charge jump
    private bool pressedChargeJumpInAir;
    private float coyoteChargeJumpTimer;
    private bool shouldChargeJump;

    // For resetting player position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Bool for time switch
    private bool timeSwitchOnCooldown;

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
        releasedJumpInAir = false;

        // Charge jump
        chargeTimer = 0;
        holdJumpDelayTimer = 0;
        jumpHeld = false;
        pressedChargeJumpInAir = false;
        shouldChargeJump = false;

        // initial positions
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;

        timeSwitchOnCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        Inputs();
        WallSlide();
        MoveDuringWallJumpSections();
        CoyoteTimeJump();
        CoyoteTimeChargeJump();
    }

    // Key inputs
    private void Inputs()
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
        // Charge jump (when let go)
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

            // Resets timer
            jumpHeld = false;
            holdJumpDelayTimer = 0;

            // Resets coyote charge jump
            coyoteChargeJumpTimer = 0;
            pressedChargeJumpInAir = false;
        }

        //  Charge jump (when holding)
        if (Input.GetKey(KeyCode.Space))
        {
            holdJumpDelayTimer += Time.deltaTime;

            // Only slows player down if they are going to charge jump
            if (holdJumpDelayTimer >= holdJumpDelay)
            {
                // Slows player down
                if ((isGrounded && coyoteChargeJumpTimer <= coyoteTime) || shouldChargeJump)
                {
                    transform.Translate(Vector2.left * .5f * Time.deltaTime);
                    shouldChargeJump = true;
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
            if (!timeSwitchOnCooldown)
            {
                Invoke("ResetTimeSwitchCooldown", 1.5f);
                platformManager.GetComponent<PlatformManager>().ChangeTimeState();
                timeSwitchOnCooldown = true;
            }
        }
    }

    // Normal and wall jump
    private void NormalAndWallJump()
    {
        // Normal jump
        if (isGrounded)
        {
            // Jump
            playerRigidBody.AddForce(new Vector2(0, jumpForce));

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
        // Wall jump
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

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
    }

    // Charge jump
    private void ChargeJump()
    {
        if (isGrounded)
        {
            // Charge jump
            playerRigidBody.AddForce(new Vector2(chargeTimer * chargeJumpMultiplier,
                jumpForce + chargeTimer * chargeJumpMultiplier));

            // Resets coyote charge jump timer
            pressedChargeJumpInAir = false;
            coyoteChargeJumpTimer = 0;
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

            // Wall jump (changes x speed depending on the platform speed)
            // Platform speed will usually be 0, but player can wall jump while it's 
            // slowing/speeding up to a stop/movespeed
            playerRigidBody.AddForce(
                new Vector2(xWallForce * (int)direction -
                platformManager.GetComponent<PlatformManager>().speed,
                yWallForce + chargeTimer * chargeJumpMultiplier * 2));
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
                // Slides down at half speed when charg jumping
                if (jumpHeld)
                {
                    playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed / 2, float.MaxValue));
                }
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
        isGrounded = true;
        shouldStopPlatforms = false;
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
    }
}