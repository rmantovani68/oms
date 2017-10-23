using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;
using DataModel;

namespace DataModelUnitTest
{
    [TestClass]
    public class DataManager_Test
    {

        /// <summary>
        ///A test for PersonName Constructor
        ///</summary>
        [TestMethod]
        public void BaseObject_ConstructorTest()
        {
            XmlDocument doc = new XmlDocument();
            string sXml =
                @"<root id=""rootId"" name=""rootName"" description=""rootDescription"" handler=""rootHandler"">
                    <child1 id=""child1Id"" name=""child1Name"" description=""child1Description"" handler=""child1Handler"">
                        <Property id=""child1Property"" name=""child1PropertyName"" propertytype=""Bool"" bind=""PLCTAG_child1"" bindtype=""PLCTag"" binddirection=""OneWay""/>
                    </child1>
                    <child2 id=""child2Id"" name=""child2Name"" description=""child2Description"" handler=""child2Handler"">
                    </child2>
                    <Property id=""rootProperty"" name=""rootPropertyName"" propertytype=""Bool"" bind=""PLCTAG_root"" bindtype=""PLCTag"" binddirection=""OneWay""/>
                </root>
                ";
            doc.LoadXml(sXml);
            XmlNode root = doc.SelectSingleNode("*");
            GenericObject rootMGO = new BaseObject(null, root);
            Assert.IsNotNull(rootMGO);
            Assert.IsInstanceOfType(rootMGO, typeof(BaseObject));
            foreach (var childMGO in rootMGO.ChildList)
            {
                if (childMGO.id == "child1Id")
                {
                    Assert.Equals(childMGO.type, "child1");
                    Assert.Equals(childMGO.path, "root.child1");
                    Assert.IsInstanceOfType(childMGO, typeof(BaseObject));
                }
            }
        }
    }
}
