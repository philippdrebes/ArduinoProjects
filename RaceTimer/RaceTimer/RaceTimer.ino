#include <LiquidCrystal.h>
#include <Chrono.h>
#include <LightChrono.h>

const int laserPin = 2;     // the number of the pushbutton pin
const int buzzerPin =  3;      // the number of the LED pin
const int onPin = 4;     // the number of the pushbutton pin

const int returnPin = 5;
const int prevPin = 6;
const int nextPin = 7;
const int setPin = 8;

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
  
  pinMode(returnPin, INPUT);
  pinMode(prevPin, INPUT);
  pinMode(nextPin, INPUT);
  pinMode(setPin, INPUT);
  
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

void settingsMenu() {

  do {
    Serial.println("Settings");

    Serial.print("Number of Measurements: ");
    Serial.print(maxMeasurements);
    Serial.println();

    if (digitalRead(nextPin)) {
      maxMeasurements++;
    }
    if (digitalRead(prevPin)) {
      maxMeasurements--;
    }
    
  } while (!digitalRead(returnPin));

}

void logAndRestart() {
    if (measurementIndex < maxMeasurements){
      measuredTimes[measurementIndex] = chrono.elapsed();
      measurementIndex++;
  
      chrono.restart();
      measurementTimeout.restart();
    }
 }

void printTime(unsigned long t_milli)
{
   char buffer[20];
   int mins, secs ;
   unsigned long millisecs;

   mins = t_milli / (1000*60);
   millisecs = t_milli - (mins * 1000 * 60);
   
   secs = millisecs / 1000;

   millisecs = millisecs - (secs * 1000);

   sprintf(buffer, "%02d:%02d:%03d", mins, secs, millisecs);
   Serial.println(buffer);
   //lcd.print(buffer);
}
