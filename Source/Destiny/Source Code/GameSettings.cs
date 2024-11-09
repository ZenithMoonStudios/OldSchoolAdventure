using System;
using System.Collections.Generic;

namespace Destiny
{
    public class GameSettings
    {
        public List<String> Levels { get; set; }
        public List<Mode> Modes { get; set; }
    }

    public class Mode
    {
        public String Name { get; set; }
        public List<String> Items { get; set; }
    }

}
