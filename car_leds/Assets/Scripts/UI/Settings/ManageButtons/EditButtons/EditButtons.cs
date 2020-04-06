using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class EditButtons : MonoBehaviour
{
    public Button[] buttons;

    public InputField inputID;
    public InputField inputText;

    private int buttonToEdit = 0;

    private void Awake()
    {
        PopulateEditUI();
    }

    public void ButtonToEdit(int val)
    {
        buttonToEdit = val;

        PopulateEditUI();
    }

    private void PopulateEditUI()
    {
        Button buttonChosen = buttons[buttonToEdit];

        ButtonProperties bp = buttonChosen.GetComponent<ButtonProperties>();

        inputID.text = bp.GetID();
        inputText.text = bp.GetMsg();

    }

    public void SetMsg(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<ButtonProperties>().SetMsg(s);
    }

    public void SetID(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<ButtonProperties>().SetID(s);
    }
}
