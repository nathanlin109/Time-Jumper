using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Fields
    public GameObject player;
    public GameObject mainCamera;
    public GameObject platformManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        resetLevel();
    }

    // Resets the game on death
    private void resetLevel()
    {
        if (mainCamera.GetComponent<Camera>().stoppedMovingCamera)
        {
            // Plays fade animation
            GameObject.Find("Panel").GetComponent<Animator>().SetTrigger("Start Fade");

            // Resets scene
            mainCamera.GetComponent<Camera>().Reset();
            player.GetComponent<Player>().Reset();
            platformManager.GetComponent<PlatformManager>().Reset();
        }
    }
}
