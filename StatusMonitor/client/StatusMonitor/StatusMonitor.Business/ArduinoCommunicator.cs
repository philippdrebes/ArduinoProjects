using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor.Business
{
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    enum Command
    {
        SetLedLeft,
        SetLedMiddle,
        SetLedRight,
        Status
    };

    public class ArduinoCommunicator
    {
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;

        private string _connectedPort = null;
        private List<string> _availablePorts = null;
        public List<string> AvailablePorts { get { return _availablePorts; } }

        private bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }

        private static ArduinoCommunicator _instance = null;
        public static ArduinoCommunicator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ArduinoCommunicator();
                return _instance;
            }
        }

        private ArduinoCommunicator()
        {
            _availablePorts = SerialPort.GetPortNames().ToList();   // Get a list of serial port names.

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in _availablePorts)
            {
                Console.WriteLine(port);
            }
        }

        public void Start(string port)
        {
            _connectedPort = port;

            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = _connectedPort, BaudRate = 115200, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            // Set if it is communicating with a 16- or 32-bit Arduino board
            _cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit16);

            AttachCommandCallBacks();   // Attach the callbacks to the Command Messenger

            var status = _cmdMessenger.Connect();   // Start listening
            if (!status)
            {
                Console.WriteLine("No connection could be made");
                return;
            }
            _isRunning = true;
        }

        public void SendState(bool state)
        {
            var command = new SendCommand((int)Command.SetLedLeft, state);  // Create command
            _cmdMessenger.SendCommand(command); // Send command
        }

        public void Stop()
        {
            if (_cmdMessenger != null)
            {
                _cmdMessenger.Disconnect();
                _cmdMessenger.Dispose();
            }
            if (_serialTransport != null) _serialTransport.Dispose();
        }

        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Status, OnStatus);
        }

        /// Executes when an unknown command has been received.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }

        // Callback function that prints the Arduino status to the console
        void OnStatus(ReceivedCommand arguments)
        {
            Console.Write("Arduino status: ");
            Console.WriteLine(arguments.ReadStringArg());
        }
    }
}
