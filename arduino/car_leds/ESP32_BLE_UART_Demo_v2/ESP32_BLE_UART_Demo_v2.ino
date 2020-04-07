/*
    Video: https://www.youtube.com/watch?v=oCMOYS71NIU
    Based on Neil Kolban example for IDF: https://github.com/nkolban/esp32-snippets/blob/master/cpp_utils/tests/BLE%20Tests/SampleNotify.cpp
    Ported to Arduino ESP32 by Evandro Copercini

   Create a BLE server that, once we receive a connection, will send periodic notifications.
   The service advertises itself as: 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
   Has a characteristic of: 6E400002-B5A3-F393-E0A9-E50E24DCCA9E - used for receiving data with "WRITE" 
   Has a characteristic of: 6E400003-B5A3-F393-E0A9-E50E24DCCA9E - used to send data with  "NOTIFY"

   The design of creating the BLE server is:
   1. Create a BLE Server
   2. Create a BLE Service
   3. Create a BLE Characteristic on the Service
   4. Create a BLE Descriptor on the characteristic
   5. Start the service.
   6. Start advertising.

   In this example rxValue is the data received (only accessible inside that function).
   And txValue is the data to be sent, in this example just a byte incremented every second. 
*/
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>

BLEServer *pServer = NULL;
BLECharacteristic * pTxCharacteristic;
bool deviceConnected = false;
bool oldDeviceConnected = false;
uint8_t txValue[50] = {0};
String inString = "";
const int LED = 22; // Could be different depending on the dev board. I used the DOIT ESP32 dev board.


// See the following for generating UUIDs:
// https://www.uuidgenerator.net/

#define SERVICE_UUID           "6E400001-B5A3-F393-E0A9-E50E24DCCA9E" // UART service UUID
#define CHARACTERISTIC_UUID_RX "6E400002-B5A3-F393-E0A9-E50E24DCCA9E" // receive characteristic from unity  arduino -> unity
#define CHARACTERISTIC_UUID_TX "6E400003-B5A3-F393-E0A9-E50E24DCCA9E" //write characteristic from unity unity -> arduino


class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
      Serial.println("CLIENT CONECTED");
    };

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
      Serial.println("CLIENT DISCONECTED");
    }
};

class MyCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
     std::string rxValue = pCharacteristic->getValue();
     txValue[0]='o';
      txValue[1]='l';
      txValue[2]='a';
      
      if (rxValue.length() > 0) {
        Serial.println("***");
        Serial.print("Received Value: ");
        inString="";
        for (int i = 0; i < rxValue.length(); i++){
          Serial.print(rxValue[i]);
          txValue[i+3]=rxValue[i];
          inString += (char)rxValue[i];
        }
        Serial.println();

        int pwm_led=inString.toInt();
        Serial.print("pwm");
        Serial.println(pwm_led);
        ledcWrite(1, pwm_led);
        Serial.println("***\n\n");

        pTxCharacteristic->setValue(txValue,rxValue.length()+3);
        pTxCharacteristic->notify();
//        txValue++;
      }
    }
};


void setup() {
  Serial.begin(115200);
  
  ledcAttachPin(LED, 1);
  ledcSetup(1, 300, 8); // 12 kHz PWM, 8-bit resolution
  ledcWrite(1, 255);//start at full bright

  // Create the BLE Device
  BLEDevice::init("ASTRA_K_LED_BLE");

  // Create the BLE Server
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());

  // Create the BLE Service
  BLEService *pService = pServer->createService(SERVICE_UUID);

  // Create a BLE Characteristic
  pTxCharacteristic = pService->createCharacteristic(
										CHARACTERISTIC_UUID_RX,
										BLECharacteristic::PROPERTY_NOTIFY
									);
                      
  pTxCharacteristic->addDescriptor(new BLE2902());

  BLECharacteristic * pRxCharacteristic = pService->createCharacteristic(
											 CHARACTERISTIC_UUID_TX,
											BLECharacteristic::PROPERTY_WRITE
										);

  pRxCharacteristic->setCallbacks(new MyCallbacks());

  // Start the service
  pService->start();

  // Start advertising
  pServer->getAdvertising()->start();
  Serial.println("BLE: Waiting a client connection to notify...");
}

void loop() {

    if (deviceConnected) {

		delay(10); // bluetooth stack will go into congestion, if too many packets are sent
	}

    // disconnecting
    if (!deviceConnected && oldDeviceConnected) {
        delay(500); // give the bluetooth stack the chance to get things ready
        pServer->startAdvertising(); // restart advertising
        Serial.println("start advertising");
        oldDeviceConnected = deviceConnected;
    }
    // connecting
    if (deviceConnected && !oldDeviceConnected) {
		// do stuff here on connecting
        oldDeviceConnected = deviceConnected;
    }
}
