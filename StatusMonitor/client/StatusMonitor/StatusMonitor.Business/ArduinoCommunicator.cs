using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor.Business
{
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    enum Command
    {
        SetLed,
        Status
    };

    public class ArduinoCommunicator
    {
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;

        public ArduinoCommunicator()
        {
            Setup();
        }

        public void Setup()
        {
            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM6", BaudRate = 115200, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            // Set if it is communicating with a 16- or 32-bit Arduino board
            _cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit16);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Start listening
            var status = _cmdMessenger.Connect();
            if (!status)
            {
                Console.WriteLine("No connection could be made");
                return;
            }
        }

        public void SetState()
        {
            bool _ledState = false;
            // Create command
            var command = new SendCommand((int)Command.SetLed, _ledState);

            // Send command
            _cmdMessenger.SendCommand(command);
        }

        public void Exit()
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
