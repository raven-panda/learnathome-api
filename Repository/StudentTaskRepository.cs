using LearnAtHomeApi.Models;

namespace LearnAtHomeApi.Repository;

public interface IStudentTaskRepository
{
    IEnumerable<StudentTaskModel> GetAllByUserId(int id);
    StudentTaskModel? Get(int id);
    StudentTaskModel Add(StudentTaskModel item);
    int Remove(int id);
    StudentTaskModel Update(StudentTaskModel item);
}

public class StudentTaskRepositoryImp(AppDbContext context) : IStudentTaskRepository
{
    public IEnumerable<StudentTaskModel> GetAllByUserId(int id)
    {
        return context.StudentTasks.Where(p => p.AttributedUserId == id);
    }

    public StudentTaskModel? Get(int id)
    {
        return context.StudentTasks.Find(id);
    }

    public StudentTaskModel Add(StudentTaskModel item)
    {
        context.StudentTasks.Add(item);
        context.SaveChanges();
        return item;
    }

    public int Remove(int id)
    {
        var entity = context.StudentTasks.Find(id);
        if (entity == null)
            throw new InvalidOperationException($"Entity with id {id} not found");

        context.StudentTasks.Remove(entity);
        return entity.Id;
    }

    public StudentTaskModel Update(StudentTaskModel item)
    {
        var exists = context.StudentTasks.Any(e => e.Id == item.Id);
        if (!exists)
            throw new InvalidOperationException();

        context.StudentTasks.Update(item);
        context.SaveChanges();
        return item;
    }
}