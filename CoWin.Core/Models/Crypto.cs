using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CoWin.Core.Models
{
    public class Crypto
    {
        /// <summary>
        /// Wrapper Around implementation of AES by Microsoft's System.Security.Cryptography lib
        ///     IV Size = 16 Bytes
        ///     Mode = ECB
        ///     Dependant on the Key Size, AES will be chosen like AES-256 for 32 byte key.
        /// </summary>
        /// <param name="key">Key used for Encryption. Key Size can be any of 128 bit, 198 bit and 256 bits</param>
        /// <param name="plainText">Data to be Encrypted</param>
        /// <returns>Base64 Encoded Encrypted/Cipher Text of the plainText</returns>
        public static string Encrypt(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        ///  Wrapper Around implementation of AES by Microsoft's System.Security.Cryptography lib
        ///     IV Size = 16 Bytes
        ///     Mode = ECB
        ///     Dependant on the Key Size, AES will be chosen like AES-256 for 32 byte key.
        ///     Encryption & Decryption Keys should be Same as AES is a symmetric algorithm 
        /// </summary>
        /// <param name="key">Key used for Decryption. Key Size can be any of 128 bit, 198 bit and 256 bits</param>
        /// <param name="cipherText">Base64 Encoded Cipher Text/Encrypted Data to be decrypted</param>
        /// <returns>Decrypted/Plain Text of the cipherText</returns>
        public static string Decrypt(string key, string cipherText)
        {
            if (!IsBase64String(cipherText))
            {
                throw new InvalidDataException("Input String not in Correct Base-64 Format which can be used to correctly decrypt the data.");
            }
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static bool IsBase64String(string base64)
        {
            base64 = base64.Trim();
            return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

    }
}