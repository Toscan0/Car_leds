using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine;

public class BTmanager : MonoBehaviour
{
    private BluetoothHelper helper;

    private string deviceName;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = "";
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
        catch (BluetoothHelper.BlueToothNotEnabledException ex) { Debug.LogError("Excetion founded in Btmagaer: " + ex); }
        catch (BluetoothHelper.BlueToothNotReadyException ex) { Debug.LogError("Excetion founded in Btmagaer: " + ex); }
        catch (BluetoothHelper.BlueToothNotSupportedException ex) { Debug.LogError("Excetion founded in Btmagaer: " + ex); }
        catch (BluetoothHelper.BlueToothPermissionNotGrantedException ex) { Debug.LogError("Excetion founded in Btmagaer: " + ex); }
    }

    void OnConnected()
    {
        helper.StartListening();

        helper.SendData("Hi arduino!!");
    }

    void OnConnFailed()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if (helper.Available)
        {
            string msg = helper.Read();
        }
    }

    private void OnDestroy()
    {
        helper.Disconnect(); // old stopListening
    }
}
