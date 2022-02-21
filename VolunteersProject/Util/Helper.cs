
namespace VolunteersProject.Util
{
    /// <summary>
    /// Helper class for extension methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Make upper the first character, make lower the rest.
        /// </summary>
        /// <param name="value">String to change.</param>
        /// <returns>Return a modified string.</returns>
        public static string FirstUpperNextLower(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return char.ToUpper(value[0]) + value.Substring(1).ToLower();
            }
            else
            {
                return value;
            }
        }       
    }
}
