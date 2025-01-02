namespace GtKasse.Core.Models;

public sealed class FirebaseCloudMessagingSettings
{
    public required string ApiKey { get; set; }
    public required string ProjectId { get; set; }
    public required string MessagingSenderId { get; set; }
    public required string AppId { get; set; }
    public required string VapidKey { get; set; }

    public bool HasConfig =>
        !string.IsNullOrEmpty(ApiKey) &&
        !string.IsNullOrEmpty(ProjectId) &&
        !string.IsNullOrEmpty(MessagingSenderId) &&
        !string.IsNullOrEmpty(AppId) &&
        !string.IsNullOrEmpty(VapidKey);
}
