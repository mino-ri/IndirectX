namespace IndirectX.ImageEmbedder;

public class ImageEmbedSetting
{
    public string Namespace { get; set; } = "";

    public string ClassName { get; set; } = "";

    public static ImageEmbedSetting Sample => new()
    {
        Namespace = "CSharpNamespace",
        ClassName = "CSharpClassName",
    };
}
