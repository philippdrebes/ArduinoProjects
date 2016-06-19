#include <LiquidCrystal.h>
#include <Chrono.h>
#include <LightChrono.h>

/* Pin Definitions */
const int laserPin = 6;
const int buzzerPin =  13;
const int onPin = 4;

const int returnPin = 10;
const int prevPin = 9;
const int nextPin = 8;
const int setPin = 7;
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
int subViewIndex = 0;

void mainMenu();
void settingsView();
void chronoView();
void resultsView();

void (*views[4]) ();
/* End Menu Definitions */

LiquidCrystal lcd(12, 11, 5, 4, 3, 2);

void setup() {
  Serial.begin(9600);   // Test
  Serial.println("Stopwatch");

  lcd.begin(16, 2);

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

void printLcd(int line, String text) {
  lcd.setCursor(0, line);
  lcd.print(text);
}

void mainMenu() {
	Serial.println("RaceTimer");
  printLcd(0, "RaceTimer");

  switch(subViewIndex){
		case 0:
			Serial.println("Start");
      printLcd(1, "Start");
			break;
		case 1:
			Serial.println("Settings");
      printLcd(1, "Settings");
			break;
		default:
			break;
	}

  if (digitalRead(nextPin)) {
    if (subViewIndex >= 1) {
      subViewIndex = 0;
    }
    else {
      subViewIndex++;
    }
  }
  if (digitalRead(prevPin)) {
    if (subViewIndex > 0) {
      subViewIndex--;
    }
  }
  if (digitalRead(setPin)) {
    switch (subViewIndex) {
      case 0:
        setView(chronoViewIndex);
        break;
      case 1:
        setView(settingsViewIndex);
        break;
    }
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
  viewIndex = index;
}

void chronoView() {
  if (digitalRead(returnPin)) {
    setView(mainMenuIndex);
  }

  if (measurementTimeout.hasPassed(600)){
    digitalWrite(buzzerPin, LOW);
  }

  if (/*digitalRead(onPin) == HIGH &&*/ enabled) { // soll ueberhaupt gemessen werden?

    oldLaserState = laserState;
    laserState = digitalRead(laserPin);

    if (laserState == LOW && chrono.isRunning()) {    // zeit laeuft

      printLcd(0, formatTime(chrono.elapsed()));

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
    printLcd(0, formatTime(measuredTimes[i]));
  }
  if (digitalRead(prevPin)) {
    setView(mainMenuIndex);
	}
}

void checkViewIndex() {
	if (digitalRead(nextPin)) {
		subViewIndex++;
	}
	if (digitalRead(prevPin)) {
		subViewIndex--;
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

String formatTime(unsigned long t_milli)
{
   char buffer[20];
   int mins, secs ;
   unsigned long millisecs;

   mins = t_milli / (1000*60);
   millisecs = t_milli - (mins * 1000 * 60);

   secs = millisecs / 1000;

   millisecs = millisecs - (secs * 1000);

   sprintf(buffer, "%02d:%02d:%03d", mins, secs, millisecs);
   return buffer;
}
