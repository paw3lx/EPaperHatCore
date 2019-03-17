using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace BetaSoft.EPaperHatCore
{
    internal static class Connections
    {
        public static readonly IGpioPin ResetPin, DcPin, CsPin, BusyPin;
        private static readonly object SyncLock = new object();
        private const int RST_PIN = 17;
        private const int DC_PIN = 25;
        private const int CS_PIN = 8;
        private const int BUSY_PIN = 24;

        static Connections()
        {
            lock(SyncLock)
            {
                ResetPin = Pi.Gpio[RST_PIN];
                ResetPin.PinMode = GpioPinDriveMode.Output;

                DcPin = Pi.Gpio[DC_PIN];
                DcPin.PinMode = GpioPinDriveMode.Output;

                CsPin = Pi.Gpio[CS_PIN];
                CsPin.PinMode = GpioPinDriveMode.Output;

                BusyPin = Pi.Gpio[BUSY_PIN];
                BusyPin.PinMode = GpioPinDriveMode.Input;

                Pi.Spi.Channel0Frequency = 2000000;
            }
        }
    }
}