namespace LearnAtHomeApi.Services;

public interface IService<TDto, TModel>
{
    TDto Get(int id);
    TDto Add(TDto item);
    int Remove(int id);
    TDto Update(TDto item);
    TDto ToDto(TModel model);
    public TModel ToModel(TDto dto);
}