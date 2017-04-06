// Load a TreeView control from an XML file.
private void LoadTreeViewFromXmlFile(string filename, TreeView trv)
{
    // Load the XML document.
    XmlDocument xml_doc = new XmlDocument();
    xml_doc.Load(filename);

    // Add the root node's children to the TreeView.
    trv.Nodes.Clear();
    AddTreeViewChildNodes(trv.Nodes, xml_doc.DocumentElement);
}



// Add the children of this XML node
// to this child nodes collection.
private void AddTreeViewChildNodes( TreeNodeCollection parent_nodes, XmlNode xml_node)
{
    foreach (XmlNode child_node in xml_node.ChildNodes)
    {
        // Make the new TreeView node.
        TreeNode new_node = parent_nodes.Add(child_node.Name);

        // Recursively make this node's descendants.
        AddTreeViewChildNodes(new_node.Nodes, child_node);

        // If this is a leaf node, make sure it's visible.
        if (new_node.Nodes.Count == 0) new_node.EnsureVisible();
    }
}
