using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class BLEManager : MonoBehaviour
{
    public Text msg;
    public Text exception;

    private BluetoothHelper bluetoothHelper;
    private float timer;

    void Start()
    {
        TryToConnect();
    }

    private void OnScanEnded(LinkedList<BluetoothDevice> devices){
        Debug.Log("FOund " + devices.Count);
        if(devices.Count == 0){
            bluetoothHelper.ScanNearbyDevices();
            return;
        }
            
        try
        {
            bluetoothHelper.setDeviceName("ASTRA_K_LED_BLE");
            // bluetoothHelper.setDeviceName("HC-08");
            bluetoothHelper.Connect();
            Debug.Log("Connecting");
        }catch(Exception ex)
        {
            Debug.Log(ex.Message);
            exception.text += ex + " ";
        }

    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

    void Update(){
        if(bluetoothHelper == null)
            return;
        if(!bluetoothHelper.isConnected())
            return;
        timer += Time.deltaTime;

        if(timer < 5)
            return;
        timer = 0;
        SendData();
    }

    void SendData(){
        // Debug.Log("Sending");
        // BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("FFE1");
        // ch.setService("FFE0"); //this line is mandatory!!!
        // bluetoothHelper.WriteCharacteristic(ch, new byte[]{0x44, 0x55, 0xff});

        Debug.Log("Sending");
        msg.text += " --- ";
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        ch.setService("6E400001-B5A3-F393-E0A9-E50E24DCCA9E"); //this line is mandatory!!!
        bluetoothHelper.WriteCharacteristic(ch, "128"); //string: 10001000 is this binary? no, as string.

        msg.text += " SENDIG ";
    }

    void Read(){
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("2A24");
        ch.setService("180A");//this line is mandatory!!!

        bluetoothHelper.ReadCharacteristic(ch);
        //Debug.Log(System.Text.Encoding.ASCII.GetString(x));
    }

    private void TryToConnect()
    {
        timer = 0;
        try
        {
            Debug.Log("HI");
            msg.text = "HI ";
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance("TEST");
            Debug.Log(bluetoothHelper.getDeviceName());
            msg.text += bluetoothHelper.getDeviceName() + " ";
            bluetoothHelper.OnConnected += () => {
                Debug.Log("Connected");
                msg.text += "Connected ";
                SendData();
            };
            bluetoothHelper.OnConnectionFailed += () => {
                Debug.Log("Connection failed");
                msg.text += "Connection failed ";
            };
            bluetoothHelper.OnScanEnded += OnScanEnded;
            bluetoothHelper.OnServiceNotFound += (serviceName) =>
            {
                Debug.Log(serviceName);
                msg.text += serviceName + " ";
            };
            bluetoothHelper.OnCharacteristicNotFound += (serviceName, characteristicName) =>
            {
                Debug.Log(characteristicName);
                msg.text += characteristicName + " ";
            };
            bluetoothHelper.OnCharacteristicChanged += (value, characteristic) =>
            {
                Debug.Log(characteristic.getName());
                Debug.Log(System.Text.Encoding.ASCII.GetString(value));
                msg.text += characteristic.getName() + " " + System.Text.Encoding.ASCII.GetString(value) + " ";
            };

            // BluetoothHelperService service = new BluetoothHelperService("FFE0");
            // service.addCharacteristic(new BluetoothHelperCharacteristic("FFE1"));
            // BluetoothHelperService service2 = new BluetoothHelperService("180A");
            // service.addCharacteristic(new BluetoothHelperCharacteristic("2A24"));
            // bluetoothHelper.Subscribe(service);
            // bluetoothHelper.Subscribe(service2);
            // bluetoothHelper.ScanNearbyDevices();

            BluetoothHelperService service = new BluetoothHelperService("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
            service.addCharacteristic(new BluetoothHelperCharacteristic("6E400003-B5A3-F393-E0A9-E50E24DCCA9E"));
            bluetoothHelper.Subscribe(service);
            //bluetoothHelper.Subscribe(service2);
            bluetoothHelper.ScanNearbyDevices();

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            exception.text += ex + " ";
        }
    }
}