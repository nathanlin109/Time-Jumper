using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathTransition : MonoBehaviour
{
    // Fields
    public Image circleImage;
    public float transitionSpeed = 2f;

    private bool shouldReveal;

    // Start is called before the first frame update
    void Start()
    {
        circleImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
