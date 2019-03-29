namespace BetaSoft.EPaperHatCore.IO
{
    public interface IEpaperConnection
    {
        void SendCommand(int command);
        void SendData(int data);
    }
}
