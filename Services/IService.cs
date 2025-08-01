namespace LearnAtHomeApi.Services;

public interface IService<TDto, TModel>
{
    TDto ToDto(TModel model);
    public TModel ToModel(TDto dto);
}