using System.Security.Cryptography;
using System.Text;

namespace TheLight_JoneBookShop_WebMVC.helper
{
    public class HASHBYTES_SHA2_256
    {
        public static byte[] Hash(string input)
        {
            using (SHA256 hasher = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = hasher.ComputeHash(Encoding.Unicode.GetBytes(input));

                // Return the hexadecimal string.
                return data;
            }
        }
    }
}
