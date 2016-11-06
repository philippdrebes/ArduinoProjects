// *** SentandReceive ***

// This example expands the previous Receive example. The Arduino will now send back a status.
// It adds a demonstration of how to:
// - Handle received commands that do not have a function attached
// - Send a command with a parameter to the PC

#include <CmdMessenger.h>  // CmdMessenger

// Blinking led variables 
const int kBlinkLed             = 13;  // Pin of internal Led

const int cLedLeftTop           = 2;
const int cLedLeftBottom        = 3;
const int cLedMiddleTop         = 4;
const int cLedMiddleBottom      = 5;
const int cLedRightTop          = 6;
const int cLedRightBottom       = 7;

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  SetLedLeft          , // Command to request led to be set in specific state
  SetLedMiddle        , // Command to request led to be set in specific state
  SetLedRight         , // Command to request led to be set in specific state
  Status              , // Command to report status
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(SetLedLeft, OnSetLedLeft);
  cmdMessenger.attach(SetLedMiddle, OnSetLedMiddle);
  cmdMessenger.attach(SetLedRight, OnSetLedRight);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(Status,"Command without attached callback");
}

void OnSetLedLeft()
{
  OnSetLed(SetLedLeft);
}

void OnSetLedMiddle()
{
  OnSetLed(SetLedMiddle);
}

void OnSetLedRight()
{
  OnSetLed(SetLedRight);
}
  
// Callback function that sets led on or off
void OnSetLed(int pos)
{
  // Read led state argument, interpret string as boolean
  bool state = cmdMessenger.readBoolArg();
  
  // Set led
  if (pos == SetLedLeft) {
    digitalWrite(cLedLeftTop, state?HIGH:LOW);
    digitalWrite(cLedLeftBottom, state?LOW:HIGH);
  } else if (pos == SetLedMiddle) {
    digitalWrite(cLedMiddleTop, state?HIGH:LOW);
    digitalWrite(cLedMiddleBottom, state?LOW:HIGH);
  } else if (pos == SetLedRight) {
    digitalWrite(cLedRightTop, state?HIGH:LOW);
    digitalWrite(cLedRightBottom, state?LOW:HIGH);
  } else {
    digitalWrite(kBlinkLed, state?HIGH:LOW);
  }
  
  // Send back status that describes the led state
  cmdMessenger.sendCmd(Status,(int)state);
}

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the PC
  Serial.begin(115200); 

  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  // Note that this is a good debug function: it will let you also know 
  // if your program had a bug and the arduino restarted  
  cmdMessenger.sendCmd(Status,"Arduino has started!");

  // set pin for blink LED
  pinMode(kBlinkLed, OUTPUT);
  
  pinMode(cLedLeftTop, OUTPUT);
  pinMode(cLedLeftBottom, OUTPUT);
  pinMode(cLedMiddleTop, OUTPUT);
  pinMode(cLedMiddleBottom, OUTPUT);
  pinMode(cLedRightTop, OUTPUT);
  pinMode(cLedRightBottom, OUTPUT);
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
}
