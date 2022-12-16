using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace MusicWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Login is required!")]
        [MinLength(4, ErrorMessage = "Login is too short. Must be between 4 and 16 symbols")]
        [MaxLength(16, ErrorMessage = "Login is too long. Must be between 4 and 16 symbols")]
        public string? Login { get; set; }

        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Password is too short. Must be between 8 and 32 symbols")]
        [MaxLength(32, ErrorMessage = "Password is too long. Must be between 8 and 32 symbols")]
        //TO DO: Change minimum after first phase of development
        public string? Password { get; set; }

        [Display(Name = "Role")]
        public Role UserRole { get; set; }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            using (Rfc2898DeriveBytes bytes = new(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] newHashed;
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] salt = new byte[0x10];
            Buffer.BlockCopy(src, 1, salt, 0, 0x10);
            byte[] hashed = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, hashed, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new(password, salt, 0x3e8))
            {
                newHashed = bytes.GetBytes(0x20);
            }
            return hashed.SequenceEqual(newHashed);
        }

        public bool PasswordMatches(string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(Password)) return false;
            return VerifyHashedPassword(Password, password);
        }
    }
    /*
    Roman: 123
    Rockel: 111
    
     */
}
