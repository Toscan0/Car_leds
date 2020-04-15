using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine;

public class BT_UIInput : MonoBehaviour
{

    public BluetoothManager btManager;

    private BluetoothHelper helper;

    public void InputChanged(string s)
    {

        if (btManager.connectBT == true)
        {
            helper = btManager.GetBluetoothHelper(); //refact this to start
            helper.SendData(s);
        }

        Debug.Log(s);
    }
}
