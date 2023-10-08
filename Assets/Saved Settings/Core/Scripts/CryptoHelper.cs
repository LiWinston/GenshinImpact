using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SavedSettings.Cryptography
{
    /// <summary>
    /// AES 256-bit crytopgraphy helper class. SaveHelper uses these helper functions to scramble the text written in savefiles.
    /// If you are in the editor SaveHelper will not use CrytoHelper so you can still read the save file in the appData folder.
    /// </summary>
    public static class CryptoHelper
    {
        static readonly byte[] _salt;
        static readonly byte[] _initVector;

        static CryptoHelper()
        {
            _salt = Encoding.UTF8.GetBytes("P6nS&0ql");
            _initVector = Encoding.UTF8.GetBytes("Jgq*%ds0Q3$cuFxL");
        }

        /// <summary>
        /// Encrypts a string using AES.
        /// </summary>      
        public static string Encrypt(string toEncrypt, string password = "DefaultPassword")
        {
            if (toEncrypt == null || toEncrypt == string.Empty)
            {
#if UNITY_EDITOR
                Debug.Log("Attempted to encrypt empty string.");
#endif
                return string.Empty;
            }

            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                using (ICryptoTransform encryptor = rijndael.CreateEncryptor(new Rfc2898DeriveBytes(password, _salt).GetBytes(32), // 256 / 8
                                                                                _initVector))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            rijndael.Mode = CipherMode.CBC;

                            byte[] textBytes = Encoding.UTF8.GetBytes(toEncrypt);
                            cryptoStream.Write(textBytes, 0, textBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            return Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
            }
        }

        /// <summary>  
        /// Decrypts a string using AES. 
        /// </summary>  
        public static string Decrypt(string encryptedText, string password = "DefaultPassword")
        {
            if (encryptedText == null || encryptedText == string.Empty)
            {
#if UNITY_EDITOR
                Debug.Log("Attempted to decrypt empty string.");
#endif
                return string.Empty;
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText.Replace(' ', '+'));
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                using (ICryptoTransform decryptor = rijndael.CreateDecryptor(new Rfc2898DeriveBytes(password, _salt).GetBytes(32),
                                                                                _initVector))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            rijndael.Mode = CipherMode.CBC;
                            byte[] readBytes = new byte[encryptedBytes.Length];
                            return Encoding.UTF8.GetString(readBytes, 0,
                                                            cryptoStream.Read(readBytes, 0,
                                                                                readBytes.Length)).TrimEnd('\0');
                        }
                    }
                }
            }
        }
    }
}
