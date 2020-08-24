using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathTransition : MonoBehaviour
{
    // Fields
    public Image circleImage;
    public float transitionSpeed = 2f;
    public GameObject mainCamera;
    public GameObject player;

    // Closes vs opens circle
    private bool shouldOpen;
    private bool startedCloseTransition;
    private bool startedOpenTransition;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        circleImage = GetComponent<Image>();
        shouldOpen = false;
        startedCloseTransition = true;
        startedOpenTransition = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Moves death transition circle to be on player
        /*transform.position = new Vector3(player.GetComponent<Player>().transform.position.x,
            player.GetComponent<Player>().transform.position.y,
           transform.position.z);*/

        // Starts the circle transition after the camera has stopped moving after death
        if (mainCamera.GetComponent<CameraScript>().stoppedMovingCamera)
        {
            startedCloseTransition = false;
        }

        TransitionDeathCircle();
    }

    private void TransitionDeathCircle()
    {
        // Checks if circle should open or close
        if (shouldOpen)
        {
            // Checks if the transition has already started
            if (!startedOpenTransition)
            {
                // Gradually closes circle death transition (move to 1.1 just to be safe b/c 1 might have artifacts)
                circleImage.material.SetFloat("_Cutoff",
                    Mathf.MoveTowards(circleImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.deltaTime));
            }

            // Checks if circle is fully opened
            if (circleImage.material.GetFloat("_Cutoff") == 1.1f)
            {
                // Sets circle up to close upon next death
                startedOpenTransition = false;
                shouldOpen = false;
            }
        }
        else if (!shouldOpen)
        {
            // Checks if the transition has already started
            if (!startedCloseTransition)
            {
                // Gradually opens circle death transition
                circleImage.material.SetFloat("_Cutoff",
                    Mathf.MoveTowards(circleImage.material.GetFloat("_Cutoff"), -.1f - circleImage.material.GetFloat("_Smoothing"),
                    transitionSpeed * Time.deltaTime));
            }

            // Checks if circle is fully closed
            if (circleImage.material.GetFloat("_Cutoff") == -.1f - circleImage.material.GetFloat("_Smoothing"))
            {
                Debug.Log("FINISHED CLOSING CIRCLE");

                // Opens circle after fully closed
                startedOpenTransition = false;
                startedCloseTransition = true;
                shouldOpen = true;

                // Resets the level
                GameObject.Find("SceneManager").GetComponent<SceneManager>().ResetLevel();
            }
        }
    }
}
