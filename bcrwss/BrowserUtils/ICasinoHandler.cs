using CefSharp;
using System.Threading.Tasks;

namespace bcrwss.BrowserUtils
{
    public interface ICasinoHandler
    {
        string GetCasinoFrameUrl();

        Task<string> GetCasinoSessionIdAsync();

        Task<bool> IsCasinoBrowserReady();

        void RandomSwitchLayoutCasino();

        void GoToTable(string tableId);

        Task<string> GetWebSessionIdFromStorageAsync();
    }
}
