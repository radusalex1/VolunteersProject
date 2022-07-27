using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;

namespace VolunteersProject.Util
{
    /// <summary>
    /// Helper class for extension methods.
    /// </summary>
    internal static class Helper
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

        /// <summary>
        /// Validates Instagram
        /// </summary>
        /// <param name="instagramProfile"></param>
        /// <returns>Return true if volunteer instagram is valid, otherwise false.</returns>
        public static bool InstagramIsValid(string instagramProfile)
        {
            //to do: move this to appConfig
            string pattern = @"(?:^|[^\w])(?:@)([A-Za-z0-9_](?:(?:[A-Za-z0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z0-9_]))?)";
            Match m = Regex.Match(instagramProfile, pattern);

            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true is email is valid, false otherwise
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Return true if volunteer email is valid, otherwise false.</returns>
        public static bool EmailIsValid(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates Phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>Return true if volunteer phone is valid, otherwise false.</returns>
        public static bool PhoneNumberIsValid(string phoneNumber)
        {
            //to do: move this to appConfig
            string pattern = @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$";
            Match m = Regex.Match(phoneNumber, pattern);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates Image profile
        /// </summary>
        /// <param name="imageProfileLength">Image profile length.</param>
        /// <param name="imageFileLengthLimit">Image file length.</param>        
        /// <returns>Return true if volunteer image profile is valid, otherwise false.</returns>
        public static bool ValidateImageProfile(long imageProfileLength, int imageFileLengthLimit)
        {
            return (imageProfileLength > imageFileLengthLimit) ? true : false;
        }       

        /// <summary>
        /// Validates Name(all capitals)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatName(string name)
        {
            return name.ToUpper();            
        }

        /// <summary>
        /// get byte array from file
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>Return byte array.</returns>
        public static byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}
