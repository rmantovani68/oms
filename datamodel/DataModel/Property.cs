using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;
// Libreria per far funzionare InotifyPropertyChanged.
// InotifyPropertyChanged è l'interfaccia per notificare a clienti (in generale binding clients) che una
// proprietà è cambiata
using System.ComponentModel;
// Libreria per far funzionare l'attributo [CallerMemberName] presente nella OnPropertyChanged(…).
// In questo modo non ci si deve più preoccupare di rinominare le proprietà.
using System.Runtime.CompilerServices;

namespace DataModel
{
    
    public class Property : BaseObject
    {
        private string _bind;
        [System.Xml.Serialization.XmlAttribute]
        public string bind { get { return _bind; } set { _bind = value; } }

        private BindType _bindtype;
        [System.Xml.Serialization.XmlAttribute]
        public BindType bindtype { get { return _bindtype; } set { _bindtype = value; } }

        private BindDirection _binddirection;
        [System.Xml.Serialization.XmlAttribute]
        public BindDirection binddirection { get { return _binddirection; } set { _binddirection = value; } }

        private PersistenceType _persistence;
        [System.Xml.Serialization.XmlAttribute]
        public PersistenceType persistence { get { return _persistence; } set { _persistence = value; } }

        private PropertyType _propertytype;
        [System.Xml.Serialization.XmlAttribute]
        public PropertyType propertytype { get { return _propertytype; } set { _propertytype = value; } }

        private object _value;
        [System.Xml.Serialization.XmlIgnore]
        public object value { get { return _value; } set { SetField(ref _value, value); } }
        [System.Xml.Serialization.XmlAttribute("value")]
        public string valuestring { get { if (_value != null) { return _value.ToString(); } else { return "NO_VALUE"; }; } set { valuestring = value; } }

        public Property(GenericObject parent)
            : base(parent)
        {
        }

        public Property()
            : base()
        {
        }


        public Property(GenericObject parent, XmlNode node)
            : base(parent, node)
        {
            if (node.Attributes != null)
            {
                // per gestire value devo fare un doppio passaggio, perche' altrimenti se viene scritta nell'XML prima del propertytype
                // (di default a Int) si rischia di falire il Parse
                string _s = "";
                XmlAttributeCollection attrs = node.Attributes;
                value = null;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);
                    switch (attr.Name)
                    {

                        case "bind":
                            bind = attr.Value;
                            break;
                        case "bindtype":
                            bindtype = (BindType)Enum.Parse(typeof(BindType), attr.Value, true);
                            break;
                        case "binddirection":
                            binddirection = (BindDirection)Enum.Parse(typeof(BindDirection), attr.Value, true);
                            break;
                        case "persistence":
                            persistence = (PersistenceType)Enum.Parse(typeof(PersistenceType), attr.Value, true);
                            break;
                        case "propertytype":
                            propertytype = (PropertyType)Enum.Parse(typeof(PropertyType), attr.Value, true);
                            break;
                        case "value":
                            _s = attr.Value;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(_s))
                    value = ParsePropertyValue(propertytype, _s);
            }
        }

        public Property(XmlNode node)
            : this(null, node)
        {

        }

        /// <summary>
        /// Occurs when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler pPropertyChanged;

        /// <summary>
        /// Raises the <see cref="pPropertyChanged">PropertyChanged</see> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler ehandler = pPropertyChanged;
            if (ehandler != null)
            {
                ehandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private object ParsePropertyValue(PropertyType type, string Value)
        {
            try
            {
                switch (type)
                {
                    case PropertyType.Bool: return bool.Parse(Value);
                    case PropertyType.Byte: return Byte.Parse(Value);
                    case PropertyType.Dint: return int.Parse(Value);
                    case PropertyType.Int: return int.Parse(Value);
                    case PropertyType.Long: return int.Parse(Value);
                    case PropertyType.Real: return int.Parse(Value);
                    case PropertyType.Word: return short.Parse(Value);
                }

            }
            catch (Exception e)
            {

                throw new Exception(String.Format( "Errore in ParsePropertyValue({0},{1}): parse non riuscito", type, Value), e);
            }
            throw new Exception(String.Format("Errore in ParsePropertyValue({0},{1}): passato un type {0} non gestito", type, Value));
        }

        //protected bool SetField<T>(ref T field, T value, string propertyName)
        /// <summary>
        /// Used in every property set to launch <see cref="GenericObject.OnPropertyChanged"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns><c>true</c> if the property is effectively changed; otherwise, <c>false</c>.</returns>
        protected bool SetPropertyField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
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


        public override void ToPlainText()
        {
            Console.WriteLine("Sono una property id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }

       
    }
}
