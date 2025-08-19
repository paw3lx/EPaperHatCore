﻿using System;
using System.Threading;
using System.Device.Gpio;
using EPaperHatCore.GUI;
using EPaperHatCore.IO;

namespace EPaperHatCore;
public class Epaper27 : EpaperBase
{
    public Epaper27(int screenWidth, int screenHeight, IHardwareSpecification specification = null)
        : base(screenWidth, screenHeight, specification)
    {

    }

    public override void Initialize()
    {
        _connections.Initialize();
        Reset();

        _ePaperConnection.SendCommand(HardwareCodes.POWER_ON);
        WaitUntilIdle();

        _ePaperConnection.SendCommand(HardwareCodes.PANEL_SETTING);
        _ePaperConnection.SendData(0xaf);       //KW-BF   KWR-AF    BWROTP 0f

        _ePaperConnection.SendCommand(HardwareCodes.PLL_CONTROL);
        _ePaperConnection.SendData(0x3a);      //3A 100HZ   29 150Hz 39 200HZ    31 171HZ

        _ePaperConnection.SendCommand(HardwareCodes.POWER_SETTING);
        _ePaperConnection.SendData(0x03);                  //# VDS_EN, VDG_EN
        _ePaperConnection.SendData(0x00);                 //# VCOM_HV, VGHL_LV[1], VGHL_LV[0]
        _ePaperConnection.SendData(0x2b);                  //# VDH
        _ePaperConnection.SendData(0x2b);                  //# VDL
        _ePaperConnection.SendData(0x09);                  //# VDHR

        _ePaperConnection.SendCommand(HardwareCodes.BOOSTER_SOFT_START);
        _ePaperConnection.SendData(0x07);
        _ePaperConnection.SendData(0x07);
        _ePaperConnection.SendData(0x17);

        //Power optimization
        _ePaperConnection.SendCommand(0xF8);
        _ePaperConnection.SendData(0x60);
        _ePaperConnection.SendData(0xA5);

        //Power optimization
        _ePaperConnection.SendCommand(0xF8);
        _ePaperConnection.SendData(0x89);
        _ePaperConnection.SendData(0xA5);

        //Power optimization
        _ePaperConnection.SendCommand(0xF8);
        _ePaperConnection.SendData(0x90);
        _ePaperConnection.SendData(0x00);

        //Power optimization
        _ePaperConnection.SendCommand(0xF8);
        _ePaperConnection.SendData(0x93);
        _ePaperConnection.SendData(0x2A);

        //Power optimization
        _ePaperConnection.SendCommand(0xF8);
        _ePaperConnection.SendData(0x73);
        _ePaperConnection.SendData(0x41);

        _ePaperConnection.SendCommand(HardwareCodes.VCM_DC_SETTING_REGISTER);
        _ePaperConnection.SendData(0x12);
        _ePaperConnection.SendCommand(HardwareCodes.VCOM_AND_DATA_INTERVAL_SETTING);
        _ePaperConnection.SendData(0x87);       //define by OTP

        SetLut();

        _ePaperConnection.SendCommand(HardwareCodes.PARTIAL_DISPLAY_REFRESH);
        _ePaperConnection.SendData(0x00);
    }

    private void SetLut()
    {
        int count;
        _ePaperConnection.SendCommand(HardwareCodes.LUT_FOR_VCOM);                            //vcom
        for (count = 0; count < 44; count++)
        {
            _ePaperConnection.SendData(HardwareCodes.lut_vcom_dc[count]);
        }

        _ePaperConnection.SendCommand(HardwareCodes.LUT_WHITE_TO_WHITE);                      //ww --
        for (count = 0; count < 42; count++)
        {
            _ePaperConnection.SendData(HardwareCodes.lut_ww[count]);
        }

        _ePaperConnection.SendCommand(HardwareCodes.LUT_BLACK_TO_WHITE);                      //bw r
        for (count = 0; count < 42; count++)
        {
            _ePaperConnection.SendData(HardwareCodes.lut_bw[count]);
        }

        _ePaperConnection.SendCommand(HardwareCodes.LUT_WHITE_TO_BLACK);                      //wb w
        for (count = 0; count < 42; count++)
        {
            _ePaperConnection.SendData(HardwareCodes.lut_bb[count]);
        }

        _ePaperConnection.SendCommand(HardwareCodes.LUT_BLACK_TO_BLACK);                      //bb b
        for (count = 0; count < 42; count++)
        {
            _ePaperConnection.SendData(HardwareCodes.lut_wb[count]);
        }
    }

    public override void Reset()
    {
        _connections.GpioController.Write(_connections.ResetPin, PinValue.High);
        Thread.Sleep(200);
        _connections.GpioController.Write(_connections.ResetPin, PinValue.Low);
        Thread.Sleep(200);
        _connections.GpioController.Write(_connections.ResetPin, PinValue.High);
    }

    public override void WaitUntilIdle()
    {
        Console.WriteLine("e-Paper busy");
        while (_connections.GpioController.Read(_connections.BusyPin) == PinValue.Low)
        {
            System.Threading.Thread.Sleep(100);
        }
        Console.WriteLine("e-Paper busy release");
    }

    public override void ClearScreen()
    {
        int Width, Height;
        Width = (ScreenWidth % 8 == 0) ? (ScreenWidth / 8) : (ScreenHeight / 8 + 1);
        Height = ScreenHeight;

        _ePaperConnection.SendCommand(HardwareCodes.DATA_START_TRANSMISSION_1);
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                _ePaperConnection.SendData(0x00);
            }
        }
        _ePaperConnection.SendData(HardwareCodes.DATA_STOP);

        _ePaperConnection.SendCommand(HardwareCodes.DATA_START_TRANSMISSION_2);
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                _ePaperConnection.SendData(0x00);
            }
        }
        _ePaperConnection.SendData(HardwareCodes.DATA_STOP);

        _ePaperConnection.SendCommand(HardwareCodes.DISPLAY_REFRESH);
        WaitUntilIdle();
    }

    public override void DisplayScreens(params Screen[] screens)
    {
        if (screens.Length != 2)
            throw new ArgumentNullException(nameof(screens));

        var blackScreen = screens[0];
        var redScreen = screens[1];

        if (blackScreen?.Image == null)
            throw new ArgumentNullException(nameof(blackScreen));
        if (redScreen?.Image == null)
            throw new ArgumentNullException(nameof(redScreen));

        DispalScreen(blackScreen, HardwareCodes.DATA_START_TRANSMISSION_1);
        DispalScreen(redScreen, HardwareCodes.DATA_START_TRANSMISSION_2);

        _ePaperConnection.SendCommand(HardwareCodes.DISPLAY_REFRESH);
        WaitUntilIdle();
    }

    private void DispalScreen(Screen screen, int dataTransmissionCode)
    {
        int width, height;
        width = (ScreenWidth % 8 == 0) ? (ScreenWidth / 8) : (ScreenHeight / 8 + 1);
        height = ScreenHeight;

        _ePaperConnection.SendCommand(dataTransmissionCode);
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                _ePaperConnection.SendData(~screen.Image[i + j * width]);
            }
        }
        _ePaperConnection.SendData(HardwareCodes.DATA_STOP);
    }

    public override void Sleep()
    {
        _ePaperConnection.SendCommand(0X50);
        _ePaperConnection.SendData(0xf7);
        _ePaperConnection.SendCommand(0X02); //power off
        _ePaperConnection.SendCommand(0X07); //deep sleep
        _ePaperConnection.SendData(0xA5);
    }
}
