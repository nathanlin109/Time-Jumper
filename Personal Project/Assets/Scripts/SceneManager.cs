﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Enum declaration for past and future elements
public enum TimeState
{
    Past,
    Future
}

public class SceneManager : MonoBehaviour
{
    // Fields
    // Determines which time state the level is in
    public TimeState currentLevelTimeState;
    public TimeState defaultLevelTimeState;

    public GameObject player;
    public GameObject enemy;
    public GameObject mainCamera;
    public GameObject platformManager;
    public GameObject[] arrayOfParallax;
    private GameObject pastManager;
    private GameObject futureManager;
    private GameObject pastParallax;
    private GameObject futureParallax;

    // Start is called before the first frame update
    void Start()
    {
        pastManager = GameObject.Find("PastManager");
        futureManager = GameObject.Find("FutureManager");
        pastParallax = GameObject.Find("PastBackgroundManager");
        futureParallax = GameObject.Find("FutureBackgroundManager");

        // Sets the appropriate time state
        if (defaultLevelTimeState == TimeState.Past)
        {
            futureManager.SetActive(false);
        }
        else
        {
            pastManager.SetActive(false);
        }
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
        mainCamera.GetComponent<CameraFollow>().Reset();
        player.GetComponent<Player>().Reset();
        platformManager.GetComponent<PlatformManager>().Reset();
        enemy.GetComponent<Enemy>().Reset();
        // Checks if masks should be switched
        if (currentLevelTimeState != defaultLevelTimeState)
        {
            SwapTimeMasks();
            ChangeTimeState();
        }

        // Looping through all the parallax layers and calling reset on them
        for (int i = 0; i < arrayOfParallax.Length; i++)
        {
            arrayOfParallax[i].GetComponent<ParallaxEffect>().Reset();
        }
    }

    // Changes time state
    public void ChangeTimeState()
    {
        switch (currentLevelTimeState)
        {
            case TimeState.Future:
                // Changes time state
                currentLevelTimeState = TimeState.Past;

                // Turns off future manager
                futureManager.SetActive(false);

                // Activates the new platforms
                pastManager.SetActive(true);
                break;

            case TimeState.Past:
                // Changes time state
                currentLevelTimeState = TimeState.Future;

                // Turns off past manager
                pastManager.SetActive(false);

                // Activates the new platforms
                futureManager.SetActive(true);
                break;
        }
    }

    // Swaps time state masks upon death if necessary
    public void SwapTimeMasks()
    {
        // Switches which platforms should be rendered inside and outside mask
        SpriteRenderer[] pastSrArray = pastManager.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer Sr in pastSrArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        TilemapRenderer[] futureTrArray = futureManager.GetComponentsInChildren<TilemapRenderer>();
        SpriteRenderer[] futureSrArray = futureManager.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer Sr in futureSrArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        foreach (TilemapRenderer Tr in futureTrArray)
        {
            if (Tr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Tr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Tr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        SpriteRenderer[] pastParallaxArray = pastParallax.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer Sr in pastParallaxArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        SpriteRenderer[] futureParallaxArray = futureParallax.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer Sr in futureParallaxArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }
    }
}
