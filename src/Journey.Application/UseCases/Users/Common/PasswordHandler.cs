using System.Security.Cryptography;
using System.Text;

namespace Journey.Application.UseCases.Users.Common
{
    public static class PasswordHandler
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);

                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string hashOfEnteredPassword = HashPassword(enteredPassword, storedSalt);

            return storedHash == hashOfEnteredPassword;
        }
    }
}
