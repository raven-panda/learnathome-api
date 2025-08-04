namespace LearnAtHomeApi._Core.Repository;

public interface IRepositoryBase<TModel>
{
    TModel? Get(int? id);
    bool Exists(int? id);
    TModel Add(TModel item);
    int Remove(int id);
    TModel Update(TModel item);
}

public interface IAuditableRepositoryBase<TModel>
{
    TModel? Get(int? id);
    bool Exists(int? id);
    TModel Add(TModel item, int createdById);
    int Remove(int id);
    TModel Update(TModel item, int updatedById);
}
