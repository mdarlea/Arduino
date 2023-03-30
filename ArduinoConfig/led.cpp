#include "Arduino.h"

class Led {
  public:
    Led(int ledR, int ledG, int ledB, int speaker) {
      _ledR = ledR;
      _ledG = ledG;
      _ledB = ledB;

      _speaker = speaker;
    };

    void setup() {
      pinMode(_ledR, OUTPUT);
      pinMode(_ledG, OUTPUT);
      pinMode(_ledB, OUTPUT);
    };

    void disable() {
      setDigitalPins(LOW);
    };

  protected:
    void turnLedOn(int redValue, int greenValue, int blueValue, int sound) {
      turnLedOn(redValue, greenValue, blueValue);

      tone(_speaker, sound);
      delay(1000);
      noTone(_speaker);
    };

    void turnLedOn(int redValue, int greenValue, int blueValue) {
      setDigitalPins(HIGH);

      setColor(redValue, greenValue, blueValue);
    };   

  private:
    int _ledR, _ledG, _ledB, _speaker;
    
    void setDigitalPins(int value) {
      if(digitalRead(_ledR) != value) {
        digitalWrite(_ledR, value);
      }

      if(digitalRead(_ledG) != value) {
        digitalWrite(_ledG, value);
      }

      if(digitalRead(_ledB) != value) {
        digitalWrite(_ledB, value);
      }
    };

    void setColor(int redValue, int greenValue, int blueValue) {
      analogWrite(_ledR, redValue);
      analogWrite(_ledG, greenValue);
      analogWrite(_ledB, blueValue);
    }
};


