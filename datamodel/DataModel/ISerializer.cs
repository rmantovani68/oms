using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface ISerializer
    {
        /// <summary>
        /// Serialize a specific object of DataModel.
        /// </summary>
        /// <param name="path">the path of the object.</param>
        /// <returns>A string that contains the serialized object</returns>
        string Serialize(GenericObject mgo);
    }
}
