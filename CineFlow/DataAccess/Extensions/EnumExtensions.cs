namespace CineFlow.DataAccess.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            // Return empty space if enum value is 0 (default/unset)
            if (Convert.ToInt32(enumValue) == 0)
                return " ";

            // Original logic for non-zero values
            return enumValue.GetType()
                           .GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           ?.GetName() ?? enumValue.ToString();
        }
    }
}
