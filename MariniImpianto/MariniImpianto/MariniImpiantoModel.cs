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

        bool _changed;
        [System.Xml.Serialization.XmlAttribute]
        public bool Changed
        {
            get { return _changed; }
            set { _changed = value; }
        }

        protected MariniGenericObject(string id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        protected MariniGenericObject(string id, string name)
            : this(id, name, "NO_DESCRIPTION")
        {
        }

        protected MariniGenericObject(string id)
            : this(id, "NO_NAME", "NO_DESCRIPTION")
        {
        }

        protected MariniGenericObject()
            : this("NO_ID", "NO_NAME", "NO_DESCRIPTION")
        {
            System.Timers.Timer tTimer = new System.Timers.Timer();
            tTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            tTimer.Interval = 1000;
            tTimer.Enabled = true;
        }


        protected MariniGenericObject(XmlNode node)
            : this()
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

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Manage();
        }

        public abstract void ToPlainText();

        public void ToPlainTextRecursive()
        {
            ToPlainText();
            foreach (MariniGenericObject mgo in _listaGenericObject)
            {
                mgo.ToPlainTextRecursive();
            }
            return;
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
        public MariniOggettoBase(string id, string name, string description)
            : base(id, name, description)
        {
        }

        public MariniOggettoBase(string id, string name)
            : base(id, name)
        {
        }

        public MariniOggettoBase(string id)
            : base(id)
        {
        }

        public MariniOggettoBase()
            : base()
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

        public MariniImpiantone()
            : base()
        {
        }

        public MariniImpiantone(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {

                    }
                }
            }

        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un impianto id: {0} name: {1} description: {2}", id, name, description);
        }
    }


    public class MariniZona : MariniGenericObject
    {
        public MariniZona()
            : base()
        {
        }

        public MariniZona(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono una zona id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniPredosatore : MariniGenericObject
    {
        public MariniPredosatore()
            : base()
        {
        }

        public MariniPredosatore(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono un Predosatore id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniPlctag : MariniGenericObject
    {
        public MariniPlctag(string tagid)
            : base()
        {
            this.tagid = tagid;
        }

        public MariniPlctag()
            : this("NO_PLCTAG")
        {
        }

        public MariniPlctag(XmlNode node)
            : base(node)
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
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }

        private string _tagid;
        [System.Xml.Serialization.XmlAttribute]
        public string tagid { get { return _tagid; } set { _tagid = value; } }

        private bool _start;

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
        public MariniBilancia()
            : base()
        {
        }

        public MariniBilancia(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono una bilancia id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniMotore : MariniGenericObject
    {
        public MariniMotore()
            : base()
        {
        }

        public MariniMotore(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono un motore id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniNastro : MariniGenericObject
    {
        public MariniNastro()
            : base()
        {
        }

        public MariniNastro(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono un nastro id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniAmperometro : MariniGenericObject
    {
        public MariniAmperometro()
            : base()
        {
        }

        public MariniAmperometro(XmlNode node)
            : base(node)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        //case "id":
                        //    id = attr.Value;
                        //    break;
                        //case "name":
                        //    name = attr.Value;
                        //    break;
                        //case "description":
                        //    description = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

        }
        public override void ToPlainText()
        {
            Console.WriteLine("Sono un amperometro id: {0} name: {1} description: {2}", id, name, description);
        }
    }

}
