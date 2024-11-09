using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OSA
{
    [XmlRoot("Game")]
    public class PlatformerGameSettings
    {
        public class GameLevel
        {
            [XmlAttribute] public string Name { get; set; }
        }
        public class GameLevelList : List<GameLevel> { }

        public class GameMode
        {
            [XmlAttribute] public string Name { get; set; }

            [XmlArray("Items")]
            [XmlArrayItem("Item")]
            public GameItemList Items { get; set; }
        }
        public class GameModeList : List<GameMode> { }

        public class GameItem
        {
            [XmlAttribute]
            public string Path { get; set; }
        }
        public class GameItemList : List<GameItem> { }

        [XmlArray("Levels")]
        [XmlArrayItem("Level")]
        public GameLevelList Levels { get; set; }

        [XmlArray("Modes")]
        [XmlArrayItem("Mode")]
        public GameModeList Modes { get; set; }
    }

    public static class GameSettingsExtensions
    {
        public static PlatformerGameSettings LoadSettings(this PlatformerGameSettings settings, Destiny.GameSettings input)
        {
            settings = new PlatformerGameSettings();
            settings.Levels = new PlatformerGameSettings.GameLevelList();
            foreach (String level in input.Levels)
                settings.Levels.Add(new OSA.PlatformerGameSettings.GameLevel() { Name = level });

            settings.Modes = new PlatformerGameSettings.GameModeList();
            foreach (Destiny.Mode modeitem in input.Modes)
            {
                OSA.PlatformerGameSettings.GameMode mode = new OSA.PlatformerGameSettings.GameMode() { Name = modeitem.Name };
                mode.Items = new PlatformerGameSettings.GameItemList();
                foreach (String modeitemitems in modeitem.Items)
                    mode.Items.Add(new OSA.PlatformerGameSettings.GameItem() { Path = modeitemitems });

                settings.Modes.Add(mode);
            }
            return settings;
        }

    }
}
