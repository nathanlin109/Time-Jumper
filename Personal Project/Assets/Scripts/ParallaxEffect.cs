﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxEffect : MonoBehaviour
{
    // Fields
    private GameObject platformManager;
    private float platformManagerSpeed;
    private Vector3 initalPos;
    public float parallaxMultiplier;
    private float textureUnitSizeX;
    private float scaledTextureUnitSizeX;

    // Start is called before the first frame update
    void Start()
    {
        // Gets platform manager game object
        platformManager = GameObject.Find("PlatformManager");
        
        // Gets initial position of layer. Will be used to implement infinite scrolling
        initalPos = transform.position;

        // Calculates size of sprite in units (as opposed to pixels)
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        scaledTextureUnitSizeX = textureUnitSizeX * transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets the speed by which the platforms are moving
        platformManagerSpeed = platformManager.GetComponent<PlatformManager>().speed;
        
        // Moves the parallax layer by a factor of the platform manager speed to create a parallax effect
        transform.Translate(Vector2.left * (platformManagerSpeed * parallaxMultiplier) * Time.deltaTime);

        // Implements infinite scrolling by moving layer by texture size in units
        if (Mathf.Abs(transform.position.x - initalPos.x) >= scaledTextureUnitSizeX)
        {
            // Calculates offset to prevent jitter when resetting position
            float offsetX = (Mathf.Abs(transform.position.x - initalPos.x) % scaledTextureUnitSizeX);
            transform.position = new Vector3(transform.position.x + scaledTextureUnitSizeX + offsetX, transform.position.y, transform.position.z);
        }
    }

    // Called when player dies
    public void Reset()
    {
        // Resets layer's position to its starting position
        transform.position = initalPos;
    }
}
