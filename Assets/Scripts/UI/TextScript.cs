using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    [TextArea]
    public string[] texts = new string[3];
    
    //vars from inspector
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        SetLanguage(Brains.MenuBrain.language);
    }

    private void Update()
    {
        
    }

    public void SetLanguage(int numb)
    {
        text.text = texts[numb];
    }

    public void UpdateText()
    {
        SetLanguage(Brains.MenuBrain.language);
    }

    public void UpdateTextWithVar(int v)
    {
        UpdateText();
        text.text += "" + v;
    }

    public void UpdateTextWithVar(float v)
    {
        UpdateText();
        text.text += "" + v;
    }

    public void UpdateTextWithVar(string v)
    {
        UpdateText();
        text.text += v;
    }
}
