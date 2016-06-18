#include <LiquidCrystal.h>
#include <Chrono.h>
#include <LightChrono.h>

/* Pin Definitions */
const int laserPin = 2;     // the number of the pushbutton pin
const int buzzerPin =  3;      // the number of the LED pin
const int onPin = 4;     // the number of the pushbutton pin

const int returnPin = 5;
const int prevPin = 6;
const int nextPin = 7;
const int setPin = 8;
/* End Pin Definitions*/

/* Constants */
const int mainMenuIndex = 0;
const int settingsViewIndex = 1;
const int chronoViewIndex = 2;
const int resultsViewIndex = 3;
/* End Constants */

/* Variable Definitions */
Chrono chrono(Chrono::MICROS);
LightChrono measurementTimeout;

unsigned long measuredTimes[10];
int measurementIndex = 0;
int maxMeasurements = 5;

int laserState = 0;         // variable for reading the pushbutton status
int oldLaserState = 0;
bool enabled = true;
/* End Variable Definitions */

/* Menu Definitions */
int viewIndex = 0;
int subviewIndex = 0;

void mainMenu();
void settingsView();
void chronoView();
void resultsView();

void (*views[4]) ();
/* End Menu Definitions */

void setup() {
  Serial.begin(9600);   // Test
  Serial.println("Stopwatch");

  // Pin Modes
  pinMode(buzzerPin, OUTPUT);
  pinMode(laserPin, INPUT);
  pinMode(onPin, INPUT);

  pinMode(returnPin, INPUT);
  pinMode(prevPin, INPUT);
  pinMode(nextPin, INPUT);
  pinMode(setPin, INPUT);

  // Chrono initializations
  measurementTimeout.restart();
  chrono.stop();

  // Menu initializations
  views[mainMenuIndex] = mainMenu;
  views[settingsViewIndex] = settingsView;
  views[chronoViewIndex] = chronoView;
  views[resultsViewIndex] = resultsView;
}

void loop() {
  (*views[viewIndex]) ();
}

void mainMenu() {
	Serial.println("RaceTimer");

	switch(subviewIndex){
		case 0:
			Serial.println("Start");
			break;
		case 1:
			Serial.println("Settings");
			break;
		default:
			break;
	}

  if (digitalRead(nextPin)) {
    subviewIndex++;
  }
  if (digitalRead(prevPin)) {
    subviewIndex--;
  }

}

void settingsView() {

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

    if (digitalRead(returnPin)) {
      setView(mainMenuIndex);
    }
}

void setView(int index) {
  viewIndex = index
}

void chronoView() {

  if (measurementTimeout.hasPassed(600)){
    digitalWrite(buzzerPin, LOW);
  }

  if (/*digitalRead(onPin) == HIGH &&*/ enabled) { // soll ueberhaupt gemessen werden?

    oldLaserState = laserState;
    laserState = digitalRead(laserPin);

    if (laserState == LOW && chrono.isRunning()) {    // zeit laeuft

       printTime(chrono.elapsed());

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
  }

}

void resultsView() {
	Serial.println("Results");

  for (int i = 0; i < maxMeasurements; i++) {
    printTime(measuredTimes[i]);
  }

  measurementIndex++;

}

void checkviewIndex() {
	if (digitalRead(nextPin)) {
		viewIndex++;
	}
	if (digitalRead(prevPin)) {
		viewIndex--;
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
