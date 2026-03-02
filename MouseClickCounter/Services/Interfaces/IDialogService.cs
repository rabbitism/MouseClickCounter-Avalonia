using System.Threading.Tasks;

namespace MouseClickCounter.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowConfigDialogAsync();
        Task ShowAllRankDialogAsync();
    }
}
