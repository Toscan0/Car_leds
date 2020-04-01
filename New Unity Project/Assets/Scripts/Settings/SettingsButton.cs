using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public GameObject UI;
    public GameObject settingsUI;

    private bool UIEnabled = true;

    public void ButtonClicked()
    {
        UIEnabled = !UIEnabled;


        UI.SetActive(UIEnabled);
        settingsUI.SetActive(!UIEnabled);
    }
}
