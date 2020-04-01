using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class ButtonProperties : MonoBehaviour
{
    public Text buttonText;
    public BluetoothManager btManager;

    [SerializeField] private string ID; //THe ID is what is showed to the user
    [SerializeField] private string msg;

    private BluetoothHelper helper;

    // Start is called before the first frame update
    void Start()
    {
        buttonText.text = ID;
    }

    public string GetMsg()
    {
        return msg;
    }

    public void SetMsg(string s)
    {
        msg = s;
    }

    public string GetID()
    {
        return ID;
    }

    public void SetID(string s)
    {
        ID = s;

        buttonText.text = ID;
    }

    public void ButtonClick()
    {
        if(btManager.connectBT == true)
        {
            helper = btManager.GetBluetoothHelper(); //refact this to start
            helper.SendData(msg);
        }
        Debug.Log(msg);
    }
}
