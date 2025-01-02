using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Annotations;

public sealed class CompareFieldAttribute : CompareAttribute
{
    public CompareFieldAttribute(string otherProperty) : base(otherProperty)
    {
        ErrorMessage = "Die Felder '{0}' und '{1}' stimmen nicht überein.";
    }
}
