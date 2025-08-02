namespace LearnAtHomeApi._Core.Exceptions.Entity;

public sealed class PasswordsNotMatchingException()
    : ApplicationException("The given password doesn't match with confirmation password.");
