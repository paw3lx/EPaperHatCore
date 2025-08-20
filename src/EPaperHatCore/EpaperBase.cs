using System;
using EPaperHatCore.GUI;
using EPaperHatCore.IO;

namespace EPaperHatCore;
public abstract class EpaperBase : IEpaper
{
    public int ScreenWidth { get; }
    public int ScreenHeight { get; }

    protected readonly IEpaperConnection _ePaperConnection;
    protected readonly Connections _connections;

    public EpaperBase(int screenWidth, int screenHeight, IHardwareSpecification specification = null)
    {
        if (screenWidth <= 0 || screenHeight <= 0)
        {
            throw new ArgumentException("Width and/or height cannot be less or equal zero");
        }
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;

        _connections = new Connections(specification ?? new DefaultSpecification());
        _ePaperConnection = new EPaperConnection(_connections);
    }

    public abstract void Initialize();
    public abstract void ClearScreen();
    public abstract void WaitUntilIdle();
    public abstract void DisplayScreens(params Screen[] screens);
    public abstract void Sleep();
    public abstract void Reset();
}