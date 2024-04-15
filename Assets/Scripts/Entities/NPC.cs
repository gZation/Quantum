using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public string DialogText;
    public DialogWindow dialog;
    public SpriteRenderer image;
    public bool isImage;

    private void Start()
    {
        Color currColor = image.color;
        currColor.a = 0;
        image.color = currColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (isImage)
            {
                Color currColor = image.color;
                currColor.a = 1;
                image.color = currColor;
            }
            else
            {
                dialog.Show(DialogText);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (isImage)
            {
                Color currColor = image.color;
                currColor.a = 0;
                image.color = currColor;
            }
            else
            {
                dialog.Close();
            }
        }
    }

}
