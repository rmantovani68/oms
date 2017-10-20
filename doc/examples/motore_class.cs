Public Type MotoreType

    presente As Boolean

    Descrizione As String

    'ex uscita. Adesso si tratta del comando "non automatico"
    ComandoManuale As Boolean
    'comando invertito
    ComandoInversione As Boolean
    
    ritorno As Boolean
    RitornoReale As Boolean
    RitornoIndietro As Boolean
    ForzatoDarwin As Boolean
    'Bitmask di allarmi
    allarme As Integer

    AllarmeTimeoutAvvio As Boolean
    AllarmeNessunRitorno As Boolean
    AllarmeTermica As Boolean
    AllarmeTimeoutArresto As Boolean
    AllarmeSicurezza As Boolean
    AllarmeSlittamentoMotore As Boolean

    'Motore bloccato da PLC
    blocco As Boolean

    'Motore forzatamente acceso da PLC
    ForzAccesoPLC As Boolean
    'Motore forzatamente spento da PLC
    ForzSpentoPLC As Boolean

    '   Ora in cui è stato dato lo start al motore
    oraStart As Long
    '   Secondi di attesa del ritorno
    tempoAttesaRitorno As Long
    '   Secondi di start
    tempoStart As Long
    '   Secondi di stop
    tempoStop As Long

    '   Flag per non accendere il motore all'avvio automatico
    offStart As Boolean

    '   Flag per non spegnere il motore allo spegnimento automatico
    onStop As Boolean

    '   Motore asservito (0 = se stesso)
    asservimento As Integer

    '   In verità vi dico che contengono minuti, non ore
    MinutiLavoroParz As Long
    MinutiLavoroTot As Long
    MinutiLavoroUltimoControllo As Long
    
    SecondiLavoroAppoggio As Long 'Appoggio per il conteggio delle ore di lavoro dei motori con funzionamento temporizzato

    '   Ottimizzazione per salvataggio lento

    pausaLavoro As MotorePausaLavoro

    uscitaAnalogica As Integer
    
    '   Flag per inserire il motore in una lista di avviamento automatico ridotto
    EsclusioneConAvviamentoRidotto As Boolean
    '   Flag per dire se la lista dove il motore è stato inserito è anche selezionata da parte dell'utente
    EsclusioneSelezionata As Boolean
    '   Serve per discriminare l'esclusione del motore fra i vari gruppi di esclusione
    GruppoEsclusione As Integer
    
    InverterPresente As Boolean
    '20150625
    SoftStarterPresente As Boolean
    '
    
    tempoRitAllSlittamento As Long

    SoloVisualizzazione As Boolean

'20150422
    OraStartAllSlittamentoMotore As Long
'

    amperometro As Boolean

    '20161020
    GestioneInternaSlittamento  As Boolean
    Soglia1Slittamento  As Double
    TempoSoglia1Slittamento As Long
    Soglia2Slittamento As Double
    TempoSoglia2Slittamento As Long
    '20161020
End Type
