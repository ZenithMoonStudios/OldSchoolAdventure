using Destiny;
using Microsoft.Xna.Framework.Content;

namespace OSA
{
    /// <summary>
    /// Terrain template factory class.
    /// </summary>
    public static class TerrainTemplateFactory
    {
        /// <summary>
        /// Load the terrain template.
        /// </summary>
        /// <param name="p_LevelPath">Path to terrain template.</param>
        /// <param name="p_ContentManager">Content manager.</param>
        /// <returns>Terrain template.</returns>
        public static TerrainTemplate Load(string p_ObjectTypePath, ContentManager p_ContentManager)
        {

            //XmlDocument xmlDocument = XmlDocumentHelper.LoadFromTitleContainer(p_ContentManager, p_ObjectTypePath + ".xml");

            //XmlNode xmlTerrainTemplateNode = xmlDocument.ChildNodes[1];
            //TerrainPosition position = TerrainPositionHelper.FromString(xmlTerrainTemplateNode.Attributes["Position"].Value);
            //int segmentLength = Convert.ToInt32(xmlTerrainTemplateNode.Attributes["SegmentLength"].Value);
            //float friction = XmlLoadHelper.LoadFloat(xmlTerrainTemplateNode, "Friction", 0.15f);

            //XmlNode xmlAllowedIncrementsNode = xmlTerrainTemplateNode["AllowedIncrements"];
            //IntList allowedIncrements = new IntList();
            //for (int i = 0; i < xmlAllowedIncrementsNode.ChildNodes.Count; i++)
            //{
            //    XmlNode xmlAllowedIncrementNode = xmlAllowedIncrementsNode.ChildNodes[i];
            //    int incrementSize = Convert.ToInt32(xmlAllowedIncrementNode.Attributes["Size"].Value);
            //    allowedIncrements.Add(incrementSize);
            //}

            //return new TerrainTemplate(p_ObjectTypePath, position, segmentLength, friction, allowedIncrements);
            return new TerrainTemplate(p_ObjectTypePath, TerrainPosition.None, 0, 0.15f, new IntList());
        }
    }
}
