using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    // Fields
    private GameObject platformManager;
    private float platformManagerSpeed;
    private Vector3 initalPos;
    public float parallaxMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        platformManager = GameObject.Find("PlatformManager");
        
        initalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        platformManagerSpeed = platformManager.GetComponent<PlatformManager>().speed;
        transform.Translate(Vector2.left * (platformManagerSpeed * parallaxMultiplier) * Time.deltaTime);
    }

    public void Reset()
    {
        transform.position = initalPos;
    }
}
