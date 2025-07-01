using GtKasse.Core.Converter;

namespace GtKasse.Core.Entities;

public interface IDtoMapper<TModel> 
    where TModel : struct, IDto
{
    TModel ToDto(GermanDateTimeConverter dc);
    void FromDto(TModel model);
}
