using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskUI : MonoBehaviour
{
    // Fields
    public bool isText;

    // Start is called before the first frame update
    void Start()
    {
        if (isText == false)
        {
            Material newMat = Instantiate(GetComponent<Image>().material);
            newMat.renderQueue = 3002;
            GetComponent<Image>().material = newMat;
        }
        else
        {
            Material newMat = Instantiate(GetComponent<TMP_Text>().fontMaterial);
            newMat.renderQueue = 3002;
            GetComponent<TMP_Text>().fontMaterial = newMat;
        }
    }
}
