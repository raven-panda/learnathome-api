using LearnAtHomeApi._Core.Repository;
using LearnAtHomeApi.StudentTask.Model;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Model;
using LearnAtHomeApi.User.Repository;
using Microsoft.EntityFrameworkCore;

namespace LearnAtHomeApi.StudentTask.Repository;

public interface IStudentTaskRepository : IAuditableRepositoryBase<StudentTaskModel>
{
    IEnumerable<StudentTaskModel> GetAllByMentorId(int id);
    IEnumerable<StudentTaskModel> GetAllByAttributedStudentId(int id);
}

internal sealed class StudentTaskRepositoryImp(AppDbContext context) : IStudentTaskRepository
{
    public IEnumerable<StudentTaskModel> GetAllByMentorId(int id)
    {
        return context
            .StudentTasks.Where(t => t.Mentor.Id == id)
            .Include(t => t.Mentor)
            .Include(t => t.CreatedBy)
            .Include(t => t.UpdatedBy)
            .Select(FormatTaskModelForGetAll)
            .ToList();
    }

    public IEnumerable<StudentTaskModel> GetAllByAttributedStudentId(int id)
    {
        return context
            .StudentTasks.Where(t => t.AttributedUserId == id)
            .Include(t => t.Mentor)
            .Include(t => t.CreatedBy)
            .Include(t => t.UpdatedBy)
            .Select(FormatTaskModelForGetAll)
            .ToList();
    }

    private StudentTaskModel FormatTaskModelForGetAll(StudentTaskModel t)
    {
        var taskModel = new StudentTaskModel();
        taskModel.Id = t.Id;
        taskModel.Name = t.Name;
        taskModel.AttributedUserId = t.AttributedUserId;
        taskModel.Mentor = new RpUserModel
        {
            Id = t.Mentor.Id,
            Username = t.Mentor.Username,
            Role = t.Mentor.Role,
        };
        taskModel.Description = t.Description;
        taskModel.EndDate = t.EndDate;
        taskModel.CreatedAt = t.CreatedAt;
        taskModel.UpdatedAt = t.UpdatedAt;
        taskModel.CreatedBy = new RpUserModel
        {
            Id = t.CreatedBy.Id,
            Username = t.CreatedBy.Username,
            Role = t.CreatedBy.Role,
        };
        taskModel.UpdatedBy = new RpUserModel
        {
            Id = t.UpdatedBy.Id,
            Username = t.UpdatedBy.Username,
            Role = t.UpdatedBy.Role,
        };
        return taskModel;
    }

    public StudentTaskModel? Get(int? id)
    {
        return context.StudentTasks.Find(id);
    }

    public bool Exists(int? id)
    {
        return context.StudentTasks.Any(item => item.Id == id);
    }

    public StudentTaskModel Add(StudentTaskModel item, int createdById)
    {
        var user = context.Users.Find(createdById);
        if (user is null)
            throw new BadHttpRequestException("User does not exist");

        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        item.CreatedBy = user;
        item.UpdatedBy = user;

        var mentor =
            user.Role == UserRole.Mentor
                ? user
                : context
                    .Users.Include(u => u.Mentor)
                    .FirstOrDefault(u => user.Mentor != null && u.Id == user.Mentor.Id);

        if (mentor == null)
            throw new BadHttpRequestException("Mentor does not exist");

        item.Mentor = mentor;

        context.StudentTasks.Add(item);
        context.SaveChanges();
        return Get(item.Id)!;
    }

    public int Remove(int id)
    {
        var entity = context.StudentTasks.Find(id)!;

        context.StudentTasks.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public StudentTaskModel Update(StudentTaskModel item, int updatedById)
    {
        var user = context.Users.Find(updatedById);
        if (user is null)
            throw new BadHttpRequestException("User does not exist");

        var existing = context.StudentTasks.Find(item.Id)!;

        item.CreatedAt = existing.CreatedAt;
        item.UpdatedAt = DateTime.Now;
        // TODO : add auth to implement this
        item.CreatedBy = context
            .Users.Include(u => u.Mentor)
            .First(u => u.Id == existing.CreatedBy.Id);
        item.UpdatedBy = user;

        context.StudentTasks.Update(item);
        context.SaveChanges();
        return item;
    }
}
