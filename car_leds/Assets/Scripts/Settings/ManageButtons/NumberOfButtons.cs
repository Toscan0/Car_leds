using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberOfButtons : MonoBehaviour
{
    public Button[] buttons;
    public Dropdown buttonsDropdown;

    private int numberOfButtons = 6;

    void Awake()
    {
        buttonsDropdown.value = numberOfButtons - 1;

        ManageNumberOfButtons();
    }

    public void NumberOfButtonsEdited(int val)
    {
        numberOfButtons = val + 1;

        ManageNumberOfButtons();
    }

    private void ManageNumberOfButtons()
    {
        for (int i = 0; i < numberOfButtons; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }

        for (int i = numberOfButtons; i < 9; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }
}
