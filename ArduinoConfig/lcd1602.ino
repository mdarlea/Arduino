#include "ansi.h"
#include <LiquidCrystal.h>
#include "greenhouse-led.cpp"

ANSI ansi(&Serial);

const float BETA = 3950; // should match the Beta Coefficient of the thermistor

volatile bool systemStarted;
int button = 2;

int speaker = A2;

GreenhouseLed ledExterior = GreenhouseLed(13, 12, 11, speaker);
GreenhouseLed ledInterior = GreenhouseLed(10, 9, A1, speaker);

LiquidCrystal lcd(8, 7, 6, 5, 4, 3);

float indoorTemperature = -500;
float outdoorTemperature = -500;
float lastOutdoorTemperature = -500;

float indoorMinTemperature = 19;
float indoorMaxTemperature = 25;

float outdoorMinTemperature = 10;
float outdoorMaxTemperature = 30;

const byte numChars = 32;
char receivedChars[numChars];
char tempChars[numChars];        // temporary array for use when parsing

// variables to hold the parsed data
char messageFromPC[numChars] = {0};
int integerFromPC = 0;
float floatFromPC = 0.0;

boolean newData = false;

void setup() 
{
  Serial.begin(9600); 

  lcd.begin(16, 2);

  ledExterior.setup();
  ledInterior.setup();

  pinMode(speaker, OUTPUT);

  pinMode(button, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(button), systemStartedISR, FALLING);
}

void loop() 
{
  if (!systemStarted) {
    ledExterior.turnLedsOff();
    ledInterior.turnLedsOff();

    lcd.clear();

    floatFromPC = 0.0;
    integerFromPC = 0;

    indoorTemperature = -500;
    outdoorTemperature = -500;

    return;
  }

  //measure internal temperature
  int analogValue = analogRead(A0);
  float indoorCelsius = 1 / (log(1 / (1023. / analogValue - 1)) / BETA + 1.0 / 298.15) - 273.15;      
  
  if (indoorTemperature != indoorCelsius) {
    setIndoorTemperature(indoorCelsius);
  } 

  if (outdoorTemperature != lastOutdoorTemperature) {
    setOutdoorTemperature(lastOutdoorTemperature);
  }

  if (Serial.available()) {
    recvWithStartEndMarkers();
    if (newData == true) {
      strcpy(tempChars, receivedChars);            
      parseData();      
      newData = false;

      if (strcmp(messageFromPC, "OMax") == 0) {
        Serial.println(outdoorMaxTemperature);
      }

      if (strcmp(messageFromPC, "OMin") == 0) {
        Serial.println(outdoorMinTemperature);
      }

      if (strcmp(messageFromPC, "IMax") == 0) {
        Serial.println(indoorMaxTemperature);
      }

      if (strcmp(messageFromPC, "IMin") == 0) {
        Serial.println(indoorMinTemperature);
      }      

      if (strcmp(messageFromPC, "TI") == 0) {
        Serial.println(indoorTemperature);
      }

      if (strcmp(messageFromPC, "TO") == 0) { 
        setOutdoorTemperature(floatFromPC);
        lastOutdoorTemperature = floatFromPC;
      } 
    }
  }

  delay(500);
}

void setOutdoorTemperature(float temperature) {
  if (outdoorTemperature != temperature) {
    //display outdoor temperature          
    lcd.setCursor(0, 1);
    lcd.print("Outdoor: ");
    lcd.print(temperature);
    lcd.print(" C");  

    if (temperature < outdoorMinTemperature) {
      // if outdoor temperature is lower than min temperature then it turns the blue led on
      ledExterior.turnBlueLedOn();
    } else if (temperature > outdoorMaxTemperature) {
      // if outdoor temperature is higher than max temperature then it turns the red led on
      ledExterior.turnRedLedOn();
    } else {
      // if outdoor temperature is within limits then it turns the green led on
      ledExterior.turnGreenLedOn();
    }

    outdoorTemperature = temperature;  
  }  
}

void setIndoorTemperature(float temperature) {
  if (outdoorTemperature != temperature) {
    //display indoor temperature          
    lcd.setCursor(0, 0);
    lcd.print("Indoor: ");
    lcd.print(temperature);
    lcd.print(" C");   

    if (temperature < indoorMinTemperature) {
      // if indoor temperature is lower than min temperature then it turns the blue led on   
      ledInterior.turnBlueLedOn();
    } else if (temperature > outdoorMaxTemperature) {
      // if indoor temperature is higher than max temperature then it turns the red led on
      ledInterior.turnRedLedOn();
    } else {
      // if indoor temperature is within limits then it turns the green led on
      ledInterior.turnGreenLedOn();      
    }

    indoorTemperature = temperature; 
  }  
}

void systemStartedISR() {
  systemStarted = !systemStarted;
}

//reads data from the serial monitor in the following format: <text, integer, float>. Example: <TO, 30, 0.25>
void recvWithStartEndMarkers() {
    static boolean recvInProgress = false;
    static byte ndx = 0;
    char startMarker = '<';
    char endMarker = '>';
    char rc;

    while (Serial.available() > 0 && newData == false) {
        rc = Serial.read();

        if (recvInProgress == true) {
            if (rc != endMarker) {
                receivedChars[ndx] = rc;
                ndx++;
                if (ndx >= numChars) {
                    ndx = numChars - 1;
                }
            }
            else {
                receivedChars[ndx] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                newData = true;
            }
        }

        else if (rc == startMarker) {
            recvInProgress = true;
        }
    }
}

//split the data into its parts
void parseData() {      

    char * strtokIndx; //this is used by strtok() as an index

    strtokIndx = strtok(tempChars,","); //get the first part - the string
    strcpy(messageFromPC, strtokIndx); // copy it to messageFromPC
 
    strtokIndx = strtok(NULL, ","); //this continues where the previous call left off
    integerFromPC = atoi(strtokIndx); //convert this part to an integer

    strtokIndx = strtok(NULL, ",");
    floatFromPC = atof(strtokIndx); //convert this part to a float

}