using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System.Timers;
using System.Xml;
using System.Runtime.Serialization;
using System.Net;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using log4net;


namespace MariniImpianti
{
    
    /// <summary>
    /// Classe singleton con la quale accedere al modello dell'impianto Marini. Crea dinamicamente il modello
    /// partendo da un file xml. Contiene alcuni metodi di supporto per la gestione del modello
    /// </summary>
    public sealed class MariniImpiantoTree
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MariniImpianto _mariniImpianto;
        public MariniImpianto MariniImpianto
        {
            get
            {
                return _mariniImpianto;
            }
        }

        private Dictionary<string, MariniGenericObject> _mariniImpiantoObjectsDictionary;
        public Dictionary<string, MariniGenericObject> MariniImpiantoObjectsDictionary
        {
            get
            {
                return _mariniImpiantoObjectsDictionary;
            }
        }

        private MariniImpiantoEventHandlers _mariniImpiantoEventHandlers;
        public MariniImpiantoEventHandlers MariniImpiantoEventHandlers
        {
            get
            {
                return _mariniImpiantoEventHandlers;
            }
        }



        private static MariniImpiantoTree _instance;
        private MariniImpiantoTree()
        {
            
            XmlDocument doc = new XmlDocument();
            Console.WriteLine("Carico il file xml impianto.xml");
            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            XmlNode root = doc.SelectSingleNode("*");
            Console.WriteLine("Creo l'oggetto MariniImpianto impiantoMarini mediante il factory MariniObjectCreator.CreateMariniObject");

            _mariniImpiantoEventHandlers = new MariniImpiantoEventHandlers();
            _mariniImpianto = (MariniImpianto)MariniObjectCreator.CreateMariniObject(root);

            this._mariniImpiantoObjectsDictionary = this._mariniImpianto.GetChildDictionary();


            //gestisco qui
            MethodInfo[] methods = typeof(MariniImpiantoEventHandlers).GetMethods();
                       
            foreach (MariniGenericObject mgo in this._mariniImpiantoObjectsDictionary.Values)
            {

                


                if (mgo.handler == "NO_HANDLER")
                {

                }
                else
                {

                    foreach (MethodInfo handlerInfo in methods)
                    {
                        Console.WriteLine(handlerInfo.Name);

                        Type t_mgo = mgo.GetType();
                        //foreach (var prop in t_mgo.GetProperties())
                        //{
                        //    Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(mgo, null));
                        //}

                        EventInfo ei = t_mgo.GetEvent("PropertyChanged");
                        //Console.WriteLine("{0}", ei.Name);
                        // Call  method.
                        
                        MethodInfo mi = null;

                        Console.WriteLine("handlerInfo.Name: {0} mgo.handler {1}", handlerInfo.Name, mgo.handler);

                        if (handlerInfo.Name == mgo.handler)
                        {
                            //MethodInfo mi = _mariniImpiantoEventHandlers.GetType().GetMethod("MyHandler");
                            mi = _mariniImpiantoEventHandlers.GetType().GetMethod(handlerInfo.Name);
                            Console.WriteLine("{0}", mi.Name);

                            //Delegate dg = Delegate.CreateDelegate(typeof(PropertyChangedEventHandler), value, mi);
                            Delegate dg = Delegate.CreateDelegate(ei.EventHandlerType, _mariniImpiantoEventHandlers, mi);

                            ei.AddEventHandler(mgo, dg);
                        }
                        else
                        {
                            //mi = _mariniImpiantoEventHandlers.GetType().GetMethod("MyDefaultHandler");
                        }
                        


                        //info.Invoke(_mariniImpiantoEventHandlers, null);


                        
                    }

                }
            }
            // Questa si userebbe se avessi l'handler dentro al mio oggetto
            // invece voglio un gestore esterno
            //mgo.PropertyChanged += PropertyChangedEventHandler;
                    
            // Questa funziona con un gestore esterno ma uso un solo handler
            //mgo.PropertyChanged += this.MariniImpiantoEventHandlers.PropertyChangedEventHandler;

        }

