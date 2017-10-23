using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;

namespace DataModel
{
    /// <summary>
    /// Classe singleton con la quale accedere al modello dell'impianto Marini. Crea dinamicamente il modello
    /// partendo da un file xml. Contiene alcuni metodi di supporto per la gestione del modello.
    /// </summary>
    public class MariniImpiantoDataManager
    {
        
        private MariniBaseObject _dataTree;
        /// <summary>
        /// Gets the actual MariniImpiantoDataTree
        /// </summary>
        public MariniBaseObject DataTree
        {
            get
            {
                return _dataTree;
            }
        }

        private Dictionary<string, MariniGenericObject> _pathObjectsDictionary = new Dictionary<string,MariniGenericObject>();
        /// <summary>
        /// Gets the dictionary of all object that composed MariniImpianto with path key
        /// </summary>
        public Dictionary<string, MariniGenericObject> PathObjectsDictionary
        {
            get
            {
                return _pathObjectsDictionary;
            }
        }

        private List<IMariniEventHandler> _eventHandlerList = new List<IMariniEventHandler>();
        /// <summary>
        /// Gets the class with event handler methods
        /// </summary>
        public List<IMariniEventHandler> EventHandlerList
        {
            get
            {
                return _eventHandlerList;
            }
        }

        private IMariniSerializer _serializer;
        /// <summary>
        /// Gets the actual MariniImpiantoDataTree
        /// </summary>
        public IMariniSerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        //Nei costruttori faccio Dependency Injection / Inversion of Control, ovvero sposto il codice su oggetti esterni.
        public MariniImpiantoDataManager(MariniBaseObject dataTree, IMariniSerializer serializer, List <IMariniEventHandler> eventHandlerList)
        {
            _Initialize(dataTree, serializer, eventHandlerList);   
        }

        public MariniImpiantoDataManager(string filename, IMariniSerializer serializer, List<IMariniEventHandler> eventHandlerList)
        {
            // TODO: Una qualche validazione del filename? Magari con metodo apposito, o oggetto validatore esterno
            // TODO: una qualche validazione dell'XML? Magari con metodo apposito o con oggetto validatore esterno (vedi XSD)
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNode root = doc.SelectSingleNode("*");
            MariniBaseObject dataTree = (MariniBaseObject)MariniObjectCreator.CreateMariniObject(root);

            _Initialize(dataTree, serializer, eventHandlerList);
        }
        
        /// <summary>
        /// Initialize the MariniImpiantoDataManager. Sets the mariniImpiantoDataTree and populate the Dictionaries
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _Initialize(MariniBaseObject dataTree, IMariniSerializer serializer, List<IMariniEventHandler> eventHandlerList)
        {
            // TODO Faccio cosi' o uso un setter???
            this._dataTree = dataTree;
            this._serializer = serializer;
            this._eventHandlerList = eventHandlerList;
            _InitializeDictionaries();
            //foreach (IMariniEventHandler mariniEventsHandler in this._eventHandlerList)
            //{
            //    _SubscribeEvents(mariniEventsHandler);
            //}
            _SubscribeEvents();
        }

        /// <summary>
        /// Initialize and populate the dictionaries of MariniImpiantoDataManager.
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _InitializeDictionaries()
        {
            // TODO: sviluppare un metodo GetChildDictionaryByParam che chieda in ingresso anche il parametro
            // di MariniGenericObject da usare come key del dizionario, per non fare 2 funzioni che fanno la stessa
            // cosa. Uso Reflection?
            _pathObjectsDictionary = new Dictionary<string, MariniGenericObject>();
            _populatePathObjectsDictionary(this._dataTree);
        }


        private void _SubscribeEvents()
        {
            Console.WriteLine("\n\n\n========== INIZIO Sottoscrizione Eventi ==========");

            foreach (MariniGenericObject mgo in this._pathObjectsDictionary.Values)
            {
                Console.WriteLine("\n\n\t-----> Oggetto Marini {0} chiede la sottoscrizione dell'handler {1}", mgo.path, mgo.handler);
                if (mgo.handler == "NO_HANDLER")
                {
                    //Console.WriteLine("\n\tNessun handler richiesto");
                }
                else
                {

                    foreach (IMariniEventHandler mariniEventHandler in this._eventHandlerList)
                    {
                        if (mariniEventHandler.GetType().Name == mgo.handler)
                        {
                            mgo.PropertyChanged += mariniEventHandler.Handle;
                        }
                    }

                }

            }
        }


        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children with Path key
        /// </summary>
        /// <returns>The children dictionary</returns>
        public void _populatePathObjectsDictionary(MariniGenericObject mgo)
        {
            _pathObjectsDictionary.Add(mgo.path, mgo);
            if (mgo.ChildList.Count > 0)
            {
                foreach (MariniGenericObject child in mgo.ChildList)
                {
                    _populatePathObjectsDictionary(child);
                }
            }
            return;
        }

        /// <summary>
        /// Gets a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">Path of the object</param>
        /// <returns>The <c>MariniGenericObject</c> with the given Path, <c>null</c> if not found.</returns>
        public MariniGenericObject GetObjectByPath(string path)
        {
            MariniGenericObject mgo = null;
            if (this.PathObjectsDictionary.TryGetValue(path, out mgo))
            {
                return mgo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Serialize a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">the path of the object.</param>
        /// <returns>A string that contains the serialized object</returns>
        public string SerializeObject(string path)
        {
            return Serializer.Serialize(GetObjectByPath(path));
        }
    }
}
