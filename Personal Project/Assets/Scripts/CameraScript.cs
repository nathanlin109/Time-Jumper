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

    // For resetting camera
    public float xInitialPos;
    public float yInitialPos;
    public float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        stoppedMovingCamera = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Recenters camera on player death
        if (platformManager.GetComponent<PlatformManager>().stoppedMovingPlatforms)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(player.transform.position.x + cameraOffsetDeath, 1f, -5f),
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

