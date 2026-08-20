namespace RecipeManager.Application.Common.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";

    public string[] Origins { get; set; } = [];
}
