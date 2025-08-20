namespace EPaperHatCore.IO;

public class DefaultSpecification : IHardwareSpecification
{
    public int RST_PIN => 17;
    public int DC_PIN => 25;
    public int CS_PIN => 8;
    public int BUSY_PIN => 24;
    public int Channel0Frequency => 2000000;
}