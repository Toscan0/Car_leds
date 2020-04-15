using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class BLE_UIButton : MonoBehaviour
{
    public Text buttonText;
    public BLEManager BLEManager;

    [SerializeField] private string ID; //THe ID is what is showed to the user
    [SerializeField] private string msg;

    // Start is called before the first frame update
    void Start()
    {
        // Get saved changes, or the default value
        buttonText.text = PlayerPrefs.GetString(gameObject.name + "ID", ID);
    }

    public string GetMsg()
    {
        return msg;
    }

    public void SetMsg(string s)
    {
        msg = s;

        //Save changes in Mobile
        PlayerPrefs.SetString(gameObject.name + "MSG", msg);
    }

    public string GetID()
    {
        return ID;
    }

    public void SetID(string s)
    {
        ID = s;

        buttonText.text = ID;

        //Save changes in Mobile
        PlayerPrefs.SetString(gameObject.name + "ID", ID);
    }

    public void ButtonClick()
    {
        if(BLEManager.connectBLE == true)
        {
            BLEManager.MySendData(msg);
        }

        Debug.Log(msg);
    }


}
