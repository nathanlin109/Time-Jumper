using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // For moving enemy when platforms stop moving due to wall jumps
    public GameObject player;
    public GameObject platformManager;
    private float speed;
    public float maxSpeed;

    // For resetting enemy position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
    }

    // Starts moving towards player if platforms have stopped
    private void MoveTowardsPlayer()
    {
        if (player.GetComponent<Player>().shouldStopPlatforms)
        {
            speed = Mathf.MoveTowards(speed,
                maxSpeed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);

            // Moves enemy to the right
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    public void Reset()
    {
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
        speed = 0;
    }
}
