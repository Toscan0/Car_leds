using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class DataReceived : MonoBehaviour
{
    public BluetoothManager btManager;
    
    private BluetoothHelper helper;
    private string outMsg = "";
    private Text text;

    void Start()
    {
        helper = btManager.GetBluetoothHelper();
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (btManager.connectBT)
        {
            outMsg = helper.Read();
            UpdateText();
        }
    }

    private void UpdateText()
    {
        text.text += outMsg + " ";
    }
}
