using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BT_EditButtons : MonoBehaviour
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

        BT_UIButtons bp = buttonChosen.GetComponent<BT_UIButtons>();

        inputID.text = bp.GetID();
        inputText.text = bp.GetMsg();

    }

    public void SetMsg(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<BT_UIButtons>().SetMsg(s);
    }

    public void SetID(string s)
    {
        Button buttonChosen = buttons[buttonToEdit];

        buttonChosen.GetComponent<BT_UIButtons>().SetID(s);
    }
}
