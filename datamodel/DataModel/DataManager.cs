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
    /// Classe con la quale accedere al modello dei dati. Crea dinamicamente il modello
    /// partendo da un file xml. Contiene alcuni metodi di supporto per la gestione del modello.
    /// </summary>
    public class DataManager
    {
        
        private BaseObject _dataTree;
        /// <summary>
        /// Gets the actual ImpiantoDataTree
        /// </summary>
        public BaseObject DataTree
        {
            get
            {
                return _dataTree;
            }
        }

        private Dictionary<string, GenericObject> _pathObjectsDictionary = new Dictionary<string,GenericObject>();
        /// <summary>
        /// Gets the dictionary of all object that composed DataModel with path key
        /// </summary>
        public Dictionary<string, GenericObject> PathObjectsDictionary
        {
            get
            {
                return _pathObjectsDictionary;
            }
        }

        private List<IEventHandler> _eventHandlerList = new List<IEventHandler>();
        /// <summary>
        /// Gets the class with event handler methods
        /// </summary>
        public List<IEventHandler> EventHandlerList
        {
            get
            {
                return _eventHandlerList;
            }
        }

        private ISerializer _serializer;
        /// <summary>
        /// Gets the actual DataManager
        /// </summary>
        public ISerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        //Nei costruttori faccio Dependency Injection / Inversion of Control, ovvero sposto il codice su oggetti esterni.
        public DataManager(BaseObject dataTree, ISerializer serializer, List <IEventHandler> eventHandlerList)
        {
            _Initialize(dataTree, serializer, eventHandlerList);   
        }

        public DataManager(string filename, ISerializer serializer, List<IEventHandler> eventHandlerList)
        {
            // TODO: Una qualche validazione del filename? Magari con metodo apposito, o oggetto validatore esterno
            // TODO: una qualche validazione dell'XML? Magari con metodo apposito o con oggetto validatore esterno (vedi XSD)
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNode root = doc.SelectSingleNode("*");
            BaseObject dataTree = (BaseObject)ObjectCreator.CreateMariniObject(root);

            _Initialize(dataTree, serializer, eventHandlerList);
        }
        
        /// <summary>
        /// Initialize the DataManager. Sets the DataManager and populate the Dictionaries
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _Initialize(BaseObject dataTree, ISerializer serializer, List<IEventHandler> eventHandlerList)
        {
            // TODO Faccio cosi' o uso un setter???
            this._dataTree = dataTree;
            this._serializer = serializer;
            this._eventHandlerList = eventHandlerList;
            _InitializeDictionaries();
            _SubscribeEvents();
        }

        /// <summary>
        /// Initialize and populate the dictionaries of DataManager.
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _InitializeDictionaries()
        {
            // TODO: sviluppare un metodo GetChildDictionaryByParam che chieda in ingresso anche il parametro
            // di GenericObject da usare come key del dizionario, per non fare 2 funzioni che fanno la stessa
            // cosa. Uso Reflection?
            _pathObjectsDictionary = new Dictionary<string, GenericObject>();
            _populatePathObjectsDictionary(this._dataTree);
        }


        private void _SubscribeEvents()
        {
            Console.WriteLine("\n\n\n========== INIZIO Sottoscrizione Eventi ==========");

            foreach (GenericObject mgo in this._pathObjectsDictionary.Values)
            {
                Console.WriteLine("\n\n\t-----> Oggetto {0} chiede la sottoscrizione dell'handler {1}", mgo.path, mgo.handler);
                if (mgo.handler == "NO_HANDLER")
                {
                    //Console.WriteLine("\n\tNessun handler richiesto");
                }
                else
                {

                    foreach (IEventHandler eventHandler in this._eventHandlerList)
                    {
                        if (eventHandler.GetType().Name == mgo.handler)
                        {
                            mgo.PropertyChanged += eventHandler.Handle;
                        }
                    }

                }

            }
        }


        /// <summary>
        /// Retrieve a dictionary of GenericObject children with Path key
        /// </summary>
        /// <returns>The children dictionary</returns>
        public void _populatePathObjectsDictionary(GenericObject mgo)
        {
            _pathObjectsDictionary.Add(mgo.path, mgo);
            if (mgo.ChildList.Count > 0)
            {
                foreach (GenericObject child in mgo.ChildList)
                {
                    _populatePathObjectsDictionary(child);
                }
            }
            return;
        }

        /// <summary>
        /// Gets a specific object of Datamodel.
        /// </summary>
        /// <param name="path">Path of the object</param>
        /// <returns>The <c>GenericObject</c> with the given Path, <c>null</c> if not found.</returns>
        public GenericObject GetObjectByPath(string path)
        {
            GenericObject mgo = null;
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
