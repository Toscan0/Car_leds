using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine;

public class BLE_UIInput : MonoBehaviour
{

    public BLEManager BLEManager;

    public void InputChanged(string s)
    {

        if (BLEManager.connectBLE == true)
        {
            BLEManager.MySendData(s);
        }

        Debug.Log(s);
    }
}
