using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Annotations;

/// <summary>
/// min 7 and max 256 chars
/// </summary>
public sealed class EmailLengthFieldAttribute : StringLengthAttribute
{
    public EmailLengthFieldAttribute() : base(256)
    {
        MinimumLength = 7;
        ErrorMessage = "Das Feld '{0}' muss mindestens {2} und höchstens {1} Zeichen enthalten.";
    }
}
