using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IMariniSerializer
    {
        /// <summary>
        /// Serialize a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">the path of the object.</param>
        /// <returns>A string that contains the serialized object</returns>
        string Serialize(MariniGenericObject mgo);
    }
}
