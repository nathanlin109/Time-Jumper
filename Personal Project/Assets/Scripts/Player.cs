using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fields
    private bool isGrounded;
    public float jumpForce;
    public float speed;
    public bool isDead;

    // all jump
    private bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed;

    // For resetting camera
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        isDead = false;
        isGrounded = true;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        Inputs();
    }

    // Key inputs
    private void Inputs()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }

        // Wall Jump
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Collision enter listener
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
        // Collision with enemy
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            isDead = true;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    // Collision exit listener
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    // Resets player positions
    public void Reset()
    {
        isDead = false;
        isGrounded = true;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}