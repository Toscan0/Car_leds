/**
FW version: V0_1
date: 27/02/2020
*/




#include "BluetoothSerial.h" 
#include "EEPROM.h"
#define EEPROM_SIZE 1024

 /**FOR OTA DOWNLOAD and WIFI report*/
#include <WiFi.h>
#include <WiFiClient.h>
#include <WebServer.h>
#include <ESPmDNS.h>
#include <Update.h>

const char FWversion[]= "1";
const char* host = "esp32";
char ota_ssid[20]="";
char ota_pwd[20]="";
String otaSSID="";
String otaPWD="";


WebServer server(80);
/********************/


/** Select DEBUG Options:  */  
#define serialConsole

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;  
String bt_buff="";
bool FlagBTclientConnected=false;
bool FlagBTclientConnected_led_notification=false;
bool flag_1st_connection=true;
//*******************/

/**PINOUT ASSIGMENT*/
#define LED 22 //SCOOTER SERIAL M365RX->22 M365TX->23
/***********************/

/**TIMER INSTANCES*/
unsigned long int esp32StartTimer=0;
//*****************/

/**whatchDog timer instaces*/
const int wdtTimeout = 120000;  //time in ms to trigger the watchdog
hw_timer_t *timer = NULL;
short int FlagWatchdogReboot=0;
/*********************/

/**INTERNAL TEMP SENSOR*/
#ifdef __cplusplus
extern "C" {
#endif
uint8_t temprature_sens_read();
#ifdef __cplusplus
}
#endif
uint8_t temprature_sens_read();
/*****************************/

/**FLOW VARIABLES */
bool flag_flow_on=false;
bool flag_flash_on=false;
int flow_min_bright=0;
int flow_max_bright=0;
int flow_delay_ms=0;
int	flash_number=0;
int	flash_delay_ms=0;
int pwm_led=255;
/****************************/


/******************************************  OTA WEB SERVER  *****************************************************/
/*
 * Login page
 */

const char* loginIndex = 
 "<form name='loginForm'>"
    "<table width='20%' bgcolor='3769C2' align='center'>"
        "<tr>"
            "<td colspan=2>"
                "<center><font size=4><b>ASTRA K LED CONTROLLER FW UPDATE</b></font></center>"
                "<br>"
            "</td>"
            "<br>"
            "<br>"
        "</tr>"
        "<td>Username:</td>"
        "<td><input type='text' size=25 name='userid'><br></td>"
        "</tr>"
        "<br>"
        "<br>"
        "<tr>"
            "<td>Password:</td>"
            "<td><input type='Password' size=25 name='pwd'><br></td>"
            "<br>"
            "<br>"
        "</tr>"
        "<tr>"
            "<td><input type='submit' onclick='check(this.form)' value='Login'></td>"
        "</tr>"
    "</table>"
"</form>"
"<script>"
    "function check(form)"
    "{"
    "if(form.userid.value=='admin' && form.pwd.value=='admin')"
    "{"
    "window.open('/serverIndex')"
    "}"
    "else"
    "{"
    " alert('Error Password or Username')/*displays error message*/"
    "}"
    "}"
"</script>";
 
/*
 * Server Index Page
 */
 
const char* serverIndex = 
"<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js'></script>"
"<form method='POST' action='#' enctype='multipart/form-data' id='upload_form'>"
   "<input type='file' name='update'>"
        "<input type='submit' value='Update'>"
    "</form>"
 "<div id='prg'>progress: 0%</div>"
 "<script>"
  "$('form').submit(function(e){"
  "e.preventDefault();"
  "var form = $('#upload_form')[0];"
  "var data = new FormData(form);"
  " $.ajax({"
  "url: '/update',"
  "type: 'POST',"
  "data: data,"
  "contentType: false,"
  "processData:false,"
  "xhr: function() {"
  "var xhr = new window.XMLHttpRequest();"
  "xhr.upload.addEventListener('progress', function(evt) {"
  "if (evt.lengthComputable) {"
  "var per = evt.loaded / evt.total;"
  "$('#prg').html('progress: ' + Math.round(per*100) + '%');"
  "}"
  "}, false);"
  "return xhr;"
  "},"
  "success:function(d, s) {"
  "console.log('success!')" 
 "},"
 "error: function (a, b, c) {"
 "}"
 "});"
 "});"
 "</script>";
 
 
 

