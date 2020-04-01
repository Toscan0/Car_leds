using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonProperties : MonoBehaviour
{
    public Text buttonText;

    [SerializeField] private string ID; //THe ID is what is showed
    [SerializeField] private string text;

    // Start is called before the first frame update
    void Start()
    {
        buttonText.text = ID;
    }

    public string GetText()
    {
        return text;
    }

    public void SetText(string s)
    {
        text = s;
    }

    public string GetID()
    {
        return ID;
    }

    public void SetID(string s)
    {
        ID = s;

        buttonText.text = ID;
    }
}
