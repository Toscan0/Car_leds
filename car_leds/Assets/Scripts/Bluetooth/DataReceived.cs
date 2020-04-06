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
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (btManager.connectBT == true)
        {
            //Read from bluetooth
            helper = btManager.GetBluetoothHelper();
            outMsg = helper.Read();

            ParseMsg();
        }
    }

    private void ParseMsg()
    {
        int indexWhiteSpace;

        outMsg = outMsg.Trim();

        if (outMsg != "")
        {
            //first white space fouded, that will be betwen two words, thx to the Trim function
            indexWhiteSpace = outMsg.IndexOf(" "); 

            outMsg = outMsg.Remove(0, indexWhiteSpace);

            UpdateText();
        }
    }

    private void UpdateText()
    {
        text.text += outMsg + " ";
    }
}
