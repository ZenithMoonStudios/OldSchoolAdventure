using Destiny;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Tile set class.
    /// </summary>
    public class TileSet
    {
        public Size TileSize { get; private set; }

        public List<TileTemplate> TileTemplates { get; private set; }
        public int MaxHeight { get; private set; }

        private Dictionary<char, TileTemplate> m_TileTemplateBySymbol = new Dictionary<char, TileTemplate>();
        private Dictionary<TileTemplate, char> m_TileTemplateToSymbol = new Dictionary<TileTemplate, char>();

        public TileSet(Size p_TileSize)
        {
            this.TileSize = p_TileSize;
            this.TileTemplates = new List<TileTemplate>();
            this.MaxHeight = 0;
        }

        public void RegisterTileTemplate(TileTemplate p_TileTemplate, char p_Symbol)
        {
            this.TileTemplates.Add(p_TileTemplate);
            m_TileTemplateBySymbol.Add(p_Symbol, p_TileTemplate);
            m_TileTemplateToSymbol.Add(p_TileTemplate, p_Symbol);
            this.MaxHeight = Math.Max(this.MaxHeight, p_TileTemplate.Height);
        }

        public TileTemplate TileTemplateBySymbol(char p_Symbol)
        {
            TileTemplate tileTemplate = null;
            if (m_TileTemplateBySymbol.ContainsKey(p_Symbol))
            {
                tileTemplate = m_TileTemplateBySymbol[p_Symbol];
            }
            return tileTemplate;
        }

        public char TileTemplateToSymbol(TileTemplate p_TileTemplate)
        {
            return m_TileTemplateToSymbol[p_TileTemplate];
        }
    }
}
