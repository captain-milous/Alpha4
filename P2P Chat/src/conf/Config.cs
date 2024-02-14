using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat.src.conf
{
    /// <summary>
    /// Třída obsahující konfigurační nastavení pro aplikaci, včetně cest k vstupním a výstupním souborům, logům a seznamu uživatelů.
    /// </summary>
    public class Config
    {
        public string PortUDP { get; set; }
        public string PortTCP { get; set; }

        /// <summary>
        /// Inicializuje novou instanci třídy Config bez parametrů.
        /// </summary>
        public Config() { }
    }
}
