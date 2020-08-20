using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 yPosition;
    private Vector2 yVelocity;
    private Vector2 yAcceleration;
    private float mass;


    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        mass = 1;
        yPosition = new Vector2(0, 0);
        yVelocity = new Vector2(0, 0);
        yAcceleration = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        Inputs();
    }

    // Updates physics
    private void updatePhysics()
    {
        yVelocity += yAcceleration * Time.deltaTime;
        yPosition += yVelocity * Time.deltaTime;
        yAcceleration = Vector2.zero;
    }

    // Key inputs
    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddForce(new Vector2(0, 5));
        }
    }

    // Forces
    private void AddForce(Vector2 force)
    {
        yAcceleration += force / mass;
    }
}
