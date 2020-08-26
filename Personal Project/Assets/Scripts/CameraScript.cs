using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Fields
    // For moving camera on death
    public GameObject player;
    public GameObject platformManager;
    public float cameraMoveSpeed;
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
    }

    // Update is called once per frame
    void Update()
    {
        StopCameraOnDeath();
    }
    
    // Stops camera when player dies
    void StopCameraOnDeath()
    {
        // Recenters camera on player death
        if (platformManager.GetComponent<PlatformManager>().stoppedMovingPlatforms)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(player.transform.position.x + cameraOffsetDeath, yInitialPos, zInitialPos),
                cameraMoveSpeed * Time.deltaTime);
        }

        if (transform.position.x == player.transform.position.x + cameraOffsetDeath)
        {
            stoppedMovingCamera = true;
        }
    }

    // Resets camera positions
    public void Reset()
    {
        stoppedMovingCamera = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}

