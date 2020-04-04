using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public Button[] buttons;

    public void ResetButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<ButtonProperties>().ResetButton();
        }
    }
}
