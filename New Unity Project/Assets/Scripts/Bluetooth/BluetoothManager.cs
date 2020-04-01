using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{
    private BluetoothHelper helper;
    private string deviceName;

    public bool connectBT;
    public Text exT;
    public Text msgT;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = "ASTRA_K_BT_SERIAL";
        if(connectBT == true)
        {
            try
            {
                helper = BluetoothHelper.GetInstance(deviceName);

                helper.OnConnected += OnConnected;
                helper.OnConnectionFailed += OnConnFailed;

                helper.setTerminatorBasedStream("\n");

                if (helper.isDevicePaired()) // old isDeviceFound
                {
                    helper.Connect();
                }
            }
            catch (BluetoothHelper.BlueToothNotEnabledException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
            }
            catch (BluetoothHelper.BlueToothNotReadyException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
            }
            catch (BluetoothHelper.BlueToothNotSupportedException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
            }
            catch (BluetoothHelper.BlueToothPermissionNotGrantedException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
            }
        }
        
    }

    void OnConnected()
    {
        helper.StartListening();

        helper.SendData("Flash");
    }

    void OnConnFailed()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if(connectBT == true)
        {
            if (helper.Available)
            {
                string msg = helper.Read(); //msg recived from arduino

                msgT.text += " " + msg;
            }
        }
        
    }

    private void OnDestroy()
    {
        if (connectBT == true)
        {
            helper.Disconnect(); // old stopListening
        }
    }

    public BluetoothHelper GetBluetoothHelper()
    {
        return helper;
    }
}
