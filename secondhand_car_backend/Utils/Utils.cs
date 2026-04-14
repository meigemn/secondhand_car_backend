using System.Security.Cryptography;
using System.Text;

namespace secondhand_car_backend.Utils
{
    public static class Utils
    {
        /// <summary>
        ///     Hashea una contraseña igual que el frontend
        /// </summary>
        /// <param name="password">la contraseña a hashear</param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return string.Concat(builder.ToString(), "@", "A", "a");
            }
        }

    }
}