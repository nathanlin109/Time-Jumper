using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    // Fields
    // For moving camera
    public float speed;
    private float deathSpeed;
    public float deathSpeedMultiplier;
    public GameObject player;
    public bool stoppedMovingPlatforms;

    // For resetting camera
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        deathSpeed = speed;
        stoppedMovingPlatforms = false;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatforms();
    }

    // Moves platforms (normal and ondeath)
    private void MovePlatforms()
    {
        if (player.GetComponent<Player>().isDead)
        {
            // Slows platforms
            deathSpeed *= deathSpeedMultiplier;

            // Makes speed 0 when its low enough
            if (deathSpeed <= .05f)
            {
                deathSpeed = 0;
                stoppedMovingPlatforms = true;
            }

            transform.Translate(Vector2.left * deathSpeed * Time.deltaTime);
        }
        else
        {
            // Moves playforms to the left
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

    // Resets platform positions
    public void Reset()
    {
        deathSpeed = speed;
        stoppedMovingPlatforms = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}
