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
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Resets the game on death
    public void ResetLevel()
    {
        // Plays fade animation
        GameObject.Find("Panel").GetComponent<Animator>().SetTrigger("Start Fade");

        // Gets all the parallax layers
        arrayOfParallax = GameObject.FindGameObjectsWithTag("Parallax");

        // Resets scene
        mainCamera.GetComponent<CameraScript>().Reset();
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
