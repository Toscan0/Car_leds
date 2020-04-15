using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class BT_UISlider : MonoBehaviour
{
    public BluetoothManager btManager;
    public Text text;

    private BluetoothHelper helper;

    private void Start()
    {
        text.text = this.gameObject.GetComponent<Slider>().value.ToString();
    }

    public void SliderChanged(float n)
    {
        string value = n.ToString();

        if (btManager.connectBT == true)
        {
            helper = btManager.GetBluetoothHelper(); //refact this to start
            helper.SendData(value);
        }
        Debug.Log(n);
        text.text = value;
    }
}
