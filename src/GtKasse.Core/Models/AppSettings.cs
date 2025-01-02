using System.Reflection;

namespace GtKasse.Core.Models;

public sealed class AppSettings
{
    public string Version { get; set; }
    public required string HeaderTitle { get; set; }
    public required string Slogan { get; set; }
    public required string[] InvoiceSender { get; set; }

    public AppSettings()
    {
        Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1";
    }
}
