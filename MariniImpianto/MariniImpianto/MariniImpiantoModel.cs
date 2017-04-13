using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System.Timers;
using System.Xml;

namespace MariniImpianto
{



    public abstract class MariniGenericObject
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
        public string name { get { return _name; } set { _name = value; } }

        private string _description;
        [System.Xml.Serialization.XmlAttribute]
        public string description { get { return _description; } set { _description = value; } }

        bool _changed;
        [System.Xml.Serialization.XmlAttribute]
        public bool Changed { get { return _changed; } set { _changed = value; } }

        private readonly List<MariniGenericObject> _listaGenericObject = new List<MariniGenericObject>();
        [XmlElement("impianto", Type = typeof(MariniImpiantone))]
        [XmlElement("zona", Type = typeof(MariniZona))]
        [XmlElement("predosatore", Type = typeof(MariniPredosatore))]
        [XmlElement("plctag", Type = typeof(MariniPlctag))]
        [XmlElement("bilancia", Type = typeof(MariniBilancia))]
        [XmlElement("motore", Type = typeof(MariniMotore))]
        [XmlElement("nastro", Type = typeof(MariniNastro))]
        [XmlElement("amperometro", Type = typeof(MariniAmperometro))]
        [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
        public List<MariniGenericObject> ListaGenericObject { get { return _listaGenericObject; } }


        /*
         * Costruttori
         */
        protected MariniGenericObject(MariniGenericObject parent, string id, string name, string description)
        {
            this.parent = parent;
            this.id = id;
            this.name = name;
            this.description = description;
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

            System.Timers.Timer tTimer = new System.Timers.Timer();
            tTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            tTimer.Interval = 1000;
            tTimer.Enabled = true;
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
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
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

        public void _GetObjectById(string id, ref MariniGenericObject mgo)
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



        public void Manage()
        {
            // scateno evento
            if (m_onManage != null)
                m_onManage(this, new OnManageEventArgs(0));

            if (Changed)
            {
                if (m_onChange != null)
                    m_onChange(this, new OnChangeEventArgs(0));
                Changed = false;
            }
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Manage();
        }


        // gestione evento
        public delegate void ManageHandler(object sender, OnManageEventArgs e);
        private event ManageHandler m_onManage;
        public event ManageHandler OnManage
        {
            add { m_onManage += value; }
            remove { m_onManage -= value; }
        }

        public class OnManageEventArgs : EventArgs
        {
            public int idImpianto;

            public OnManageEventArgs(int id_impianto)
            {
                idImpianto = id_impianto;
            }
        }

        // gestione evento
        public delegate void ChangeHandler(object sender, OnChangeEventArgs e);
        private event ChangeHandler m_onChange;
        public event ChangeHandler OnChange
        {
            add { m_onChange += value; }
            remove { m_onChange -= value; }
        }


        public class OnChangeEventArgs : EventArgs
        {
            public int idImpianto;

            public OnChangeEventArgs(int id_impianto)
            {
                idImpianto = id_impianto;
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
            Console.WriteLine("Sono un oggetto base id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniImpiantone : MariniGenericObject
    {

        private bool _start;

        public bool Start
        {

            get { return _start; }

            set
            {
                if (value == true && _start == false || value == false && _start == true)
                {
                    _start = value;
                    Changed = true;
                }
                else
                {
                    // segnalare tantativo di settare valore uguale a proprieta'
                }
            }
        }

        public MariniImpiantone(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniImpiantone()
            : base()
        {
        }

        public MariniImpiantone(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {

        }

        public MariniImpiantone(XmlNode node)
            : this(null, node)
        {

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un impianto id: {0} name: {1} description: {2}", id, name, description);
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
            Console.WriteLine("Sono una zona id: {0} name: {1} description: {2}", id, name, description);
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
            Console.WriteLine("Sono un Predosatore id: {0} name: {1} description: {2}", id, name, description);
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

        private bool _start;
        [System.Xml.Serialization.XmlAttribute]
        public bool Value
        {

            get { return _start; }

            set
            {
                if (value == true && _start == false || value == false && _start == true)
                {
                    _start = value;
                    Changed = true;
                }
                else
                {
                    // segnalare tantativo di settare valore uguale a proprieta'
                }
            }
        }


        public override void ToPlainText()
        {
            Console.WriteLine("Sono un plctag id: {0} name: {1} description: {2} tagid: {3}", id, name, description, tagid);
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
            Console.WriteLine("Sono una bilancia id: {0} name: {1} description: {2}", id, name, description);
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
            Console.WriteLine("Sono un motore id: {0} name: {1} description: {2}", id, name, description);
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
            Console.WriteLine("Sono un nastro id: {0} name: {1} description: {2}", id, name, description);
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
            Console.WriteLine("Sono un amperometro id: {0} name: {1} description: {2}", id, name, description);
        }
    }

}
