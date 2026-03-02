using System;
using System.IO;
using System.Threading.Tasks;
using MouseClickCounter.Models;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services;

/// <summary>
/// 数据存储服务
/// </summary>
public class DataStorageService : IDataStorageService
{
    private readonly IEncryptionService _encryptionService;
    private readonly ILogService _logService;
    private readonly string _dataFilePath;

    /// <summary>
    /// 创建数据存储服务实例
    /// </summary>
    public DataStorageService(IEncryptionService encryptionService, ILogService logService)
    {
        _encryptionService = encryptionService;
        _logService = logService;
        _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "click_data.enc");
    }

    /// <summary>
    /// 保存点击数据
    /// </summary>
    public async Task<bool> SaveClickDataAsync(ClickData clickData)
    {
        try
        {
            var plainText = clickData.ToString();
            var encryptedData = _encryptionService.Encrypt(plainText);
            await File.WriteAllTextAsync(_dataFilePath, encryptedData);
            return true;
        }
        catch (Exception ex)
        {
            // 记录错误
            await _logService.WriteErrorAsync("保存点击数据失败", ex);
            return false;
        }
    }

    /// <summary>
    /// 加载点击数据
    /// </summary>
    public async Task<ClickData?> LoadClickDataAsync()
    {
        try
        {
            if (!File.Exists(_dataFilePath))
            {
                return null;
            }

            var encryptedData = await File.ReadAllTextAsync(_dataFilePath);

            // 检查是否为有效的加密数据
            if (!_encryptionService.IsValidEncryptedText(encryptedData))
            {
                // 文件可能已损坏，删除它
                File.Delete(_dataFilePath);
                return null;
            }

            var decryptedData = _encryptionService.Decrypt(encryptedData);

            // 检查解密是否成功
            if (decryptedData == encryptedData)
            {
                // 解密失败，文件可能已损坏
                File.Delete(_dataFilePath);
                return null;
            }

            return ClickData.FromString(decryptedData);
        }
        catch (Exception ex)
        {
            // 记录错误
            await _logService.WriteErrorAsync("加载点击数据失败", ex);
            return null;
        }
    }

    /// <summary>
    /// 删除数据文件
    /// </summary>
    public async Task<bool> DeleteDataFileAsync()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                File.Delete(_dataFilePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync("删除数据文件失败", ex);
            return false;
        }
    }

    /// <summary>
    /// 检查数据文件是否存在
    /// </summary>
    public bool DataFileExists()
    {
        return File.Exists(_dataFilePath);
    }

    /// <summary>
    /// 获取数据文件路径
    /// </summary>
    public string GetDataFilePath()
    {
        return _dataFilePath;
    }
}