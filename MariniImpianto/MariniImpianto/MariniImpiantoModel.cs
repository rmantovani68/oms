using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System.Timers;

namespace MariniImpianto
{
    
    public abstract class MariniGenericObject
    {

        bool _changed;

        public bool Changed {
            get { return _changed; }
            set {_changed=value;}
        } 


        public MariniGenericObject()
        {
            System.Timers.Timer tTimer = new System.Timers.Timer();
            tTimer.Elapsed+=new ElapsedEventHandler(OnTimedEvent);
            tTimer.Interval=1000;
            tTimer.Enabled=true;

        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Manage();
        }




        //[System.Xml.Serialization.XmlElementAttribute("id")]
        [System.Xml.Serialization.XmlAttribute]
        public string id { get; set; }
        [System.Xml.Serialization.XmlAttribute]
        public string name { get; set; }
        [System.Xml.Serialization.XmlAttribute]
        public string description { get; set; }

        /* LG: Qui prima era private readonly, ma ho messo public per far funzionare la serialization */
        //[XmlArray("Lista")]
        //[XmlElement("result")]
        //[XmlArrayItem("impianto", Type = typeof(MariniImpianto))]
        //[XmlArrayItem("zona", Type = typeof(MariniZona))]
        //[XmlArrayItem("predosatore", Type = typeof(MariniPredosatore))]
        //[XmlArrayItem("plctag", Type = typeof(MariniPlctag))]
        //[XmlArrayItem("bilancia", Type = typeof(MariniBilancia))]
        //[XmlArrayItem("motore", Type = typeof(MariniMotore))]
        //[XmlArrayItem("nastro", Type = typeof(MariniNastro))]
        //[XmlArrayItem("amperometro", Type = typeof(MariniAmperometro))]
        //[XmlArrayItem("oggettobase", Type = typeof(MariniOggettoBase))]
        [XmlElement("impianto", Type = typeof(MariniImpianto))]
        [XmlElement("zona", Type = typeof(MariniZona))]
        [XmlElement("predosatore", Type = typeof(MariniPredosatore))]
        [XmlElement("plctag", Type = typeof(MariniPlctag))]
        [XmlElement("bilancia", Type = typeof(MariniBilancia))]
        [XmlElement("motore", Type = typeof(MariniMotore))]
        [XmlElement("nastro", Type = typeof(MariniNastro))]
        [XmlElement("amperometro", Type = typeof(MariniAmperometro))]
        [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
        public readonly List<MariniGenericObject> _listaGenericObject = new List<MariniGenericObject>();
        
        public IList<MariniGenericObject> ListaGenericObject { get { return _listaGenericObject; } }

        
        
        
        public abstract void AutoManage();

        public void AutoManageAll()
        {

            AutoManage();


            foreach (MariniGenericObject mgo in _listaGenericObject)
            {
                mgo.AutoManageAll();
            }
            return;
        }

        public void Manage()
        {
            // scateno evento
            if (m_onManage!=null)
                m_onManage(this, new OnManageEventArgs(0));

            if(Changed)
            {
                if (m_onChange!= null)
                    m_onChange(this, new OnChangeEventArgs(0));
                Changed = false;

            }
        }


        // gestione evento
        public delegate void ManageHandler(object sender, OnManageEventArgs e);
        private event ManageHandler m_onManage;
        public event ManageHandler OnManage
        {
            add {m_onManage+=value;}
            remove {m_onManage-=value;}
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
        public override void AutoManage()
        {
            Console.WriteLine("Sono un oggetto base");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
            //Thread.Sleep(500);
        }
    }

    public class MariniImpianto : MariniGenericObject
    {
        private bool _start;

        public bool Start {
            
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

        public override void AutoManage()
        {
            Console.WriteLine("Sono un impianto");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
        }
    }


    public class MariniZona : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un zona");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
            //Thread.Sleep(500);
        }
    }

    public class MariniPredosatore : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un predosatore");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
            //Thread.Sleep(500);
        }
    }

    public class MariniPlctag : MariniGenericObject
    {

        private bool _value;

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

        public MariniPlctag(string tagid)
        {
            this.tagid = tagid;
        }

        public MariniPlctag():this("NO_PLCTAG")
        {
        }


        [System.Xml.Serialization.XmlAttribute]
        public string tagid { get; set; }

        public override void AutoManage()
        {
            Console.WriteLine("Sono un plctag");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2} tagid: {3}", id, name, description, tagid);
        }
    }

    public class MariniBilancia : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un bilancia");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniMotore : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un motore");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name,description);
        }
    }

    public class MariniNastro : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un nastro");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
        }
    }

    public class MariniAmperometro : MariniGenericObject
    {
        public override void AutoManage()
        {
            Console.WriteLine("Sono un amperometro");
            Console.WriteLine("AutoManage id: {0} name: {1} description: {2}", id, name, description);
        }
    }
           
}
