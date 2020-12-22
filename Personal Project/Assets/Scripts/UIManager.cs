using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    // Fields
    public GameObject mainCanvas;
    public GameObject pauseCanvas;
    public GameObject pauseButton;
    public GameObject creditsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        // Plays menu theme
        /*if ((SceneManager.GetActiveScene().name == "MainMenu" ||
            SceneManager.GetActiveScene().name == "Win") &&
            GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.isPlaying == false)
        {
            GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.Play();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" &&
            SceneManager.GetActiveScene().name != "LevelCompleteScreen")
        {
            UIKeyboardInputs();
        }
    }

    public void RunLevel1Scene()
    {
        PlayClickSound();
        /*if (GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.isPlaying == true)
        {
            GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.Stop();
        }*/
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void RunMenuScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        /*Sound backgroundSound = Array.Find(GameObject.Find("AudioManager").GetComponent<AudioMan>().sounds, sound => sound.name == "background-sounds");
        if (backgroundSound != null && backgroundSound.source.isPlaying == true)
        {
            backgroundSound.source.Stop();
        }*/
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void OpenCredits()
    {
        PlayClickSound();
        GameObject.Find("MainMenuCanvas/CreditsButton").GetComponent<ButtonHover>().spriteIndex = 0;
        GameObject.Find("MainMenuCanvas/CreditsButton").GetComponent<Image>().sprite = GameObject.Find("MainMenuCanvas/CreditsButton").GetComponent<ButtonHover>().buttonSprites[0];
        GameObject.Find("MainMenuCanvas/CreditsButton").GetComponentInChildren<Text>().color = GameObject.Find("MainMenuCanvas/CreditsButton").GetComponent<ButtonHover>().buttonColors[0];
        mainCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void CloseCredits()
    {
        PlayClickSound();
        if (creditsCanvas.activeSelf)
        {
            GameObject.Find("CreditsCanvas/BackButton").GetComponent<ButtonHover>().spriteIndex = 0;
            GameObject.Find("CreditsCanvas/BackButton").GetComponent<Image>().sprite = GameObject.Find("CreditsCanvas/BackButton").GetComponent<ButtonHover>().buttonSprites[0];
            GameObject.Find("CreditsCanvas/BackButton").GetComponentInChildren<Text>().color = GameObject.Find("CreditsCanvas/BackButton").GetComponent<ButtonHover>().buttonColors[0];
            creditsCanvas.SetActive(false);
        }
        mainCanvas.SetActive(true);
    }

    public void Pause()
    {
        PlayClickSound();
        GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;

        if (pauseCanvas.activeSelf == false)
        {
            pauseCanvas.SetActive(true);
        }
        if (pauseButton.activeSelf == true)
        {
            pauseButton.SetActive(false);
        }
    }

    public void UnPause()
    {
        PlayClickSound();
        GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;

        if (pauseCanvas.activeSelf == true)
        {
            pauseCanvas.SetActive(false);
        }
        if (pauseButton.activeSelf == false)
        {
            pauseButton.SetActive(true);
        }
    }

    private void PlayClickSound()
    {
        FindObjectOfType<AudioMan>().Play("button-sound");
    }

    // Keyboard UI Inputs
    void UIKeyboardInputs()
    {
        // Pauses the game when user presses escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused == false)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }
}
