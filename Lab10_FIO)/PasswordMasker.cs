using System.Security.Cryptography;
using System.Text;

namespace Lab10_FIO_
{

    public static class PasswordMasker
    {
        private static readonly HashAlgorithm _hashAlgorithm = SHA256.Create();

        public static string MaskPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "***EMPTY***";

            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = _hashAlgorithm.ComputeHash(inputBytes);

            string hash = BitConverter.ToString(hashBytes, 0, 16).Replace("-", "");

            return $"***MASKED-{hash}***";
        }
    }
}