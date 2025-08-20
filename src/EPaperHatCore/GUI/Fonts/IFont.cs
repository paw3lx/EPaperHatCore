namespace EPaperHatCore.GUI.Fonts;

public interface IFont
{
    uint Width { get; }
    uint Height { get; }
    char[] Table { get; }
}
