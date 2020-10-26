using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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
    public float cameraOffsetX;
    public float cameraOffsetY;

    // For resetting camera position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // For vertical recentering camera
    public float upRecenterSpeed;
    public float downRecenterSpeed;
    private bool finishedMovingCamera;
    float previousCameraPosY;
    public float verticalRecenterOffset;
    float initialYDistance;
    public float maxRecenterHeight;

    // Start is called before the first frame update
    void Start()
    {
        stoppedMovingCamera = false;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
        wallJumpCurrentSpeed = 0;
        finishedMovingCamera = true;
        previousCameraPosY = transform.position.y;
        initialYDistance = 0;

        platformManager = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        StopCameraOnDeath();
        RecenterCameraWallJump();
        VerticalRecenterCamera();
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
    void RecenterCameraWallJump()
    {
        // Recenters only when player isn't in wall jump section
        if (!player.isInWallJumpSection && !player.recenteredCameraWallJump)
        {
            // Increases speed to match platform speed, so it's linear instead of accelerating towards player
            wallJumpCurrentSpeed = Mathf.MoveTowards(wallJumpCurrentSpeed,
                wallJumpMoveSpeed, wallJumpMoveSpeedMultiplier * Time.deltaTime);

            // Moves camera towards player
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(player.transform.position.x + cameraOffsetDeath, player.transform.position.y + cameraOffsetY, zInitialPos),
                wallJumpCurrentSpeed * Time.deltaTime);
        }

        if (transform.position.x == player.transform.position.x + cameraOffsetDeath)
        {
            player.recenteredCameraWallJump = true;
            wallJumpCurrentSpeed = 0;
        }
    }

    // Recenters camera normally
    void VerticalRecenterCamera()
    {
        // Checks vertical distance between player and camera
        float YdistBetweenCameraAndPlayer = transform.position.y - player.transform.position.y;

        // If vertical distance between player and camera exceeds a certain threshold, move the camera by a set number of units
        if (player.isDead == false && Mathf.Abs(YdistBetweenCameraAndPlayer) >= maxRecenterHeight || finishedMovingCamera == false)
        {
            // Captures some initial variables when camera starts moving
            if (finishedMovingCamera)
            {
                previousCameraPosY = transform.position.y;
                initialYDistance = YdistBetweenCameraAndPlayer;
            }

            finishedMovingCamera = false;

            // Recenters camera vertically
            // Going up
            if (initialYDistance < 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, 
                    new Vector3(xInitialPos,
                    transform.position.y + verticalRecenterOffset,
                    -15f),
                    Time.deltaTime * upRecenterSpeed);
            }
            // Going down
            else if (initialYDistance > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(xInitialPos,
                    transform.position.y - verticalRecenterOffset,
                    -15f),
                    Time.deltaTime * downRecenterSpeed);
            }

            // Checks if camera has finished recentering vertically and sets bool to true
            if ((transform.position.y >= previousCameraPosY + verticalRecenterOffset || 
                transform.position.y <= previousCameraPosY -verticalRecenterOffset) && 
                finishedMovingCamera == false)
            {
                finishedMovingCamera = true;
            }
        }
    }

    // Resets camera positions
    public void Reset()
    {
        stoppedMovingCamera = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
        finishedMovingCamera = true;
    }
}

