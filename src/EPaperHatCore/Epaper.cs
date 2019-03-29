using System;
using System.IO;
using System.Threading;
using BetaSoft.EPaperHatCore.GUI;
using BetaSoft.EPaperHatCore.IO;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace BetaSoft.EPaperHatCore
{
    public class Epaper
    {
        private readonly IEpaperConnection _connection;
        public Epaper(int screenWidth, int screenHeight)
        {
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                throw new ArgumentException("Width and/or height cannot be less or equal zero");
            }
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _connection = new EPaperConnection();
        }

        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public void Initialize()
        {
            Reset();

            _connection.SendCommand(HardwareCodes.POWER_ON);
            WaitUntilIdle();

            _connection.SendCommand(HardwareCodes.PANEL_SETTING);
            _connection.SendData(0xaf);       //KW-BF   KWR-AF    BWROTP 0f
        
            _connection.SendCommand(HardwareCodes.PLL_CONTROL);
            _connection.SendData(0x3a);      //3A 100HZ   29 150Hz 39 200HZ    31 171HZ

            _connection.SendCommand(HardwareCodes.POWER_SETTING);
            _connection.SendData(0x03);                  //# VDS_EN, VDG_EN
            _connection.SendData(0x00);                 //# VCOM_HV, VGHL_LV[1], VGHL_LV[0]
            _connection.SendData(0x2b);                  //# VDH
            _connection.SendData(0x2b);                  //# VDL
            _connection.SendData(0x09);                  //# VDHR

            _connection.SendCommand(HardwareCodes.BOOSTER_SOFT_START);
            _connection.SendData(0x07);
            _connection.SendData(0x07);
            _connection.SendData(0x17);

            //Power optimization
            _connection.SendCommand(0xF8);
            _connection.SendData(0x60);
            _connection.SendData(0xA5);

            //Power optimization
            _connection.SendCommand(0xF8);
            _connection.SendData(0x89);
            _connection.SendData(0xA5);

            //Power optimization
            _connection.SendCommand(0xF8);
            _connection.SendData(0x90);
            _connection.SendData(0x00);
            
            //Power optimization
            _connection.SendCommand(0xF8);
            _connection.SendData(0x93);
            _connection.SendData(0x2A);

            //Power optimization
            _connection.SendCommand(0xF8);
            _connection.SendData(0x73);
            _connection.SendData(0x41);

            _connection.SendCommand(HardwareCodes.VCM_DC_SETTING_REGISTER);
            _connection.SendData(0x12);                   
            _connection.SendCommand(HardwareCodes.VCOM_AND_DATA_INTERVAL_SETTING);
            _connection.SendData(0x87);       //define by OTP

            SetLut();

            _connection.SendCommand(HardwareCodes.PARTIAL_DISPLAY_REFRESH);
            _connection.SendData(0x00);
        }

        private void SetLut()
        {
            int count;     
            _connection.SendCommand(HardwareCodes.LUT_FOR_VCOM);                            //vcom
            for(count = 0; count < 44; count++) {
                _connection.SendData(HardwareCodes.lut_vcom_dc[count]);
            }
            
            _connection.SendCommand(HardwareCodes.LUT_WHITE_TO_WHITE);                      //ww --
            for(count = 0; count < 42; count++) {
                _connection.SendData(HardwareCodes.lut_ww[count]);
            }   
            
            _connection.SendCommand(HardwareCodes.LUT_BLACK_TO_WHITE);                      //bw r
            for(count = 0; count < 42; count++) {
                _connection.SendData(HardwareCodes.lut_bw[count]);
            } 

            _connection.SendCommand(HardwareCodes.LUT_WHITE_TO_BLACK);                      //wb w
            for(count = 0; count < 42; count++) {
                _connection.SendData(HardwareCodes.lut_bb[count]);
            } 

            _connection.SendCommand(HardwareCodes.LUT_BLACK_TO_BLACK);                      //bb b
            for(count = 0; count < 42; count++) {
                _connection.SendData(HardwareCodes.lut_wb[count]);
            } 
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

        public void ClearScreen()
        {    
            int Width, Height;
            Width = (ScreenWidth % 8 == 0)? (ScreenWidth / 8 ): (ScreenHeight / 8 + 1);
            Height = ScreenHeight;

            _connection.SendCommand(HardwareCodes.DATA_START_TRANSMISSION_1);
            for (int j = 0; j < Height; j++) {
                for (int i = 0; i < Width; i++) {
                    _connection.SendData(0x00);
                }
            }
            _connection.SendData(HardwareCodes.DATA_STOP);

            _connection.SendCommand(HardwareCodes.DATA_START_TRANSMISSION_2);
            for (int j = 0; j < Height; j++) {
                for (int i = 0; i < Width; i++) {
                    _connection.SendData(0x00);
                }
            }
            _connection.SendData(HardwareCodes.DATA_STOP);
            
            _connection.SendCommand(HardwareCodes.DISPLAY_REFRESH);
            WaitUntilIdle();
        }

        public void DisplayScreens(Screen blackScreen, Screen redScreen)
        {
            if (blackScreen?.Image == null)
                throw new ArgumentNullException(nameof(blackScreen));
            if (redScreen?.Image == null)
                throw new ArgumentNullException(nameof(redScreen));

            DispalScreen(blackScreen, HardwareCodes.DATA_START_TRANSMISSION_1);
            DispalScreen(redScreen, HardwareCodes.DATA_START_TRANSMISSION_2);
            
            _connection.SendCommand(HardwareCodes.DISPLAY_REFRESH);
            WaitUntilIdle();
        }

        private void DispalScreen(Screen screen, int dataTransmissionCode)
        {
            int width, height;
            width = (ScreenWidth % 8 == 0)? (ScreenWidth/ 8 ): (ScreenHeight / 8 + 1);
            height = ScreenHeight;

            _connection.SendCommand(dataTransmissionCode);
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    _connection.SendData(~screen.Image[i + j * width]);
                }
            }
            _connection.SendData(HardwareCodes.DATA_STOP);
        }

        public void Sleep()
        {
            _connection.SendCommand(0X50);
            _connection.SendData(0xf7);
            _connection.SendCommand(0X02); //power off
            _connection.SendCommand(0X07); //deep sleep
            _connection.SendData(0xA5);
        }
    }
}
