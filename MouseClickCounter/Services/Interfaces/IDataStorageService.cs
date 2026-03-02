using System.Threading.Tasks;
using MouseClickCounter.Models;

namespace MouseClickCounter.Services.Interfaces;

public interface IDataStorageService
{
    Task<bool> SaveClickDataAsync(ClickData clickData);
    Task<ClickData?> LoadClickDataAsync();
    Task<bool> DeleteDataFileAsync();
    bool DataFileExists();
    string GetDataFilePath();
}