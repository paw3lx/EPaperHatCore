using BetaSoft.EPaperHatCore.IO;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace BetaSoft.EPaperHatCore
{
    internal class Connections
    {
        private readonly IHardwareSpecification _specification;
        public IGpioPin ResetPin { get; private set; }
        public IGpioPin DcPin { get; private set; } 
        public IGpioPin CsPin { get; private set; }
        public IGpioPin BusyPin { get; private set; }
        public ISpiChannel Channel { get; private set; }
        private static readonly object _syncLock = new object();

        public Connections(IHardwareSpecification specification)
        {
            _specification = specification;
        }

        public void Initialize()
        {
            lock(_syncLock)
            {
                ResetPin = Pi.Gpio[_specification.RST_PIN];
                ResetPin.PinMode = GpioPinDriveMode.Output;

                DcPin = Pi.Gpio[_specification.DC_PIN];
                DcPin.PinMode = GpioPinDriveMode.Output;

                CsPin = Pi.Gpio[_specification.CS_PIN];
                CsPin.PinMode = GpioPinDriveMode.Output;

                BusyPin = Pi.Gpio[_specification.BUSY_PIN];
                BusyPin.PinMode = GpioPinDriveMode.Input;

                Pi.Spi.Channel0Frequency = _specification.Channel0Frequency;
                Channel = Pi.Spi.Channel0;
            }
        }
    }
}