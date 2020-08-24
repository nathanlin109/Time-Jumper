using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // For resetting camera
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}
