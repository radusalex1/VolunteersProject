namespace VolunteersProject.Common
{
    internal class ApplicationValues
    {
        public static string JwtToken = string.Empty;

        public static bool IsUserLogged()
        {
            return !string.IsNullOrEmpty(JwtToken);
        }
    }
}