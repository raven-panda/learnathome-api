namespace LearnAtHomeApi.Repository;

public interface IRepositoryBase<TModel>
{
    TModel Get(int id);
    bool Exists(int id);
    TModel Add(TModel item);
    int Remove(int id);
    TModel Update(TModel item);
}
