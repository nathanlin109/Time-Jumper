using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Fields
    public GameObject player;
    public GameObject enemy;
    public GameObject mainCamera;
    public GameObject platformManager;
    public GameObject[] arrayOfParallax;

    // Start is called before the first frame update
    void Start()
    {
        arrayOfParallax = GameObject.FindGameObjectsWithTag("Parallax");
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
            enemy.GetComponent<Enemy>().Reset();

            // Looping through all the parallax layers and calling reset on them
            for (int i = 0; i < arrayOfParallax.Length; i++)
            {
                arrayOfParallax[i].GetComponent<ParallaxEffect>().Reset();
            }
        }
    }
}
