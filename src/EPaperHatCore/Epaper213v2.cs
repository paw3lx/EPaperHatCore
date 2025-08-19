using System;
using System.Threading;
using System.Device.Gpio;
using EPaperHatCore.GUI;
using EPaperHatCore.IO;

namespace EPaperHatCore;
public class Epaper213v2 : EpaperBase
{
    public Epaper213v2(int screenWidth, int screenHeight, IHardwareSpecification specification = null) : base(screenWidth, screenHeight, specification)
    {
    }

    public override void ClearScreen()
    {
        int Width, Height;
        Width = (ScreenWidth % 8 == 0) ? (ScreenWidth / 8) : (ScreenWidth / 8 + 1);
        Height = ScreenHeight;

        _ePaperConnection.SendCommand(0x24);
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                _ePaperConnection.SendData(0xFF);
            }
        }

        TurnOnDisplay();
    }

    public override void DisplayScreens(params Screen[] screens)
    {
        if (screens.Length == 0)
            return;
        var screen = screens[0];

        int Width, Height;
        Width = (ScreenWidth % 8 == 0) ? (ScreenWidth / 8) : (ScreenWidth / 8 + 1);
        Height = ScreenHeight;

        _ePaperConnection.SendCommand(0x24);
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                _ePaperConnection.SendData(screen.Image[i + j * Width]);
            }
        }
        TurnOnDisplay();
    }

    public override void Initialize()
    {
        _connections.Initialize();
        Reset();

        WaitUntilIdle();
        _ePaperConnection.SendCommand(0x12); // soft reset
        WaitUntilIdle();

        _ePaperConnection.SendCommand(0x74); //set analog block control
        _ePaperConnection.SendData(0x54);
        _ePaperConnection.SendCommand(0x7E); //set digital block control
        _ePaperConnection.SendData(0x3B);

        _ePaperConnection.SendCommand(0x01); //Driver output control
        _ePaperConnection.SendData(0xF9);
        _ePaperConnection.SendData(0x00);
        _ePaperConnection.SendData(0x00);

        _ePaperConnection.SendCommand(0x11); //data entry mode
        _ePaperConnection.SendData(0x01);

        _ePaperConnection.SendCommand(0x44); //set Ram-X address start/end position
        _ePaperConnection.SendData(0x00);
        _ePaperConnection.SendData(0x0F);    //0x0C-->(15+1)*8=128

        _ePaperConnection.SendCommand(0x45); //set Ram-Y address start/end position
        _ePaperConnection.SendData(0xF9);   //0xF9-->(249+1)=250
        _ePaperConnection.SendData(0x00);
        _ePaperConnection.SendData(0x00);
        _ePaperConnection.SendData(0x00);

        _ePaperConnection.SendCommand(0x3C); //BorderWavefrom
        _ePaperConnection.SendData(0x03);

        _ePaperConnection.SendCommand(0x2C); //VCOM Voltage
        _ePaperConnection.SendData(0x55); //

        _ePaperConnection.SendCommand(0x03);
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[70]);

        _ePaperConnection.SendCommand(0x04); //
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[71]);
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[72]);
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[73]);

        _ePaperConnection.SendCommand(0x3A);     //Dummy Line
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[74]);
        _ePaperConnection.SendCommand(0x3B);     //Gate time
        _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[75]);

        _ePaperConnection.SendCommand(0x32);
        for (int count = 0; count < 70; count++)
        {
            _ePaperConnection.SendData(EPD_2IN13_V2_lut_full_update[count]);
        }

        _ePaperConnection.SendCommand(0x4E);   // set RAM x address count to 0;
        _ePaperConnection.SendData(0x00);
        _ePaperConnection.SendCommand(0x4F);   // set RAM y address count to 0X127;
        _ePaperConnection.SendData(0xF9);
        _ePaperConnection.SendData(0x00);
        WaitUntilIdle();

    }

    public override void Reset()
    {
        _connections.GpioController.Write(_connections.ResetPin, PinValue.High);
        Thread.Sleep(200);
        _connections.GpioController.Write(_connections.ResetPin, PinValue.Low);
        Thread.Sleep(10);
        _connections.GpioController.Write(_connections.ResetPin, PinValue.High);
        Thread.Sleep(200);
    }

    public override void Sleep()
    {
        _ePaperConnection.SendCommand(0x10);
        _ePaperConnection.SendData(0x01);
        Thread.Sleep(100);
    }

    public override void WaitUntilIdle()
    {
        while (_connections.GpioController.Read(_connections.BusyPin) == PinValue.High)
        {      //LOW: idle, HIGH: busy
            Thread.Sleep(100);
        }
    }

    private void TurnOnDisplay()
    {
        _ePaperConnection.SendCommand(0x22);
        _ePaperConnection.SendData(0xC7);
        _ePaperConnection.SendCommand(0x20);
        WaitUntilIdle();
    }

    private static int[] EPD_2IN13_V2_lut_full_update = new int[76]{
            0x80,0x60,0x40,0x00,0x00,0x00,0x00,             //LUT0: BB:     VS 0 ~7
            0x10,0x60,0x20,0x00,0x00,0x00,0x00,             //LUT1: BW:     VS 0 ~7
            0x80,0x60,0x40,0x00,0x00,0x00,0x00,             //LUT2: WB:     VS 0 ~7
            0x10,0x60,0x20,0x00,0x00,0x00,0x00,             //LUT3: WW:     VS 0 ~7
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,             //LUT4: VCOM:   VS 0 ~7

            0x03,0x03,0x00,0x00,0x02,                       // TP0 A~D RP0
            0x09,0x09,0x00,0x00,0x02,                       // TP1 A~D RP1
            0x03,0x03,0x00,0x00,0x02,                       // TP2 A~D RP2
            0x00,0x00,0x00,0x00,0x00,                       // TP3 A~D RP3
            0x00,0x00,0x00,0x00,0x00,                       // TP4 A~D RP4
            0x00,0x00,0x00,0x00,0x00,                       // TP5 A~D RP5
            0x00,0x00,0x00,0x00,0x00,                       // TP6 A~D RP6

            0x15,0x41,0xA8,0x32,0x30,0x0A,
        };
}