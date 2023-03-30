#include "led.cpp"

class GreenhouseLed: public Led {
  public:
    using Led::Led;
    
    void turnRedLedOn() {
      turnLedOn(255, 0, 0, NOTE_C1);
    };

    void turnGreenLedOn() {
      turnLedOn(0, 255, 0);
    };

    void turnBlueLedOn() {
      turnLedOn(0, 0, 255, NOTE_B0);
    };

  private:
    int NOTE_B0 = 262;
    int NOTE_C1 = 294;
};