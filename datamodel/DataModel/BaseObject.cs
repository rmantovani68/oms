using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;


namespace DataModel
{

    public class BaseObject : GenericObject
    {

        public BaseObject(GenericObject parent, string type, string id, string name, string description)
            : base(parent, type, id, name, description)
        {
        }

        public BaseObject(GenericObject parent, string type, string id, string name)
            : base(parent, type, id, name)
        {
        }

        public BaseObject(GenericObject parent, string type, string id)
            : base(parent, type, id)
        {
        }

        public BaseObject(GenericObject parent, string type)
            : base(parent, type)
        {
        }

        public BaseObject(GenericObject parent)
            : base(parent)
        {
        }

        public BaseObject()
            : base()
        {
        }

        public BaseObject(GenericObject parent, XmlNode node)
            : base(parent, node)
        {
        }

        public BaseObject(XmlNode node)
            : base(node)
        {
        }

        // TODO: magari da spostare in un eventuale altro oggetto agente che faccia cose
        // sul data model???

        
        /// <summary>
        /// Retrieve the Property bound to the property prop_id
        /// </summary>
        /// <param name="prop_id">the property bound to the prop_id</param>
        /// <returns></returns>
        public Property GetPropertyFromId(string prop_id)
        {

            return ChildList
                .Where(mgo => mgo.GetType() == typeof(Property))
                .Cast<Property>()
                .FirstOrDefault(mp => mp.id == prop_id);
        }

        /// <summary>
        /// Retrieve the MariniProperty bound to the bind item
        /// </summary>
        /// <param name="bind">the property bound to the bind item</param>
        /// <returns></returns>
        public Property GetPropertyFromBoundItem(string bind)
        {

            return ChildList
                .Where(mgo => mgo.GetType() == typeof(Property))
                .Cast<Property>()
                .FirstOrDefault(mp => mp.bind == bind);
        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un oggetto base id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }
}
