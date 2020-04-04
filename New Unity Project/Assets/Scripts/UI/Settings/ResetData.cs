using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    private bool enabled = false;

    public BluetoothManager bluetoothManager;
    public ButtonsManager buttonsManager;

    public void ResetButtonClicked()
    {
        enabled = !enabled;

        this.gameObject.SetActive(enabled);
    }

    public void Reset()
    {
        //Reset all data saved in the device
        PlayerPrefs.DeleteAll();

        //Reset the current data
        bluetoothManager.ResetDeviceName();
        buttonsManager.ResetButtons();

        enabled = false;
        this.gameObject.SetActive(enabled);
    }

    public void NoButtonClicked()
    {
        enabled = false;
        this.gameObject.SetActive(enabled);
    }
}
