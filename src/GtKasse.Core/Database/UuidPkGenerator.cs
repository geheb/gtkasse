using System.Security.Cryptography;

namespace GtKasse.Core.Database;

public sealed class UuidPkGenerator
{
    public Guid Generate() => Guid.CreateVersion7(DateTimeOffset.UtcNow);
}
