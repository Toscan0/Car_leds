using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class UISlider : MonoBehaviour
{
    public BLEManager BLEManager;
    public Text text;


    private void Start()
    {
        text.text = this.gameObject.GetComponent<Slider>().value.ToString();
    }

    public void SliderChanged(float n)
    {
        string value = n.ToString();

        if (BLEManager.connectBLE == true)
        {
            BLEManager.MySendData(value);
        }

        //Debug.Log(n);
        text.text = value;
    }
}
