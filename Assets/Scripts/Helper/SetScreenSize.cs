using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenSize : MonoBehaviour
{
    public int width;
    public int height;
    void Start()
    {
        Screen.SetResolution(width, height, false);
    }
}
