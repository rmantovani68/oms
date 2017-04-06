using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyber5000
{

    public abstract class BaseObject
    {
        Boolean Active {get; set;}
        String Id {get; set;}
        String Name {get; set;}
        String Description {get; set;}

        public BaseObject()
	    {
	    }
    }

    public abstract class Container : BaseObject
    {
        List<BaseObject> Children;

	    public Container()
	    {
	    }
    }

    public class PLCTag : BaseObject
    {
        public PLCTag()
        {
        }
    }

    public class Motore : Container
    {
        public class PausaLavoroMotore
        {
            Boolean Abilitato { get; set;} 

            // Tempo di permanenza del motore acceso
            Int32 TempoPausa { get; set;} 

            // Tempo di permanenza del motore spento
            Int32 TempoLavoro { get; set;} 
        }

        // da copiare verso il PLC
        Boolean Manuale {get; set;}
        Boolean Inversione {get; set;}
        Boolean Ritorno {get; set;}

        // da copiare dal PLC
        Boolean AllarmeTimeoutAvvio {get; set;}
        Boolean AllarmeNessunRitorno {get; set;}
        Boolean AllarmeTermica {get; set;}
        Boolean AllarmeTimeoutArresto {get; set;}
        Boolean AllarmeSicurezza {get; set;}
        Boolean AllarmeSlittamentoMotore {get; set;}



        // Motore bloccato da PLC
        Boolean Blocco {get; set;}
        // Motore forzatamente acceso da PLC
        Boolean ForzAccesoPLC {get; set;}
        // Motore forzatamente spento da PLC
        Boolean ForzSpentoPLC {get; set;}

        // Ora in cui è stato dato lo start al motore
        Int64 OraStart {get; set;}
        // Secondi di attesa del ritorno
        Int64 TempoAttesaRitorno {get; set;}
        // Secondi di start
        Int64 TempoStart {get; set;}
        // Secondi di stop
        Int64 TempoStop {get; set;}

        // Flag per non accendere il motore all'avvio automatico
        Boolean OffStart {get; set;}

        // Flag per non spegnere il motore allo spegnimento automatico
        Boolean OnStop {get; set;}

        // Motore asservito (0 = se stesso)
        Int32 Asservimento {get; set;}

        // In verità vi dico che contengono minuti, non ore
        Int64 MinutiLavoroParz {get; set;}
        Int64 MinutiLavoroTot {get; set;}
        Int64 MinutiLavoroUltimoControllo {get; set;}
    
        // Appoggio per il conteggio delle ore di lavoro dei motori con funzionamento temporizzato
        Int64 SecondiLavoroAppoggio {get; set;}

        // Deve essere gestita la pausa/lavoro

        Int32 UscitaAnalogica { get; set;} 
    
        // Flag per inserire il motore in una lista di avviamento automatico ridotto
        Boolean EsclusioneConAvviamentoRidotto { get; set;} 
        
        // Flag per dire se la lista dove il motore è stato inserito è anche selezionata da parte dell'utente
        Boolean EsclusioneSelezionata { get; set;} 
        // Serve per discriminare l'esclusione del motore fra i vari gruppi di esclusione
        Int32 GruppoEsclusione { get; set;} 
    
        Boolean InverterPresente { get; set;} 

        Boolean SoftStarterPresente { get; set;} 
    
        Int64 tempoRitAllSlittamento { get; set;} 

        Boolean SoloVisualizzazione { get; set;} 

        Int64 OraStartAllSlittamentoMotore { get; set;} 

        Boolean amperometro { get; set;} 

        Boolean GestioneInternaSlittamento { get; set;} 
        Double Soglia1Slittamento { get; set;} 
        Int64 TempoSoglia1Slittamento { get; set;} 
        Double Soglia2Slittamento { get; set;} 
        Int64 TempoSoglia2Slittamento { get; set;}

        PausaLavoroMotore PausaLavoro;

        public Motore()
	    {
            PausaLavoro = new PausaLavoroMotore();
	    }
    }
}



