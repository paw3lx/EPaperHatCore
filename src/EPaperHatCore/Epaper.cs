using System;
using System.IO;
using System.Threading;
using BetaSoft.EPaperHatCore.GUI;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace BetaSoft.EPaperHatCore
{
    public class Epaper
    {
        public Epaper(int screenWidth, int screenHeight)
        {
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                throw new ArgumentException("Width and/or height cannot be less or equal zero");
            }
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public void Initialize()
        {
            Reset();

            SendCommand(HardwareCodes.POWER_ON);
            WaitUntilIdle();

            SendCommand(HardwareCodes.PANEL_SETTING);
            SendData(0xaf);       //KW-BF   KWR-AF    BWROTP 0f
        
            SendCommand(HardwareCodes.PLL_CONTROL);
            SendData(0x3a);      //3A 100HZ   29 150Hz 39 200HZ    31 171HZ

            SendCommand(HardwareCodes.POWER_SETTING);
            SendData(0x03);                  //# VDS_EN, VDG_EN
            SendData(0x00);                 //# VCOM_HV, VGHL_LV[1], VGHL_LV[0]
            SendData(0x2b);                  //# VDH
            SendData(0x2b);                  //# VDL
            SendData(0x09);                  //# VDHR

            SendCommand(HardwareCodes.BOOSTER_SOFT_START);
            SendData(0x07);
            SendData(0x07);
            SendData(0x17);

            //Power optimization
            SendCommand(0xF8);
            SendData(0x60);
            SendData(0xA5);

            //Power optimization
            SendCommand(0xF8);
            SendData(0x89);
            SendData(0xA5);

            //Power optimization
            SendCommand(0xF8);
            SendData(0x90);
            SendData(0x00);
            
            //Power optimization
            SendCommand(0xF8);
            SendData(0x93);
            SendData(0x2A);

            //Power optimization
            SendCommand(0xF8);
            SendData(0x73);
            SendData(0x41);

            SendCommand(HardwareCodes.VCM_DC_SETTING_REGISTER);
            SendData(0x12);                   
            SendCommand(HardwareCodes.VCOM_AND_DATA_INTERVAL_SETTING);
            SendData(0x87);       //define by OTP

            SetLut();

            SendCommand(HardwareCodes.PARTIAL_DISPLAY_REFRESH);
            SendData(0x00);
        }

        private void SetLut()
        {
            int count;     
            SendCommand(HardwareCodes.LUT_FOR_VCOM);                            //vcom
            for(count = 0; count < 44; count++) {
                SendData(HardwareCodes.lut_vcom_dc[count]);
            }
            
            SendCommand(HardwareCodes.LUT_WHITE_TO_WHITE);                      //ww --
            for(count = 0; count < 42; count++) {
                SendData(HardwareCodes.lut_ww[count]);
            }   
            
            SendCommand(HardwareCodes.LUT_BLACK_TO_WHITE);                      //bw r
            for(count = 0; count < 42; count++) {
                SendData(HardwareCodes.lut_bw[count]);
            } 

            SendCommand(HardwareCodes.LUT_WHITE_TO_BLACK);                      //wb w
            for(count = 0; count < 42; count++) {
                SendData(HardwareCodes.lut_bb[count]);
            } 

            SendCommand(HardwareCodes.LUT_BLACK_TO_BLACK);                      //bb b
            for(count = 0; count < 42; count++) {
                SendData(HardwareCodes.lut_wb[count]);
            } 
        }

        private void SendCommand(int hex)
        {
            Connections.DcPin.Write(GpioPinValue.Low);
            Connections.CsPin.Write(GpioPinValue.Low);
            Pi.Spi.Channel0.SendReceive(BitConverter.GetBytes(hex));
            Connections.CsPin.Write(GpioPinValue.High);
        }

        private void SendData(int hex)
        {
            Connections.DcPin.Write(GpioPinValue.High);
            Connections.CsPin.Write(GpioPinValue.Low);
            Pi.Spi.Channel0.SendReceive(BitConverter.GetBytes(hex));
            Connections.CsPin.Write(GpioPinValue.High);
        }

        private void Reset()
        {
            Connections.ResetPin.Write(true);
            Thread.Sleep(200);
            Connections.ResetPin.Write(false);
            Thread.Sleep(200);
            Connections.ResetPin.Write(true);
        }

        private void WaitUntilIdle()
        {
            Console.WriteLine("e-Paper busy");
            while(Connections.BusyPin.Read() == false)
            {
                System.Threading.Thread.Sleep(100);
            }
            Console.WriteLine("e-Paper busy release");
        }

        public void EPD_Clear()
        {    
            int Width, Height;
            Width = (ScreenWidth % 8 == 0)? (ScreenWidth / 8 ): (ScreenHeight / 8 + 1);
            Height = ScreenHeight;

            SendCommand(HardwareCodes.DATA_START_TRANSMISSION_1);
            for (int j = 0; j < Height; j++) {
                for (int i = 0; i < Width; i++) {
                    SendData(0x00);
                }
            }
            SendData(HardwareCodes.DATA_STOP);

            SendCommand(HardwareCodes.DATA_START_TRANSMISSION_2);
            for (int j = 0; j < Height; j++) {
                for (int i = 0; i < Width; i++) {
                    SendData(0x00);
                }
            }
            SendData(HardwareCodes.DATA_STOP);
            
            SendCommand(HardwareCodes.DISPLAY_REFRESH);
            WaitUntilIdle();
        }

        public void DispalScreens(Screen blackScreen, Screen redScreen)
        {
            if (blackScreen?.Image == null)
                throw new ArgumentNullException(nameof(blackScreen));
            if (redScreen?.Image == null)
                throw new ArgumentNullException(nameof(redScreen));

            DispalScreen(blackScreen, HardwareCodes.DATA_START_TRANSMISSION_1);
            DispalScreen(redScreen, HardwareCodes.DATA_START_TRANSMISSION_2);
            
            SendCommand(HardwareCodes.DISPLAY_REFRESH);
            WaitUntilIdle();
        }

        private void DispalScreen(Screen screen, int dataTransmissionCode)
        {
            int width, height;
            width = (ScreenWidth % 8 == 0)? (ScreenWidth/ 8 ): (ScreenHeight / 8 + 1);
            height = ScreenHeight;
            
            SendCommand(dataTransmissionCode);
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    SendData(~screen.Image[i + j * width]);
                }
            }
            SendData(HardwareCodes.DATA_STOP);
        }

        public void Sleep()
        {
            SendCommand(0X50);
            SendData(0xf7);
            SendCommand(0X02);  	//power off
            SendCommand(0X07);  	//deep sleep
            SendData(0xA5);
        }
    }
}
