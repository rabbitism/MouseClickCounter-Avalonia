using MouseClickCounter.Models;

namespace MouseClickCounter.Services.Interfaces
{
    public interface IDataStorageService
    {
        bool SaveClickData(ClickData clickData);
        ClickData? LoadClickData();
        bool DeleteDataFile();
        bool DataFileExists();
        string GetDataFilePath();
    }
}
