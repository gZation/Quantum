using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogWindow : MonoBehaviour
{
    public TMP_Text text;
    private string currentText;

    CanvasGroup group;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    public void Show(string text)
    {
        group.alpha = 1;
        currentText = text;
        StartCoroutine(DisplayText());
    }

    public void Close()
    {
        StopAllCoroutines();
        group.alpha = 0;
    }

    private IEnumerator DisplayText()
    {
        text.text = "";

        foreach(char c in currentText.ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