/**General Functions*/
/*
void BTcommand( String bt_buff){
	if(bt_buff.indexOf("OTA")>=0){
	int ind1=bt_buff.indexOf(",");
  int ind2=bt_buff.indexOf(",",ind1+1);
  int ind3=bt_buff.indexOf(";");
  String ssid_S=bt_buff.substring(ind1+1, ind2);
  String password_S=bt_buff.substring(ind2+1, ind3);
	
	Serial.println(bt_buff);
	
	ssid_S.trim();	
	ssid_S.remove(30);
  ssid_S.toCharArray(ssid,sizeof(ssid));
	Serial.print("SSID name: ");Serial.println(ssid); //Serial.println(ssid_S);
	
	password_S.trim();	
	password_S.remove(30);
  password_S.toCharArray(password,sizeof(password));
	Serial.print("password: ");Serial.println(password);//Serial.println(password_S);
	
	OTAdownload();
	}
	
}*/


void callback(esp_spp_cb_event_t event, esp_spp_cb_param_t *param){
  if(event == ESP_SPP_SRV_OPEN_EVT){
    Serial.println("Client Connected");
		FlagBTclientConnected=true;
  }
	if (event == ESP_SPP_CLOSE_EVT){
	Serial.println("Client Disconnected");	
	FlagBTclientConnected=false;
	}
}


void debugPrint(String s){
  #ifdef bluetoothConsole
    //SerialBT.print("DBG:");
		SerialBT.print(s);
  #endif      
  #ifdef serialConsole
		//Serial.print("DBG:");
    Serial.print(s);
  #endif    
}//debugPrint

void debugPrintln(String s){
  #ifdef bluetoothConsole
    //SerialBT.print("DBG:");
		SerialBT.print(s);
    SerialBT.print("\n");
  #endif      
  #ifdef serialConsole
    //Serial.print("DBG:");
		Serial.print(s);
    Serial.print("\n");
  #endif
}//debugPrintln

void IRAM_ATTR resetModule() {
  ets_printf("!!! WHATCHDOG REBOOT !!!\n");
	FlagWatchdogReboot=1;
	EEPROM.write(103, FlagWatchdogReboot);
  EEPROM.commit();
  esp_restart();
}//whatchdog reset ISR

void resetESP(){ //Restarts esp32 on request
	Serial.println("YOU ASKED TO RESTART ESP32");
  esp_restart(); //reset esp
  //needs timer logic
}//uCTRLrst
/*********************/

 
void OTAdonwload(){
		
	// Connect to WiFi network
	otaSSID.toCharArray(ota_ssid, sizeof(ota_ssid));
	otaPWD.toCharArray(ota_pwd, sizeof(ota_pwd));
  WiFi.begin(ota_ssid, ota_pwd);
  Serial.println("");
	
	unsigned long int connectionTimer=millis();
	
  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    if(millis()-connectionTimer>10000){
			Serial.println("noIP\n");
			SerialBT.println("\noIP\n");
			ESP.restart();
		}
  }
	
	Serial.println(WiFi.localIP());Serial.println("");
	SerialBT.println(WiFi.localIP());SerialBT.println("");
	
  /*use mdns for host name resolution*/
  if (!MDNS.begin(host)) { //http://esp32.local
    Serial.println("noMDNS!");
		SerialBT.println("noMDNS!");
    while (1) {
      delay(1000);
    }
  }
  Serial.println("MDNSstarted");
	SerialBT.println("MDNSstarted");
  /*return index page which is stored in serverIndex */
  server.on("/", HTTP_GET, []() {
    server.sendHeader("Connection", "close");
    server.send(200, "text/html", loginIndex);
  });
  server.on("/serverIndex", HTTP_GET, []() {
    server.sendHeader("Connection", "close");
    server.send(200, "text/html", serverIndex);
  });
  /*handling uploading firmware file */
  server.on("/update", HTTP_POST, []() {
    server.sendHeader("Connection", "close");
    server.send(200, "text/plain", (Update.hasError()) ? "FAIL" : "OK");
    ESP.restart();
  }, []() {
    HTTPUpload& upload = server.upload();
    if (upload.status == UPLOAD_FILE_START) {
      Serial.printf("Update: %s\n", upload.filename.c_str());
			SerialBT.printf("Update: %s\n", upload.filename.c_str());
      if (!Update.begin(UPDATE_SIZE_UNKNOWN)) { //start with max available size
        Update.printError(Serial);
      }
    } else if (upload.status == UPLOAD_FILE_WRITE) {
      /* flashing firmware to ESP*/
      if (Update.write(upload.buf, upload.currentSize) != upload.currentSize) {
        Update.printError(Serial);
      }
    } else if (upload.status == UPLOAD_FILE_END) {
      if (Update.end(true)) { //true to set the size to the current progress
        Serial.printf("Update Success: %u\nRebooting...\n", upload.totalSize);
				SerialBT.printf("Update Success: %u\nRebooting...\n", upload.totalSize);
      } else {
        Update.printError(Serial);
      }
    }
  });
  server.begin();
	while(1){
		//timerWrite(timer, 0); //reset timer (feed watchdog)
		server.handleClient();
		delay(1);
	}
 }




