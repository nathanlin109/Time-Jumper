using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // Fields
    public float inputDelayTime = 1f;
    private float inputDelayTimer;
    private GameObject player;
    public GameObject pauseCanvas;
    public GameObject pauseButton;
    public GameObject vampire;
    public GameObject wizard;
    public GameObject mother;
    public GameObject blood;
    public GameObject buttonPromptText;
    public GameObject dialogueBackground;
    public GameObject cutscene1_1Text;
    public GameObject cutscene1_2Text;
    public GameObject cutscene1_3Text;
    public GameObject cutscene1_4Text;
    public GameObject cutscene2_1Text;
    public GameObject cutscene2_2Text;
    public GameObject cutscene2_3Text;
    public GameObject cutscene2_4Text;
    public GameObject cutscene2_5Text;
    public GameObject cutscene2_6Text;
    public GameObject cutscene3Text;
    public GameObject cutscene4_1Text;
    public GameObject cutscene4_2Text;
    public GameObject cutscene4_3Text;
    public GameObject cutscene4_4Text;
    public GameObject cutscene4_5Text;
    public GameObject cutscene5_1Text;
    public GameObject cutscene5_2Text;
    public GameObject cutscene5_3Text;
    public GameObject cutscene5_4Text;
    public GameObject pickupTrapText;
    public GameObject pickupObjectiveItemText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        inputDelayTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UIKeyboardInputs();
    }

    // Keyboard UI Inputs
    void UIKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GetComponent<SceneMan>().gameState != GameState.Death && GetComponent<SceneMan>().gameState != GameState.Win)
            {
                // Pause
                if (GetComponent<SceneMan>().gameState != GameState.Pause)
                {
                    GameObject.Find("UIManager").GetComponent<UIManager>().Pause();
                }
                // Unpause
                else
                {
                    GameObject.Find("UIManager").GetComponent<UIManager>().UnPause();
                }
            }
        }
    }
}
