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

    // For resetting camera
    public float xInitialPos;
    public float yInitialPos;
    public float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        isDead = false;
        isGrounded = true;
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }
    }

    // Collision enter listener
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }

       
        else if (collision.gameObject.tag == "Enemy")
        {
            isDead = true;
        }
    }

    // Collision exit listener
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = false;
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
