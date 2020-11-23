using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Quit Application Called");
        Application.Quit();
    }
}
