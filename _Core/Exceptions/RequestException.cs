namespace LearnAtHomeApi._Core.Exceptions;

public sealed class PasswordsNotMatchingException()
    : ApplicationException("The given password doesn't match with confirmation password.");

public sealed class InvalidCredentialsException()
    : ApplicationException("The given credentials doesn't match any account.");
