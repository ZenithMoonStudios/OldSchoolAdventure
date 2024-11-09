using Destiny;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Level introduction page class.
    /// </summary>
    public class LevelIntroPage
    {
        /// <summary>
        /// If a line has no text, replace with this.
        /// </summary>
        private static string c_EmptyLine = "               ";

        /// <summary>
        /// Lines of text on the page.
        /// </summary>
        public StringList Lines { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelIntroPage(StringList p_Lines)
        {
            this.Lines = new StringList();
            foreach (string line in p_Lines)
            {
                this.Lines.Add(!string.IsNullOrEmpty(line) ? line : c_EmptyLine);
            }
        }
    }

    /// <summary>
    /// Level introduction page list class.
    /// </summary>
    public class LevelIntroPageList : List<LevelIntroPage> { }
}
