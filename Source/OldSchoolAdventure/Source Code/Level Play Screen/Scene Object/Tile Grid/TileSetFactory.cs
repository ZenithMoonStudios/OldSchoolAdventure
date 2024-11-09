using Destiny;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Tile set factory class.
    /// </summary>
    public static class TileSetFactory
    {
        public delegate TileTemplate TileTemplateGetter(string p_Path);

        /// <summary>
        /// Load the tile set.
        /// </summary>
        /// <param name="p_LevelPath">Path to tile set.</param>
        /// <param name="p_ContentManager">Content manager.</param>
        /// <param name="p_GetTileTemplateDelegate">Method to get a tile template.</param>
        /// <returns>Tile set.</returns>
        public static TileSet Load(
            string p_ObjectTypePath,
            ContentManager p_ContentManager,
            TileTemplateGetter p_GetTileTemplateDelegate
            )
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OSATypes.TileSet));
#if (MOBILE)
			Stream fs = TitleContainer.OpenStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"));
#else
            FileStream fs = new FileStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"), FileMode.Open);
#endif
            OSATypes.TileSet tilesetLoad;
            tilesetLoad = (OSATypes.TileSet)serializer.Deserialize(fs);
            OSATypes.TileSetTileSize loadSize = new OSATypes.TileSetTileSize();
            OSATypes.TileSetTileTemplates loadTemplate = new OSATypes.TileSetTileTemplates(); ;
            foreach (object item in tilesetLoad.Items)
            {
                switch (item.GetType().ToString())
                {
                    case "OSATypes.TileSetTileSize":
                        loadSize = (OSATypes.TileSetTileSize)item;
                        break;
                    case "OSATypes.TileSetTileTemplates":
                        loadTemplate = (OSATypes.TileSetTileTemplates)item;
                        break;
                }
            }


            Size tileSize = new Size(int.Parse(loadSize.Width), int.Parse(loadSize.Height));

            TileSet tileSet = new TileSet(tileSize);
            for (int tileTemplateIndex = 0; tileTemplateIndex < loadTemplate.TileTemplate.Length; tileTemplateIndex++)
            {
                OSATypes.TileSetTileTemplatesTileTemplate tileTemplate = loadTemplate.TileTemplate[tileTemplateIndex];
                string tileTemplatePath = tileTemplate.Path != null ? tileTemplate.Path : string.Empty;
                //char tileTemplateGridSymbol = tileTemplate.GridSymbol != null ? char.Parse(tileTemplate.GridSymbol) : new char();
                char[] tileTemplateGridSymbolarray = tileTemplate.GridSymbol != null ? tileTemplate.GridSymbol.ToCharArray() : new char[1];
                char tileTemplateGridSymbol = tileTemplateGridSymbolarray.Length > 0 ? tileTemplateGridSymbolarray[0] : new char();

                TileTemplate tileTemplatedef = p_GetTileTemplateDelegate(tileTemplatePath);
                tileTemplatedef.Height = tileTemplate.Height != null ? int.Parse(tileTemplate.Height) : 0;
                tileSet.RegisterTileTemplate(tileTemplatedef, tileTemplateGridSymbol);
            }

            return tileSet;
        }
    }
}
