using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace BetaSoft.EPaperHatCore.IO
{
    public class EPaperConnection : IEpaperConnection
    {
        public void SendCommand(int command)
        {
            Connections.DcPin.Write(GpioPinValue.Low);
            Connections.CsPin.Write(GpioPinValue.Low);
            Pi.Spi.Channel0.SendReceive(BitConverter.GetBytes(command));
            Connections.CsPin.Write(GpioPinValue.High);
        }

        public void SendData(int data)
        {
            Connections.DcPin.Write(GpioPinValue.High);
            Connections.CsPin.Write(GpioPinValue.Low);
            Pi.Spi.Channel0.SendReceive(BitConverter.GetBytes(data));
            Connections.CsPin.Write(GpioPinValue.High);
        }
    }
}