using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ParallaxEffect : MonoBehaviour
{
    // Fields
    private Vector3 startPos;
    public GameObject platformManager;
    public float parallaxFactor;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.left * (platformManager.GetComponent<PlatformManager>().speed * parallaxFactor) * Time.deltaTime);
    }

    public void Reset()
    {
        transform.position = startPos;
    }
}
