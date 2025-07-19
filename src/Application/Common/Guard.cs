namespace BaseTemplate.Application.Common;
public static class Guard
{
    public static void AgainstNull<T>(T argument)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(nameof(argument), $"{nameof(argument)} cannot be null.");
        }
    }
    public static void AgainstNull<T>(T argument, string argumentName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(argumentName, $"{argumentName} cannot be null.");
        }
    }

    public static void AgainstNotNull<T>(T argument)
    {
        if (argument != null)
        {
            throw new ArgumentException($"{nameof(argument)} should be null.", nameof(argument));
        }
    }

    public static void AgainstNotNull<T>(T argument, string argumentName)
    {
        if (argument != null)
        {
            throw new ArgumentException($"{argumentName} should be null.", argumentName);
        }
    }

    public static void AgainstEmptyOrNullString(string argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException($"{argumentName} cannot be null or empty.", argumentName);
        }
    }

    public static void AgainstNegativeNumber(int value, string argumentName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} cannot be negative.");
        }
    }

    public static void AgainstNegativeNumber(decimal value, string argumentName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} cannot be negative.");
        }
    }
}

