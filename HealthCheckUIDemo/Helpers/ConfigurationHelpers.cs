namespace HealthCheckUIDemo.Helpers
{
    public static class ConfigurationHelpers
    {
        private static IConfiguration? _configuration;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IConfiguration GetConfiguration()
        {
            return _configuration ?? throw new InvalidOperationException("Configuration is not set.");            
        }
        
        public static string GetSectionValueStr(string sectionPath)
        {
            var value = GetConfiguration().GetSection(sectionPath).Value;

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(message: $"{sectionPath} is null or empty.");
            }

            return value!;
        }

        public static string GetSectionValueGuid(string sectionPath)
        {
            var value = GetSectionValueStr(sectionPath);

            var tryResult = Guid.TryParse(value, out Guid valueAsGuid);

            if (!tryResult)
            {
                throw new FormatException($"{sectionPath} is not in proper format.");
            }
            return valueAsGuid.ToString();
        }

        public static bool GetSectionValueBool(string sectionPath)
        {
            var value = GetSectionValueStr(sectionPath);

            var tryResult = bool.TryParse(value, out bool valueAsBoolean);

            if (!tryResult)
            {
                throw new FormatException($"{sectionPath} is not in proper format.");
            }
            return valueAsBoolean;
        }

        public static int GetSectionValueInt(string sectionPath)
        {
            var value = GetSectionValueStr(sectionPath);

            var tryResult = int.TryParse(value, out int valueAsInt);

            if (!tryResult)
            {
                throw new FormatException($"{sectionPath} is not in proper format.");
            }
            return valueAsInt;
        }

        public static TEnum GetSectionValueEnum<TEnum>(string sectionPath) where TEnum : struct
        {
            var value = GetSectionValueStr(sectionPath);

            var tryResult = Enum.TryParse<TEnum>(value, true, out TEnum valueAsEnum);

            if (!tryResult)
            {
                throw new FormatException($"{sectionPath} is not in proper format.");
            }

            return valueAsEnum;
        }
        
    }
}
