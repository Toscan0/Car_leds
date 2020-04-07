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

    private String queue;
    private bool awatingMsg;

    // UART service UUID
    private const string UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
    // UUID_RX -> recive from arduino
    private const string UUID_RX = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
    // UUID_TX -> to write on arduino
    private const string UUID_TX = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
    

    void Start()
    {
        queue = "";
        awatingMsg = false;
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
    
    public void MySendData(string s){ 
        // need to concatunate the string, because c# reoder strings to optimize things

        msg.text += "Trying to send ";

        queue += s + "|";

        sendNext();
        
        //BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic(UUID_RX);
        //ch.setService(UUID); //this line is mandatory!!!

        //bluetoothHelper.WriteCharacteristic(ch, s); //send as string
    }


    private void sendNext()
    {

        if (awatingMsg || bluetoothHelper == null)
            return;

        Debug.Log(queue);
       
        int x = queue.IndexOf('|');
        if(x <= 0) 
        {
            queue = "";
            return;
        }
        string msgg = queue.Substring(0, x);
        queue = queue.Substring(x + 1);
        awatingMsg = true;

        msg.text += "Sending  " + msgg; 

        bluetoothHelper.SendData(msgg);
    }

    private void BluetoothHelper_OnDataReceived()
    {
        awatingMsg = false;
        //msg.text += "Characteristic name: " + characteristic.getName() + " "
        //           + System.Text.Encoding.ASCII.GetString(value) + " ";

        dataReceived.UpdateText(bluetoothHelper.Read());

        // send the next message only when the previous message response is received.
        sendNext();
        
    }

    private void TryToConnect()
    {
        msg.text = "App started ";
        msg.text = "Search for BLE connection: " + connectBLE + " ";

        if (connectBLE == true)
        {
            try
            {
                msg.text += "Trying... ";

                awatingMsg = false;

                BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
                bluetoothHelper = BluetoothHelper.GetInstance("TEST");

                bluetoothHelper.setTerminatorBasedStream("\n");

                Debug.Log(bluetoothHelper.getDeviceName());
                msg.text += "Device name: " + bluetoothHelper.getDeviceName() + " ";

                bluetoothHelper.OnConnected += () => {

                    msg.text += "Connected ";
                    awatingMsg = false;
                    bluetoothHelper.StartListening();
                };

                bluetoothHelper.OnConnectionFailed += () => {
                    msg.text += "Connection failed ";
                };

                bluetoothHelper.OnScanEnded += OnScanEnded;

                bluetoothHelper.OnDataReceived += BluetoothHelper_OnDataReceived;

                BluetoothHelperCharacteristic txC = new BluetoothHelperCharacteristic(UUID_TX);
                txC.setService(UUID);

                BluetoothHelperCharacteristic rxC = new BluetoothHelperCharacteristic(UUID_RX);
                rxC.setService(UUID);


                bluetoothHelper.setRxCharacteristic(rxC);
                bluetoothHelper.setTxCharacteristic(txC);

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