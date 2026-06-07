
namespace DVLD.Business.Common.Validation
{
    public static class Guard
    {
        public static void AgainstNull(object? value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null");
        }

        public static void AgainstNonPositive(int value, string paramName)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
        }

        public static void AgainstInvalidEnum<TEnum>(int value, string paramName)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), Enum.ToObject(typeof(TEnum), value)))
                throw new ArgumentOutOfRangeException(paramName, $"{value} is not a valid {typeof(TEnum).Name}");
        }

        public static void AgainstNullOrWhiteSpace(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }

        public static void AgainstInvalidFees(Decimal value,  string paramName, decimal minValue = 5)
        {
            if (value < minValue)
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be at least {minValue}.");
        }
    }
}

