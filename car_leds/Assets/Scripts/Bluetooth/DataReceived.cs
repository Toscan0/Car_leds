using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class DataReceived : MonoBehaviour
{    
    private Text text;

    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    public void UpdateText(string s)
    {
        text.text += s + " ";
    }
}
