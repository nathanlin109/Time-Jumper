using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Fields
    // For moving camera on death
    private Player player;
    private PlatformManager platformManager;
    public float deathMoveSpeed;
    public float wallJumpMoveSpeed;
    public float wallJumpMoveSpeedMultiplier;
    private float wallJumpCurrentSpeed;
    public float cameraOffsetDeath;
    public bool stoppedMovingCamera;

    // For resetting camera position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        stoppedMovingCamera = false;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
        wallJumpCurrentSpeed = 0;

        platformManager = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        StopCameraOnDeath();
        RecenterCamera();
    }
    
    // Stops camera when player dies
    void StopCameraOnDeath()
    {
        // Recenters camera on player death
        if (player.isDead)
        {
            if (platformManager.stoppedMovingPlatforms && !player.isDeadFromFall)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(player.transform.position.x + cameraOffsetDeath, yInitialPos, zInitialPos),
                    deathMoveSpeed * Time.deltaTime);
            }
            else if (platformManager.stoppedMovingPlatforms && player.isDeadFromFall)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xInitialPos, yInitialPos - (cameraOffsetDeath * 1.5f), zInitialPos),
                    deathMoveSpeed * Time.deltaTime);
            }

            if (transform.position.x == player.transform.position.x + cameraOffsetDeath)
            {
                stoppedMovingCamera = true;
            }
            else if (transform.position.y == yInitialPos - (cameraOffsetDeath * 1.5f))
            {
                stoppedMovingCamera = true;
            }
        }
    }

    // Recenters camera after wall jump section
    void RecenterCamera()
    {
        // Recenters only when player isn't in wall jump section
        if (!player.isInWallJumpSection && !player.recenteredCamera)
        {
            // Increases speed to match platform speed, so it's linear instead of accelerating towards player
            wallJumpCurrentSpeed = Mathf.MoveTowards(wallJumpCurrentSpeed,
                wallJumpMoveSpeed, wallJumpMoveSpeedMultiplier * Time.deltaTime);

            // Moves camera towards player
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(player.transform.position.x + cameraOffsetDeath, yInitialPos, zInitialPos),
                wallJumpCurrentSpeed * Time.deltaTime);
        }

        if (transform.position.x == player.transform.position.x + cameraOffsetDeath)
        {
            player.recenteredCamera = true;
            wallJumpCurrentSpeed = 0;
        }
    }

    // Resets camera positions
    public void Reset()
    {
        stoppedMovingCamera = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}

