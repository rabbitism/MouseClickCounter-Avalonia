namespace MouseClickCounter.Services.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        bool IsValidEncryptedText(string text);
    }
}
