namespace LearnAtHomeApi._Core.Exceptions.Entity;

public class EntityNotFoundException(string entityName, object? id)
    : ApplicationException($"{entityName} with id {id} not found");

public class EntityUniqueConstraintViolationException(string entityName, string fieldName)
    : ApplicationException(
        $"An existing record {entityName} already uses the given value for {fieldName}"
    );
