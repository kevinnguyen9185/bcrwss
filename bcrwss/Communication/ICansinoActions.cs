using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcrwss.Communication
{
    internal interface ICansinoActions
    {
        Task DoGoToCasinoLobbyAsync();

        Task UpdateLobbyInfoAsync();

        Task StopWsAsync();
    }
}
