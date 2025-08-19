using System;
using System.Device.Gpio;
using System.Device.Spi;
using EPaperHatCore.IO;

namespace EPaperHatCore;
public class Connections : IDisposable
{
    private readonly IHardwareSpecification _specification;
    public int ResetPin { get; private set; }
    public int DcPin { get; private set; }
    public int CsPin { get; private set; }
    public int BusyPin { get; private set; }
    public SpiDevice SpiDevice { get; private set; }
    public GpioController GpioController { get; private set; }
    private static readonly object _syncLock = new object();

    public Connections(IHardwareSpecification specification)
    {
        _specification = specification;
    }

    public void Initialize()
    {
        lock (_syncLock)
        {
            GpioController = new GpioController();

            ResetPin = _specification.RST_PIN;
            GpioController.OpenPin(ResetPin, PinMode.Output);

            DcPin = _specification.DC_PIN;
            GpioController.OpenPin(DcPin, PinMode.Output);

            CsPin = _specification.CS_PIN;
            GpioController.OpenPin(CsPin, PinMode.Output);

            BusyPin = _specification.BUSY_PIN;
            GpioController.OpenPin(BusyPin, PinMode.Input);

            var spiSettings = new SpiConnectionSettings(0, CsPin)
            {
                ClockFrequency = _specification.Channel0Frequency,
                Mode = SpiMode.Mode0
            };
            SpiDevice = SpiDevice.Create(spiSettings);
        }
    }

    public void Dispose()
    {
        SpiDevice?.Dispose();
        GpioController?.Dispose();
        GC.SuppressFinalize(this);
    }
}