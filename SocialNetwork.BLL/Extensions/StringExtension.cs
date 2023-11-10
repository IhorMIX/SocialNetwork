using System.Security.Cryptography;
using System.Text;

namespace SocialNetwork.BLL.Extensions;

public static class StringExtension
{
    public static string ToBase64(this string str)
    {
        return Convert.ToBase64String(Encoding.Default.GetBytes(str));
    }

    public static string FromBase64ToString(this string base64String)
    {
        return Encoding.Default.GetString(Convert.FromBase64String(base64String));
    }
    
    public static string Encrypt(this string plainText, string encryptionKey, string iv)
    {
        using Aes aesAlg = Aes.Create(); 
        aesAlg.Key = FromHexString(encryptionKey);
        aesAlg.IV = FromHexString(iv);

        using ICryptoTransform encryptor = aesAlg.CreateEncryptor();
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedData = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

        string encryptedHex = BitConverter.ToString(encryptedData).Replace("-", "").ToLower();
        return encryptedHex;
    }
    
    public static string Decrypt(this string encryptedHex, string encryptionKey, string iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = FromHexString(encryptionKey);
        aesAlg.IV = FromHexString(iv);

        using ICryptoTransform decryptor = aesAlg.CreateDecryptor();

        byte[] encryptedData = new byte[encryptedHex.Length / 2];
        for (int i = 0; i < encryptedData.Length; i++)
        {
            encryptedData[i] = Convert.ToByte(encryptedHex.Substring(i * 2, 2), 16);
        }

        byte[] decryptedData = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        string originalData = Encoding.UTF8.GetString(decryptedData);
        return originalData;
    }
    
    private static byte[] FromHexString(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
    
}