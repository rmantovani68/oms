using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MariniImpianti;

namespace MariniWcfService.App_Code
//namespace MariniWcfService
{

    // Questa classe e' puo' essere chiamata come mi pare. Serve solo che ci sia, al suo interno, un metodo AppInitialize()
    // che, essendo nella sottocartella App_Code, viene lanciato all'inizio del servizio, cioe' alla prima chiamata da browser
    // di una delle funzioni implementate.
    public class MariniWcfServiceInitializer
    {
        
        public static void AppInitialize()
        {
            // This will get called on startup

            // Qui faccio una chiamata a MariniRestService perche', se faccio partire il logging da qui, non so perche',
            // non funziona. Da MariniRestService il logging torna corretto. Forse perche' se parte qui fa la prima istanza
            // di Logger dove non ha i suoi file di configurazione
            MariniRestService.Initialize();

        }
    }

}