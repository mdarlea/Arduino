#include "ansi.h"
#include <LiquidCrystal.h>

ANSI ansi(&Serial);

const float BETA = 3950; // should match the Beta Coefficient of the thermistor

volatile bool systemStarted;
int button = 2;

int ledExteriorR = 13;
int ledExteriorG = 12;
int ledExteriorB = 11;

int ledInteriorR = 10;
int ledInteriorG = 9;
int ledInteriorB = A1;

//volatile bool ledInteriorOK = false;
//volatile bool ledExteriorOK = false;

int speaker = A2;
#define NOTE_B0  262
#define NOTE_C1  294

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

  pinMode(button, INPUT_PULLUP);

  pinMode(ledExteriorR, OUTPUT);
  pinMode(ledExteriorG, OUTPUT);
  pinMode(ledExteriorB, OUTPUT);

  pinMode(ledInteriorR, OUTPUT);
  pinMode(ledInteriorG, OUTPUT);
  pinMode(ledInteriorB, OUTPUT);

  pinMode(speaker, OUTPUT);

  pinMode(button, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(button), systemStartedISR, FALLING);

  Serial.println("Hello, I'm in a terminal!");
  Serial.println();
}

void loop() 
{
  if (!systemStarted) {
    disableLedExterior();
    disableLedInterior();

    noTone(speaker);

    lcd.clear();

    floatFromPC = 0.0;
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
    enableLedExterior();

    //display outdoor temperature          
    lcd.setCursor(0, 1);
    lcd.print("Outdoor: ");
    lcd.print(temperature);
    lcd.print(" C");  

    if (temperature < outdoorMinTemperature) {      
      setExteriorLedColor(0, 0, 255);                 
      tone(speaker, NOTE_B0);
      delay(1000);
      noTone(speaker);
    } else if (temperature > outdoorMaxTemperature) {
      setExteriorLedColor(255, 0, 0);
      tone(speaker, NOTE_C1);
      delay(1000);
      noTone(speaker);
    } else {
      setExteriorLedColor(0, 255, 0);
      noTone(speaker);
    }

    outdoorTemperature = temperature;  
  }  
}

void setIndoorTemperature(float temperature) {
  if (outdoorTemperature != temperature) {
    enableLedInterior();

    //display outdoor temperature          
    lcd.setCursor(0, 0);
    lcd.print("Indoor: ");
    lcd.print(temperature);
    lcd.print(" C");   

    if (temperature < indoorMinTemperature) {      
      setInteriorLedColor(0, 0, 255);                 
      tone(speaker, NOTE_B0);
      delay(1000);
      noTone(speaker);
    } else if (temperature > outdoorMaxTemperature) {
      setInteriorLedColor(255, 0, 0);
      tone(speaker, NOTE_C1);
      delay(1000);
      noTone(speaker);      
    } else {
      setInteriorLedColor(0, 255, 0);
      noTone(speaker);
    }

    indoorTemperature = temperature; 
  }  
}

void setExteriorLedColor(int redValue, int greenValue, int blueValue) {
  analogWrite(ledExteriorR, redValue);
  analogWrite(ledExteriorG, greenValue);
  analogWrite(ledExteriorB, blueValue);
}

void setInteriorLedColor(int redValue, int greenValue, int blueValue) {
  analogWrite(ledInteriorR, redValue);
  analogWrite(ledInteriorG, greenValue);
  analogWrite(ledInteriorB, blueValue);
}

void systemStartedISR() {
  systemStarted = !systemStarted;
}

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

void parseData() {      // split the data into its parts

    char * strtokIndx; // this is used by strtok() as an index

    strtokIndx = strtok(tempChars,",");      // get the first part - the string
    strcpy(messageFromPC, strtokIndx); // copy it to messageFromPC
 
    strtokIndx = strtok(NULL, ","); // this continues where the previous call left off
    integerFromPC = atoi(strtokIndx);     // convert this part to an integer

    strtokIndx = strtok(NULL, ",");
    floatFromPC = atof(strtokIndx);     // convert this part to a float

}

void enableLedInterior() {
  digitalWrite(ledInteriorR, HIGH);
  digitalWrite(ledInteriorG, HIGH);
  digitalWrite(ledInteriorB, HIGH);
}

void enableLedExterior() {
  digitalWrite(ledExteriorR, HIGH);
  digitalWrite(ledExteriorG, HIGH);
  digitalWrite(ledExteriorB, HIGH);
}

void disableLedExterior() {
  digitalWrite(ledExteriorR, LOW);
  digitalWrite(ledExteriorG, LOW);
  digitalWrite(ledExteriorB, LOW);
}

void disableLedInterior() {
  digitalWrite(ledInteriorR, LOW);
  digitalWrite(ledInteriorG, LOW);
  digitalWrite(ledInteriorB, LOW);
}