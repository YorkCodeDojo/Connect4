namespace Connect4.Core.Services
{
    public class PasswordHashing
    {
        public bool CompareHashes(string passwordToCheck, string storedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, storedPassword);
        }

        public string HashPassword(string plainTextPassword)
        {
            var costParameter = 12;
            return BCrypt.Net.BCrypt.EnhancedHashPassword(plainTextPassword, costParameter);
        }
    }
}
