using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{
    private BluetoothHelper helper;
    [SerializeField]
    [Tooltip("The name of the arduino")]
    private string deviceName = "ASTRA_K_BT_SERIAL"; //ASTRA_K_LED -> BLE , Arduino -> ASTRA_K_BT_SERIAL

    [Tooltip("To enable or disable the bluetooth conection. " +
        "Make sure it's true when build!")]
    public bool connectBT;

    public Text exT;
    public Text debbugerMsg;

    // Start is called before the first frame update
    void Start()
    {
        TryToConnect();
    }

    void OnConnected()
    {
        helper.StartListening();
    }

    void OnConnFailed()
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        if (connectBT == true)
        {
            helper.Disconnect(); // old stopListening
        }
    }

    private void TryToConnect()
    {
        // Get saved changes, or the default value
        deviceName = PlayerPrefs.GetString("DeviceName", deviceName);

        debbugerMsg.text = "Device name: " + deviceName + "; ";
        debbugerMsg.text += "Looking for a connection: " + connectBT + "; ";

        if (connectBT == true)
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

                debbugerMsg.text += "Bluetooth connected: yes!";
            }
            catch (BluetoothHelper.BlueToothNotEnabledException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
                debbugerMsg.text += "Bluetooth connected: No!";
            }
            catch (BluetoothHelper.BlueToothNotReadyException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
                debbugerMsg.text += "Bluetooth connected: No!";
            }
            catch (BluetoothHelper.BlueToothNotSupportedException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
                debbugerMsg.text += "Bluetooth connected: No!";
            }
            catch (BluetoothHelper.BlueToothPermissionNotGrantedException ex)
            {
                Debug.LogError("Excetion founded in Btmagaer: " + ex);
                exT.text += " " + ex;
                debbugerMsg.text += "Bluetooth connected: No!";
            }
        }
    }

    public BluetoothHelper GetBluetoothHelper()
    {
        return helper;
    }

    public void SetDeviceName(string s)
    {
        deviceName = s;


        //Save changes in Mobile
        PlayerPrefs.SetString("DeviceName", deviceName);

        // Turn on the looking for connection
        connectBT = true;
        TryToConnect();
    }
}