/*************************************/


void setup(){

  Serial.begin(115200); //PC DEBUG SERIAL


  /**PIN initializations*/
  //pinMode(LED, OUTPUT);
  ledcAttachPin(LED, 1);
  ledcSetup(1, 300, 8); // 12 kHz PWM, 8-bit resolution
	ledcWrite(1, 255);//start at full bright
  /****************************/
  
  /** whatchDog timer init*/
  timer = timerBegin(0, 80, true);                  //timer 0, div 80
  timerAttachInterrupt(timer, &resetModule, true);  //attach callback
  timerAlarmWrite(timer, wdtTimeout * 1000, false); //set time in us
  timerAlarmEnable(timer);                          //enable interrupt
  /****************************/
  
  if (!EEPROM.begin(EEPROM_SIZE)){
    debugPrintln("failed to initialise EEPROM, restarting esp32"); 
    delay(1000);
    ESP.restart();
  }
	SerialBT.register_callback(callback);
	SerialBT.begin("ASTRA_K_BT_SERIAL");
	
	
}//setup

void loop(){
	
	timerWrite(timer, 0); //reset timer (feed watchdog)
	bt_buff="";

	if(SerialBT.available()){
		while(SerialBT.available()){
			bt_buff+=(char)SerialBT.read();
		}
		SerialBT.print("bt_buff ");SerialBT.println(bt_buff);
		Serial.print("bt_buff ");Serial.println(bt_buff);
	}	
	
	if(bt_buff.toInt()){
		pwm_led=bt_buff.toInt();
		Serial.print("pwm_led ");Serial.println(pwm_led);
		ledcWrite(1, pwm_led);
		flag_flow_on=false;
		flag_flash_on=false;
	}
	
	if(bt_buff.indexOf("FLOW")>=0 || bt_buff.indexOf("flow")>=0){
		//FLOW,flowMinBright,flowMaxBright,flowDdelay_ms;
		int ind0_flow=bt_buff.indexOf(",");
		int ind1_flow=bt_buff.indexOf(",",ind0_flow+1);
		int ind2_flow=bt_buff.indexOf(",",ind1_flow+1);
		int ind3_flow=bt_buff.indexOf(";");
		String flowMinBright=bt_buff.substring(ind0_flow+1, ind1_flow);
		String flowMaxBright=bt_buff.substring(ind1_flow+1, ind2_flow);
		String flowDdelay_ms=bt_buff.substring(ind2_flow+1, ind3_flow);
		if(flowMinBright.toInt() && flowMaxBright.toInt() && flowDdelay_ms.toInt()){
			flow_min_bright=flowMinBright.toInt();
			flow_max_bright=flowMaxBright.toInt();
			flow_delay_ms=flowDdelay_ms.toInt();
			flag_flow_on=true;
		}
		
	}
	
	if(flag_flow_on){
		for (int i=flow_min_bright;i<=flow_max_bright;i++){
			ledcWrite(1, i);
			delay(flow_delay_ms);
		}
		for (int i=flow_max_bright;i>=flow_min_bright;i--){
			ledcWrite(1, i);
			delay(flow_delay_ms);
		}
	}
	
	
	if(bt_buff.indexOf("FLASH")>=0 || bt_buff.indexOf("flash")>=0){
		//FLOW,flashNumber,flashDelay_ms;
		int ind0_flash=bt_buff.indexOf(",");
		int ind1_flash=bt_buff.indexOf(",",ind0_flash+1);
		int ind2_flash=bt_buff.indexOf(";");
		String flashNumber=bt_buff.substring(ind0_flash+1, ind1_flash);
		String flashDelay_ms=bt_buff.substring(ind1_flash+1, ind2_flash);
		if(flashNumber.toInt() && flashDelay_ms.toInt()){
			flash_number=flashNumber.toInt();
			flash_delay_ms=flashDelay_ms.toInt();
			flag_flash_on=true;
		}
		
	}
	
	if(flag_flash_on){
		for (int i=0;i<flash_number;i++){
			ledcWrite(1, 255);
			delay(flash_delay_ms);
			ledcWrite(1, 0);
			delay(flash_delay_ms);
		}
		ledcWrite(1, pwm_led);
		flag_flash_on=false;
	}
	
	
	if(bt_buff.indexOf("OFF")==0 || bt_buff.indexOf("off")==0 || bt_buff.indexOf('0')==0){
		ledcWrite(1, 0);
		flag_flow_on=false;
		flag_flash_on=false;
		pwm_led=0;
	}
	
	if(bt_buff.indexOf("temp")==0 || bt_buff.indexOf("TEMP")==0){
		SerialBT.print("temperature: ");
		float internal_temp=0;
		for(int i=0;i<3;i++){
			internal_temp=internal_temp+((temprature_sens_read() - 32) / 1.8);
		}
		internal_temp=(internal_temp / 3)-32; //32 is the calibrated value at 19 ambient temp
		SerialBT.print(internal_temp);
		SerialBT.println(" C");
	}
	
	if(FlagBTclientConnected && !FlagBTclientConnected_led_notification){
		FlagBTclientConnected_led_notification=true;
		for (int i=0;i<3;i++){
			ledcWrite(1, 255);
			delay(150);
			ledcWrite(1, 0);
			delay(100);
		}
		if(flag_1st_connection){
			ledcWrite(1, 255);
			flag_1st_connection=false;
		}
		else
			ledcWrite(1, pwm_led);
	}
	
	if(!FlagBTclientConnected && FlagBTclientConnected_led_notification){
		FlagBTclientConnected_led_notification=false;
		for (int i=0;i<3;i++){
			ledcWrite(1, 255);
			delay(150);
			ledcWrite(1, 0);
			delay(100);
		}
		ledcWrite(1, pwm_led);
	}
	
	if(bt_buff.indexOf("ota")>=0 || bt_buff.indexOf("OTA")>=0){
		//FLOW,otaSSID,otaPWD;
		int ind0_ota=bt_buff.indexOf(",");
		int ind1_ota=bt_buff.indexOf(",",ind0_ota+1);
		int ind2_ota=bt_buff.indexOf(";");
		otaSSID="";
		otaPWD="";
		otaSSID=bt_buff.substring(ind0_ota+1, ind1_ota);
		otaPWD=bt_buff.substring(ind1_ota+1, ind2_ota);
		OTAdonwload();
	}

	/*for(int i=0;i<255;i++){
		ledcWrite(1, i);
		delay(5);
	}
	for(int i=255;i>0;i--){
		ledcWrite(1, i);
		delay(5);
	}*/
  
}//loop
