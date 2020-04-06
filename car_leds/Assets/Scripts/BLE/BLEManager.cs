using System;
using System.Collections;
using System.Collections.Generic;
using ArduinoBluetoothAPI;
using UnityEngine.UI;
using UnityEngine;

public class BLEManager : MonoBehaviour
{
    public DataReceived dataReceived;

    public Text msg;
    public Text exception;

    [Tooltip("To enable or disable the BLE conection. " +
        "Make sure it's true when build!")]
    public bool connectBLE;

    private BluetoothHelper bluetoothHelper;
    private float timer;

    void Start()
    {
        TryToConnect();
    }

    private void OnScanEnded(LinkedList<BluetoothDevice> devices){
        msg.text += "Found " + devices.Count + " ";

        if (devices.Count == 0){
            bluetoothHelper.ScanNearbyDevices();
            return;
        }
            
        try
        {
            bluetoothHelper.setDeviceName("ASTRA_K_LED_BLE");
            bluetoothHelper.Connect();

            msg.text += "Connecting ";
        }catch(Exception ex)
        {
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
        //SendData();
    }

    public void MySendData(string s){
        msg.text += "Sending ";

        //  UUID_RX -> to write on arduino
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        //  UART service UUID
        ch.setService("6E400001-B5A3-F393-E0A9-E50E24DCCA9E"); //this line is mandatory!!!
        bluetoothHelper.WriteCharacteristic(ch, s); //send as string
    }

    void Read(){
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");
        ch.setService("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");//this line is mandatory!!!

        bluetoothHelper.ReadCharacteristic(ch);
        //Debug.Log(System.Text.Encoding.ASCII.GetString(x));
    }

    private void TryToConnect()
    {
        msg.text = "App started ";
        msg.text = "Search for BLE connection: " + connectBLE + " ";
        if (connectBLE == true)
        {
            timer = 0;
            try
            {
                msg.text += "Trying... ";

                BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
                bluetoothHelper = BluetoothHelper.GetInstance("TEST");

                Debug.Log(bluetoothHelper.getDeviceName());
                msg.text += "Device name: " + bluetoothHelper.getDeviceName() + " ";

                bluetoothHelper.OnConnected += () => {

                    msg.text += "Connected ";
                };

                bluetoothHelper.OnConnectionFailed += () => {
                    msg.text += "Connection failed ";
                };

                bluetoothHelper.OnScanEnded += OnScanEnded;

                bluetoothHelper.OnServiceNotFound += (serviceName) =>
                {
                    msg.text += "Service name: " + serviceName + " ";
                };

                bluetoothHelper.OnCharacteristicNotFound += (serviceName, characteristicName) =>
                {
                    msg.text += "Characteristic name: " + characteristicName + " ";
                };

                // msg recieved
                bluetoothHelper.OnCharacteristicChanged += (value, characteristic) =>
                {
                    msg.text += "Characteristic name: " + characteristic.getName() + " "
                    + System.Text.Encoding.ASCII.GetString(value) + " ";

                    dataReceived.UpdateText(System.Text.Encoding.ASCII.GetString(value)); 
                };

                //  UART service UUID
                BluetoothHelperService service = new BluetoothHelperService("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
                // UUID_TX -> recive from arduino
                service.addCharacteristic(new BluetoothHelperCharacteristic("6E400003-B5A3-F393-E0A9-E50E24DCCA9E"));
                bluetoothHelper.Subscribe(service);
                bluetoothHelper.ScanNearbyDevices();

            }
            catch (Exception ex)
            {
                exception.text += ex + " ";
            }
        }
    }

    public BluetoothHelper GetBluetoothHelper()
    {
        return bluetoothHelper;
    }
}