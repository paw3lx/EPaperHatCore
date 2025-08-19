using System;
using System.Device.Gpio;

namespace EPaperHatCore.IO;
internal class EPaperConnection : IEpaperConnection
{
    private readonly Connections _connection;
    private static readonly object _syncLock = new object();

    public EPaperConnection(Connections connection)
    {
        _connection = connection;
    }

    public void SendCommand(int command)
    {
        lock (_syncLock)
        {
            _connection.GpioController.Write(_connection.DcPin, PinValue.Low);
            _connection.GpioController.Write(_connection.CsPin, PinValue.Low);
            _connection.SpiDevice.Write(ToBytes(command));
            _connection.GpioController.Write(_connection.CsPin, PinValue.High);
        }
    }

    public void SendData(int data)
    {
        lock (_syncLock)
        {
            _connection.GpioController.Write(_connection.DcPin, PinValue.High);
            _connection.GpioController.Write(_connection.CsPin, PinValue.Low);
            _connection.SpiDevice.Write(ToBytes(data));
            _connection.GpioController.Write(_connection.CsPin, PinValue.High);
        }
    }

    private byte[] ToBytes(int intValue)
    {
        byte b = (byte)intValue;
        return new byte[1] { b };
    }
}