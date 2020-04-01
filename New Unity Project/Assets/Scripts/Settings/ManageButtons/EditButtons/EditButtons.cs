using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class EditButtons : MonoBehaviour
{
    public Button[] buttons;

    public Text inputID;
    public Text inputText;

    private int buttonToEdit = 0;

    private void Awake()
    {
        PopulateEditUI();
    }

    public void ButtonToEdit(int val)
    {
        buttonToEdit = val;

        //inputID.text = "";
        //inputText.text = "";

        PopulateEditUI();
    }

    private void PopulateEditUI()
    {
        Button buttonChosen = buttons[buttonToEdit];

        ButtonProperties bp = buttonChosen.GetComponent<ButtonProperties>();

        /*
        * TODO: Not Working
        */
        string id = bp.GetID();
        string text = bp.GetText();

        inputID.text = id;
        inputText.text = "....";

        Debug.Log(id + " ** " + text);
    }

    public void SetText(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<ButtonProperties>().SetText(s);
    }

    public void SetID(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<ButtonProperties>().SetID(s);
    }
}
