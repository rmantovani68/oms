using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per far funzionare InotifyPropertyChanged.
// InotifyPropertyChanged è l'interfaccia per notificare a clienti (in generale binding clients) che una
// proprietà è cambiata
using System.ComponentModel;
// Libreria per far funzionare la serializzazione. Ad esmpio per decorare le proprietà da
// serializzare con [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
using System.Xml.Serialization;
// Libreria per far funzionare l'attributo [CallerMemberName] presente nella OnPropertyChanged(…).
// In questo modo non ci si deve più preoccupare di rinominare le proprietà.
using System.Runtime.CompilerServices;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;

namespace DataModel
{
    /// <summary>
    /// Represents a generic object of MariniImpianto
    /// </summary>
    public abstract class MariniGenericObject : INotifyPropertyChanged
    {

        #region properties

        

        private MariniGenericObject _parent;
        /// <summary>
        /// Gets and Sets the parent of an object
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public MariniGenericObject parent { get { return _parent; } set { _parent = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("type")]
        private string _type;
        /// <summary>
        /// Gets and Sets the type of an object (Impianto, Nastro, Motote,...)
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string type { get { return _type; } set { _type = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _id;
        /// <summary>
        /// Gets and Sets the ID of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string id { get { return _id; } set { _id = value; } }

        private string _name;
        /// <summary>
        /// Gets and Sets the name of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string name { get { return _name; } set { _name = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _path;
        /// <summary>
        /// Gets and Sets the path of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string path { get { return _path; } set { _path = value; } }

        private string _description;
        /// <summary>
        /// Gets and Sets the description of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string description { get { lock (synchro) { return _description; } } set { SetField(ref _description, value); } }
        
        private string _handler;
        /// <summary>
        /// Gets and Sets the handler method of an object propertychanged event
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string handler { get { return _handler; } set { _handler = value; } }

        private readonly List<MariniGenericObject> _childList = new List<MariniGenericObject>();
        /// <summary>
        /// Gets the list of children objects
        /// </summary>
        // TODO: Vanno aggiunti gli elementi presenti nella serializzazione.
        [XmlElement("Property", Type = typeof(MariniProperty))]
        [XmlElement("oggettobase", Type = typeof(MariniBaseObject))]
        public List<MariniGenericObject> ChildList { get { return _childList; } }

        /// <summary>
        /// Threadsafe implementation of the event <see cref="MariniGenericObject.OnPropertyChanged"/>
        /// </summary>
        readonly object synchro = new object();

        #endregion

        #region events
        

        // By default the event implementation is not threadsafe.
        // We will do a thread-safe implementation of the event
        // So this is the old implementation
        /*
        public event PropertyChangedEventHandler PropertyChanged;
        */
        // And this is the new implementation
        private PropertyChangedEventHandler internalPropertyChanged;
        /// <summary>
        /// Occurs when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (synchro)
                {
                    internalPropertyChanged += value;
                }
            }
            remove
            {
                lock (synchro)
                {
                    internalPropertyChanged -= value;
                }
            }
        }

        private void OnPropertyChanged(string property)
        {
            //Send notification without acquiring the synchro object
            PropertyChangedEventHandler localCopy;
            lock (synchro)
            {
                localCopy = internalPropertyChanged != null ? (PropertyChangedEventHandler)internalPropertyChanged.Clone() : null;
            }

            if (localCopy != null)
            {
                localCopy(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Used in every property set to launch <see cref="MariniGenericObject.OnPropertyChanged"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns><c>true</c> if the property is effectively changed; otherwise, <c>false</c>.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            lock (synchro)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    //Console.WriteLine("Sono nella SetField e la proprieta' non e' cambiata");
                    return false;
                }
                field = value;
            }

            //Console.WriteLine("Sono nella SetField e lancio OnPropertyChanged(propertyName)");
            OnPropertyChanged(propertyName);

            return true;
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="type">MariniGenericObject ID</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        /// <param name="description">MariniGenericObject description</param>
        /// <param name="handler">MariniGenericObject method name to handle the PropertyChange event</param>
        protected MariniGenericObject(MariniGenericObject parent, string type, string id, string name, string description, string handler)
        {
            this.parent = parent;
            this.type = type;
            this.id = id;
            this.name = name;
            this.description = description;
            this.handler = handler;

            SetObjPath(parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="type">MariniGenericObject ID</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        /// <param name="description"></param>
        protected MariniGenericObject(MariniGenericObject parent, string type, string id, string name, string description)
            : this(parent, type, id, name, description, "NO_HANDLER")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="type">MariniGenericObject ID</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        protected MariniGenericObject(MariniGenericObject parent, string type, string id, string name)
            : this(parent, type, id, name, "NO_DESCRIPTION")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="type">MariniGenericObject ID</param>
        /// <param name="id">MariniGenericObject ID</param>
        protected MariniGenericObject(MariniGenericObject parent, string type, string id)
            : this(parent, type, id, "NO_NAME")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="type">MariniGenericObject ID</param>
        protected MariniGenericObject(MariniGenericObject parent, string type)
            : this(parent, type, "NO_ID")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        protected MariniGenericObject(MariniGenericObject parent)
            : this(parent, "NO_TYPE")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        protected MariniGenericObject()
            : this(null, "NO_TYPE")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="node">An Xml node from which to construct the object</param>
        protected MariniGenericObject(MariniGenericObject parent, XmlNode node)
            : this(parent)
        {
            // L'attributo type viene ricavato dal nome del nodo
            type = node.Name;
            // Gli altri attributi della classe li ricavo dalle altri attributi dell'XML
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "type":
                        //    type = attr.Value;
                        //    break;
                        case "id":
                            id = attr.Value;
                            break;
                        case "name":
                            name = attr.Value;
                            break;
                        case "description":
                            description = attr.Value;
                            break;
                        case "handler":
                            handler = attr.Value;
                            break;
                        // default:
                        // throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }
            // calcolo il path dell'oggetto
            SetObjPath(parent);
        }

        /// <summary>
        /// Initializes the object path for the MariniGenericObject <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="node">An Xml node from which to construct the object</param>
        private void SetObjPath(MariniGenericObject parent)
        {
            if (parent == null)
            {
                path = id;
            }
            else
            {
                path = parent.path + "." + id;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="node">An Xml node from which to construct the object</param>
        protected MariniGenericObject(XmlNode node)
            : this(null, node)
        {
        }

        #endregion

        #region Methods

        public override bool Equals(Object o)
        {
            MariniGenericObject mgo = o as MariniGenericObject;
            if (mgo == null)
                return false;
            else
                return path.Equals(mgo.path);
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        // TODO: attenzione anche ai metodi. Quali metodi e' corretto lasciare all'interno dell'oggetto e
        // quali invece e' bene far gestire a agenti esterni, facendo dell'oggetto un puro modello di dati?


        /// <summary>
        /// Plain description of the MariniGenericObject.
        /// </summary>
        public abstract void ToPlainText();

        /// <summary>
        /// Plain description of the MariniGenericObject and, recursively, it's children.
        /// </summary>
        public void ToPlainTextRecursive()
        {
            ToPlainText();
            foreach (MariniGenericObject mgo in _childList)
            {
                mgo.ToPlainTextRecursive();
            }
            return;
        }

        /*
        /// <summary>
        /// Retrieve a list of MariniGenericObject child of a specific type
        /// </summary>
        /// <param name="type">The type of MariniGenericObject</param>
        /// <returns>A list of MariniGenericObject</returns>
        public List<MariniGenericObject> GetObjectListByType(Type type)
        {
            List<MariniGenericObject> mgoList = new List<MariniGenericObject>();
            _GetObjectListByType(type, ref mgoList);
            return mgoList;
        }

        private void _GetObjectListByType(Type type, ref List<MariniGenericObject> mgoList)
        {
            if (this.GetType() == type)
            {
                mgoList.Add(this);
                //return;
            }

            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetObjectListByType(type, ref  mgoList);
                }
            }
        }
        */

        // TODO: magari da spostare in un eventuale altro oggetto agente che faccia cose
        // sul data model???

        /*
        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children
        /// </summary>
        /// <returns>The children dictionary</returns>
        public Dictionary<string, MariniGenericObject> GetIdChildDictionary()
        {
            Dictionary<string, MariniGenericObject> md = new Dictionary<string, MariniGenericObject>();
            _GetIdChildDictionary(ref md);
            return md;
        }

        private void _GetIdChildDictionary(ref Dictionary<string, MariniGenericObject> md)
        {
            md.Add(this.id, this);
            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetIdChildDictionary(ref md);
                }
            }
        }
        */

        // TODO: magari da spostare in un eventuale altro oggetto agente che faccia cose
        // sul data model???

        /*
        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children
        /// </summary>
        /// <returns>The children dictionary</returns>
        public Dictionary<string, MariniGenericObject> GetPathChildDictionary()
        {
            Dictionary<string, MariniGenericObject> md = new Dictionary<string, MariniGenericObject>();
            _GetPathChildDictionary(ref md);
            return md;
        }

        private void _GetPathChildDictionary(ref Dictionary<string, MariniGenericObject> md)
        {
            md.Add(this.path, this);
            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetPathChildDictionary(ref md);
                }
            }
        }
        */



        #endregion
    }

}
