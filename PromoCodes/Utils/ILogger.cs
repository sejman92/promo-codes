using FluentValidation.Results;

namespace PromoCodes.Utils;

public interface ILogger
{
    void Log(string message);
    void LogErrors(string location, IEnumerable<ValidationFailure> validationErrors);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }

    public void LogErrors(string location, IEnumerable<ValidationFailure> validationErrors)
    {
        Console.WriteLine($"{location}:");
        foreach (var error in validationErrors)
        {
            Console.WriteLine($"{error.ErrorMessage}");
        }
    }
}