using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services;

/// <summary>
/// 加密服务
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const string ENCRYPTION_KEY = "MouseClickCounter2025!@#";
    private const int KEY_SIZE = 32; // AES-256

    /// <summary>
    /// 加密字符串
    /// </summary>
    public string Encrypt(string plainText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                aes.IV = new byte[16]; // 简单实现，实际应该使用随机IV

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        catch
        {
            return plainText; // 加密失败时返回原文
        }
    }

    /// <summary>
    /// 解密字符串
    /// </summary>
    public string Decrypt(string cipherText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch
        {
            return cipherText; // 解密失败时返回原文
        }
    }

    /// <summary>
    /// 获取加密密钥
    /// </summary>
    private byte[] GetKey()
    {
        // 将密钥填充或截断到指定长度
        byte[] keyBytes = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
        byte[] result = new byte[KEY_SIZE];

        int length = Math.Min(keyBytes.Length, KEY_SIZE);
        Array.Copy(keyBytes, result, length);

        // 如果密钥太短，用0填充
        for (int i = length; i < KEY_SIZE; i++)
        {
            result[i] = 0;
        }

        return result;
    }

    /// <summary>
    /// 检查是否为有效的加密文本
    /// </summary>
    public bool IsValidEncryptedText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        try
        {
            // 检查是否为有效的Base64字符串
            byte[] data = Convert.FromBase64String(text);
            return data.Length > 0;
        }
        catch
        {
            return false;
        }
    }
}