#include <LiquidCrystal.h>
#include <Chrono.h>
#include <LightChrono.h>

// constants won't change. They're used here to
// set pin numbers:
const int laserPin = 2;     // the number of the pushbutton pin
const int buzzerPin =  3;      // the number of the LED pin
const int onPin = 4;     // the number of the pushbutton pin

Chrono chrono(Chrono::MICROS);
unsigned long measuredTimes[10];
int measurementIndex = 0;
int maxMeasurements = 5;

LightChrono measurementTimeout;

int laserState = 0;         // variable for reading the pushbutton status
int oldLaserState = 0;
bool enabled = true;

void setup() {
  Serial.begin(9600);   // Test
  Serial.println("Stopwatch");
  pinMode(buzzerPin, OUTPUT);
  pinMode(laserPin, INPUT);
  pinMode(onPin, INPUT);
  
  measurementTimeout.restart();

  chrono.stop();
}

void loop() {
  
  if (measurementTimeout.hasPassed(600)){
    digitalWrite(buzzerPin, LOW);
  }

  if (/*digitalRead(onPin) == HIGH &&*/ enabled) { // soll ueberhaupt gemessen werden?
    
    oldLaserState = laserState;
    laserState = digitalRead(laserPin);
        
    if (laserState == LOW && chrono.isRunning()) {    // zeit laeuft
      
      Serial.println(chrono.elapsed());
      
    } else if (laserState == HIGH && measurementTimeout.hasPassed(1500) && chrono.isRunning()) {    // zeit stoppen
      
      Serial.println("Lap");
      digitalWrite(buzzerPin, HIGH);
      
      
      logAndRestart();
      
    }
    else if (laserState == HIGH && !chrono.isRunning()) {   // first start
      chrono.restart();
      measurementTimeout.restart();
    }
    
  }

  if (measurementIndex >= maxMeasurements) {
    enabled = false;
    
    Serial.println("Gemessene Zeiten:");
    for (int i = 0; i < maxMeasurements; i++) {
      printTime(measuredTimes[i]);
    }
    measurementIndex++;
    delay(10000);
  }
  
}

void logAndRestart() {
    if (measurementIndex < maxMeasurements){
      measuredTimes[measurementIndex] = chrono.elapsed();
      measurementIndex++;
  
      chrono.restart();
      measurementTimeout.restart();
    }
 }


// http://forum.arduino.cc/index.php?topic=18588.0
// argument is time in milliseconds
void printTime(unsigned long t_milli)
{
   char buffer[20];
   int days, hours, mins, secs;
   int fractime;
   unsigned long inttime;

   inttime  = t_milli / 1000;
   fractime = t_milli % 1000;
   // inttime is the total number of number of seconds
   // fractimeis the number of thousandths of a second

   // number of days is total number of seconds divided by 24 divided by 3600
   days     = inttime / (24*3600);
   inttime  = inttime % (24*3600);

   // Now, inttime is the remainder after subtracting the number of seconds
   // in the number of days
   hours    = inttime / 3600;
   inttime  = inttime % 3600;

   // Now, inttime is the remainder after subtracting the number of seconds
   // in the number of days and hours
   mins     = inttime / 60;
   inttime  = inttime % 60;

   // Now inttime is the number of seconds left after subtracting the number
   // in the number of days, hours and minutes. In other words, it is the
   // number of seconds.
   secs = inttime;

   // Don't bother to print days
   sprintf(buffer, "%02d:%02d:%02d.%03d", hours, mins, secs, fractime);
   Serial.println(buffer);
   //lcd.print(buffer);
}
