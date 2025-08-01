namespace LearnAtHomeApi._Core.Exceptions.Entity;

public class EntityNotFoundException(string entityName, object id)
    : ApplicationException($"{entityName} with id {id} not found");