        public static MariniImpiantoTree Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MariniImpiantoTree();
                    Logger.Info("Creata l'Istanza di MariniImpiantoTree");
                }
                return _instance;
            }
        }

        public bool InitializeFromXmlFile(string filename)
        {
            XmlDocument doc = new XmlDocument();
            Console.WriteLine("Carico il file xml impianto.xml");
            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            XmlNode root = doc.SelectSingleNode("*");
            Console.WriteLine("Creo l'oggetto MariniImpianto impiantoMarini mediante il factory MariniObjectCreator.CreateMariniObject");

            _mariniImpiantoEventHandlers = new MariniImpiantoEventHandlers();
            _mariniImpianto = (MariniImpianto)MariniObjectCreator.CreateMariniObject(root);

            this._mariniImpiantoObjectsDictionary = this._mariniImpianto.GetChildDictionary();


            //gestisco qui
            MethodInfo[] methods = typeof(MariniImpiantoEventHandlers).GetMethods();

            foreach (MariniGenericObject mgo in this._mariniImpiantoObjectsDictionary.Values)
            {




                if (mgo.handler == "NO_HANDLER")
                {

                }
                else
                {

                    foreach (MethodInfo handlerInfo in methods)
                    {
                        Console.WriteLine(handlerInfo.Name);

                        Type t_mgo = mgo.GetType();
                        //foreach (var prop in t_mgo.GetProperties())
                        //{
                        //    Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(mgo, null));
                        //}

                        EventInfo ei = t_mgo.GetEvent("PropertyChanged");
                        //Console.WriteLine("{0}", ei.Name);
                        // Call  method.

                        MethodInfo mi = null;

                        Console.WriteLine("handlerInfo.Name: {0} mgo.handler {1}", handlerInfo.Name, mgo.handler);

                        if (handlerInfo.Name == mgo.handler)
                        {
                            //MethodInfo mi = _mariniImpiantoEventHandlers.GetType().GetMethod("MyHandler");
                            mi = _mariniImpiantoEventHandlers.GetType().GetMethod(handlerInfo.Name);
                            Console.WriteLine("{0}", mi.Name);

                            //Delegate dg = Delegate.CreateDelegate(typeof(PropertyChangedEventHandler), value, mi);
                            Delegate dg = Delegate.CreateDelegate(ei.EventHandlerType, _mariniImpiantoEventHandlers, mi);

                            ei.AddEventHandler(mgo, dg);
                        }
                        else
                        {
                            //mi = _mariniImpiantoEventHandlers.GetType().GetMethod("MyDefaultHandler");
                        }



                        //info.Invoke(_mariniImpiantoEventHandlers, null);



                    }

                }
            }
            // Questa si userebbe se avessi l'handler dentro al mio oggetto
            // invece voglio un gestore esterno
            //mgo.PropertyChanged += PropertyChangedEventHandler;

            // Questa funziona con un gestore esterno ma uso un solo handler
            //mgo.PropertyChanged += this.MariniImpiantoEventHandlers.PropertyChangedEventHandler;

            return true;
        }



        public MariniGenericObject GetObjectById(string id)
        {
            MariniGenericObject mgo = null;
            //_GetObjectById(id, ref mgo);
            //string test;
            if (this.MariniImpiantoObjectsDictionary.TryGetValue(id, out mgo)) // Returns true.
            {
                //Console.WriteLine(test); // This is the value at cat.
                return mgo;
            }
            else
            {
                return null;
            }

        }



        public string SerializeObject(string id)
        {
            MariniGenericObject mgo = null;

            mgo = this.GetObjectById(id);
            if (mgo == null)
            {
                Console.WriteLine("\nNon ho trovato nulla con id {0}", id);
                return "NN";
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(mgo.GetType());
                //using (StringWriter textWriter = new StringWriter())
                //{
                //    xmlSerializer.Serialize(textWriter, mgo);
                //    return textWriter.ToString();
                //}

                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true
                        ,
                        ConformanceLevel = ConformanceLevel.Auto
                            //, ConformanceLevel = ConformanceLevel.Document
                            //, NewLineOnAttributes = true
                        ,
                        Indent = true
                    }))
                    {
                        // Build Xml with xw.
                        xmlSerializer.Serialize(xmlWriter, mgo);

                    }
                    return WebUtility.HtmlDecode(stringWriter.ToString());
                }
            }
        }
    }



    public abstract class MariniGenericObject : INotifyPropertyChanged
    {
        /*
         * Attributi e proprietà 
         */
        private MariniGenericObject _parent;
        [System.Xml.Serialization.XmlIgnore]
        public MariniGenericObject parent { get { return _parent; } set { _parent = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _id;
        [System.Xml.Serialization.XmlAttribute]
        public string id { get { return _id; } set { _id = value; } }

        private string _name;
        [System.Xml.Serialization.XmlAttribute]
        public string name { get { return _name; } set { SetField(ref _name, value); } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _path;
        [System.Xml.Serialization.XmlAttribute]
        public string path { get { return _path; } set { _path = value; } }

        private string _description;
        [System.Xml.Serialization.XmlAttribute]
        public string description { get { return _description; } set { SetField(ref _description, value); } }

        private string _handler;
        [System.Xml.Serialization.XmlAttribute]
        public string handler { get { return _handler; } set { SetField(ref _handler, value); } }

        private readonly List<MariniGenericObject> _listaGenericObject = new List<MariniGenericObject>();
        [XmlElement("impianto", Type = typeof(MariniImpianto))]
        [XmlElement("zona", Type = typeof(MariniZona))]
        [XmlElement("predosatore", Type = typeof(MariniPredosatore))]
        [XmlElement("plctag", Type = typeof(MariniPlctag))]
        [XmlElement("bilancia", Type = typeof(MariniBilancia))]
        [XmlElement("motore", Type = typeof(MariniMotore))]
        [XmlElement("nastro", Type = typeof(MariniNastro))]
        [XmlElement("amperometro", Type = typeof(MariniAmperometro))]
        [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
        public List<MariniGenericObject> ListaGenericObject { get { return _listaGenericObject; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            //Console.WriteLine("Sono nel metodo OnPropertyChanged");
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        //protected bool SetField<T>(ref T field, T value, string propertyName)
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                //Console.WriteLine("Sono nella SetField e la proprieta' non e' cambiata");
                return false;
            }
            field = value;
            //Console.WriteLine("Sono nella SetField e lancio OnPropertyChanged(propertyName)");
            OnPropertyChanged(propertyName);
            return true;
        }


        /*
         * Costruttori
         */
        protected MariniGenericObject(MariniGenericObject parent, string id, string name, string description,string handler)
        {
            if (parent==null)
            {
                path = "/" + id;
            } 
            else
            {
                path = parent.path + "/" + id;
            }           
            this.parent = parent;
            this.id = id;
            this.name = name;
            this.description = description;
            this.handler = handler;

        }

        protected MariniGenericObject(MariniGenericObject parent, string id, string name,string description)
            : this(parent, id, name, description, "NO_HANDLER")
        {
        }

        protected MariniGenericObject(MariniGenericObject parent, string id, string name)
            : this(parent, id, name, "NO_DESCRIPTION")
        {
        }

        protected MariniGenericObject(MariniGenericObject parent, string id)
            : this(parent, id, "NO_NAME")
        {
        }

        protected MariniGenericObject(MariniGenericObject parent)
            : this(parent, "NO_ID")
        { 
        }

        protected MariniGenericObject()
            : this(null, "NO_ID")
        {
        }

        protected MariniGenericObject(MariniGenericObject parent, XmlNode node)
            : this(parent)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
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
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

            if (parent == null)
            {
                path = "/" + id;
            }
            else
            {
                path = parent.path + "/" + id;
            }           

        }

        protected MariniGenericObject(XmlNode node)
            : this(null, node)
        {
        }



        /***********************
         * Metodi ed eventi
         ***********************/

        /*
         * Prova per metodo astratto
         * Metodo astratto per semplice visualizzazione su Console
         */
        public abstract void ToPlainText();

        /*
         * Prova per ricorsività su metodo astratto
         */
        public void ToPlainTextRecursive()
        {
            ToPlainText();
            foreach (MariniGenericObject mgo in _listaGenericObject)
            {
                mgo.ToPlainTextRecursive();
            }
            return;
        }

        public MariniGenericObject GetObjectById(string id)
        {
            MariniGenericObject mgo = null;
            _GetObjectById(id, ref mgo);
            return mgo;
        }

        private void _GetObjectById(string id, ref MariniGenericObject mgo)
        {
            if (this.id == id)
            {
                mgo = this;
                return;
            }
            else
            {
                if (_listaGenericObject.Count > 0)
                {
                    foreach (MariniGenericObject child in _listaGenericObject)
                    {
                        child._GetObjectById(id, ref mgo);
                    }
                }
            }
        }

        public List<MariniGenericObject> GetObjectListByType(Type type)
        {
            List<MariniGenericObject> mgoList= new List<MariniGenericObject>();
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

        public Dictionary<string, MariniGenericObject> GetChildDictionary()
        {
            Dictionary<string, MariniGenericObject> md = new Dictionary<string, MariniGenericObject>();
            _GetChildDictionary(ref md);
            return md;


        }

        private void _GetChildDictionary(ref Dictionary<string, MariniGenericObject> md)
        {

            md.Add(this.id, this);
            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetChildDictionary(ref md);
                }
            }
        }

    }

    public class MariniOggettoBase : MariniGenericObject
    {
        public MariniOggettoBase(MariniGenericObject parent, string id, string name, string description)
            : base(parent, id, name, description)
        {
        }

        public MariniOggettoBase(MariniGenericObject parent, string id, string name)
            : base(parent, id, name)
        {
        }

        public MariniOggettoBase(MariniGenericObject parent, string id)
            : base(parent, id)
        {
        }

        public MariniOggettoBase(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniOggettoBase()
            : base()
        {
        }

        public MariniOggettoBase(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniOggettoBase(XmlNode node)
            : base(node)
        {

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un oggetto base id: {0} name: {1} description: {2}", id, name, description, path);
        }
    }

    public class MariniImpianto : MariniGenericObject
    {

        //protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public MariniImpianto(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniImpianto()
            : base()
        {
        }

        public MariniImpianto(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniImpianto(XmlNode node)
            : this(null, node)
        {

        }

        public override void ToPlainText()
        {
            //Logger.Info("Sono un impianto ");
            Console.WriteLine("Sono un impianto id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }


    public class MariniZona : MariniGenericObject
    {
        public MariniZona(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniZona()
            : base()
        {
        }

        public MariniZona(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniZona(XmlNode node)
            : this(null, node)
        {

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono una zona id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

    public class MariniPredosatore : MariniGenericObject
    {
        public MariniPredosatore(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniPredosatore()
            : base()
        {
        }

        public MariniPredosatore(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniPredosatore(XmlNode node)
            : this(null, node)
        {

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un Predosatore id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

    public class MariniPlctag : MariniGenericObject
    {
        public MariniPlctag(MariniGenericObject parent, string tagid)
            : base(parent)
        {
            this.tagid = tagid;
        }

        public MariniPlctag(MariniGenericObject parent)
            : this(parent, "NO_PLCTAG")
        {
        }


        public MariniPlctag()
            : this(null, "NO_PLCTAG")
        {
        }

        public MariniPlctag(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        case "tagid":
                            tagid = attr.Value;
                            break;

                    }
                }
            }

        }

        public MariniPlctag(XmlNode node)
            : this(null, node)
        {

        }

        private string _tagid;
        [System.Xml.Serialization.XmlAttribute]
        public string tagid { get { return _tagid; } set { _tagid = value; } }

        


        public override void ToPlainText()
        {
            Console.WriteLine("Sono un plctag id: {0} name: {1} description: {2} tagid: {3} path: {4}", id, name, description, tagid, path);
        }
    }

    public class MariniBilancia : MariniGenericObject
    {
        public MariniBilancia(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniBilancia()
            : base()
        {
        }

        public MariniBilancia(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniBilancia(XmlNode node)
            : this(null, node)
        {

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono una bilancia id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

    public class MariniMotore : MariniGenericObject
    {

        public MariniMotore(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniMotore()
            : base()
        {
        }

        public MariniMotore(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniMotore(XmlNode node)
            : this(null, node)
        {

        }


        public override void ToPlainText()
        {
            Console.WriteLine("Sono un motore id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

    public class MariniNastro : MariniGenericObject
    {

        public MariniNastro(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniNastro()
            : base()
        {
        }

        public MariniNastro(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniNastro(XmlNode node)
            : this(null, node)
        {

        }


        public override void ToPlainText()
        {
            Console.WriteLine("Sono un nastro id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

    public class MariniAmperometro : MariniGenericObject
    {

        public MariniAmperometro(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniAmperometro()
            : base()
        {
        }

        public MariniAmperometro(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniAmperometro(XmlNode node)
            : this(null, node)
        {

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un amperometro id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }

}
