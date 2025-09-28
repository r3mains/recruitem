namespace recruitem_backend.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
                return isValid;
            }
            catch
            {
                return false;
            }
        }
    }
}
