using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum declaration for past and future elements
public enum TimeState
{
    Past,
    Future
}

public class PlatformManager : MonoBehaviour
{
    // Fields
    // Determines which time state the level is in
    public TimeState currentLevelTimeState;

    // To hold future and past gameobjects
    private GameObject pastManager;
    private GameObject futureManager;

    // For moving camera
    public float speed;
    private float initialSpeed;
    public float DeathSpeedMultiplier;
    public float WallJumpSpeedMultiplier;
    public GameObject player;
    public bool stoppedMovingPlatforms;

    // For resetting platform position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        initialSpeed = speed;
        stoppedMovingPlatforms = false;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
        currentLevelTimeState = TimeState.Past;
        pastManager = GameObject.Find("PastManager");
        futureManager = GameObject.Find("FutureManager");
        futureManager.SetActive(false);
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
            speed = Mathf.MoveTowards(speed, 0, DeathSpeedMultiplier * Time.deltaTime);

            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (speed == 0)
            {
                stoppedMovingPlatforms = true;
            }
        }
        else
        {
            // Checks to stop platforms when player enters wall jump area
            if (player.GetComponent<Player>().shouldStopPlatforms)
            {
                speed = Mathf.MoveTowards(speed, 0, WallJumpSpeedMultiplier * Time.deltaTime);
            }
            else
            {
                speed = Mathf.MoveTowards(speed, initialSpeed, WallJumpSpeedMultiplier * Time.deltaTime);
            }

            // Moves platforms to the left
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

    // Changes Time State when signal is received from Player script
    public void ChangeTimeState()
    {
        switch (currentLevelTimeState)
        {
            case TimeState.Future:
                // Changes time state
                currentLevelTimeState = TimeState.Past;

                // Toggles visibility of all platforms within the specific platform manager
                DisableVisibility(futureManager);
                EnableVisibility(pastManager);

                // Leaves the collider in the scene for a bit to make the game a bit more forgiving and then turns it off
                Invoke("DisableFuturePlatformsActiveState", 0.2f);

                // Activates the new platforms
                pastManager.SetActive(true);
                break;

            case TimeState.Past:
                // Changes time state
                currentLevelTimeState = TimeState.Future;

                // Toggles visibility of all platforms within the specific platform manager
                DisableVisibility(pastManager);
                EnableVisibility(futureManager);

                // Leaves the collider in the scene for a bit to make the game a bit more forgiving and then turns it off
                Invoke("DisablePastPlatformsActiveState", 0.2f);

                // Activates the new platforms
                futureManager.SetActive(true);
                break;
        }
    }

    // Enables visibility of specified platforms
    private void EnableVisibility(GameObject platformManager)
    {
        SpriteRenderer[] spArray = platformManager.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < spArray.Length; i++)
        {
            spArray[i].enabled = true;
        }
    }

    // Disables visibility of specified platforms
    private void DisableVisibility(GameObject platformManager)
    {
        SpriteRenderer[] spArray = platformManager.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < spArray.Length; i++)
        {
            spArray[i].enabled = false;
        }
    }

    // Function for disabling active state of future platform manager. Mainly used to feed into Invoke method
    private void DisableFuturePlatformsActiveState()
    {
        futureManager.SetActive(false);
    }

    // Function for disabling active state of past platform manager. Mainly used to feed into Invoke method
    private void DisablePastPlatformsActiveState()
    {
        pastManager.SetActive(false);
    }

    // Resets platform positions
    public void Reset()
    {
        speed = initialSpeed;
        stoppedMovingPlatforms = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}
