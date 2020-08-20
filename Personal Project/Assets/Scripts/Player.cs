using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fields
    private bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
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
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 200f));
        }
    }

    // Collision enter listener
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION ENTERING");
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }

    // Collision exit listener
    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("COLLISION EXITING");
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = false;
        }
    }
}
