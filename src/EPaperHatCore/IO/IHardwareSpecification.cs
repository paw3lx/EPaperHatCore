namespace EPaperHatCore.IO;

public interface IHardwareSpecification
{
    int RST_PIN { get; }
    int DC_PIN { get; }
    int CS_PIN { get; }
    int BUSY_PIN { get; }
    int Channel0Frequency { get; }
}