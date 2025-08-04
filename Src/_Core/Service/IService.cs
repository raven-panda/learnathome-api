namespace LearnAtHomeApi._Core.Service;

public interface IService<TDto, TModel>
{
    TDto Get(int id);
    TDto Add(TDto item);
    int Remove(int id);
    TDto Update(TDto item);
    TDto ToDto(TModel model);
    public TModel ToModel(TDto dto);
}

public interface IAuditableService<TDto, TModel, in TCreateDto, in TUpdateDto>
{
    TDto Get(int id);
    TDto Add(TCreateDto item, int createdById);
    int Remove(int id);
    TDto Update(TUpdateDto item, int updatedById);
    TDto ToDto(TModel model);
    public TModel ToModelForCreation(TCreateDto dto);
    public TModel ToModelForUpdate(TUpdateDto dto);
}
