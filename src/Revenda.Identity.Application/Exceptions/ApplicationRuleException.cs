namespace Revenda.Identity.Application.Exceptions;

public abstract class ApplicationRuleException : Exception
{
    protected ApplicationRuleException(string message) : base(message)
    {
    }
}
