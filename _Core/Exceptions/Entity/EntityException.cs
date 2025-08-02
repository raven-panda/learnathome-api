namespace LearnAtHomeApi._Core.Exceptions.Entity;

public sealed class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, object? id)
        : base($"{entityName} with id {id} not found") { }

    public EntityNotFoundException(string entityName, string fieldName, object? id)
        : base($"{entityName} with {fieldName} {id} not found") { }
};

public sealed class EntityUniqueConstraintViolationException(string entityName, string fieldName)
    : ApplicationException(
        $"An existing record {entityName} already uses the given value for {fieldName}"
    );
