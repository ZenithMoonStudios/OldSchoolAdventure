using Microsoft.Xna.Framework;
using System;

namespace OSATypes
{
    public static class Functions
    {
        public static void GetType(String input, out Type type)
        {
            Type output = Type.GetType(input);
            type = output;
        }

        public static Type GetType(String input)
        {
            return Type.GetType(input);
        }

        public enum RoomObjectTypes
        {
            CharacterTemplate,
            DoorTemplate,
            CollectibleTemplate,
            ObstacleTemplate,
            DirectorTemplate,
            SpawnPointTemplate,
            SpeakerTemplate,
            MusicPointTemplate,
            SwitchTemplate,
            EnemyTemplate,
            ProjectileTemplate
        }

        public static Type GetTypeFromFolder(String input)
        {
            String Folder;

            Folder = input.IndexOf("/") > 0 ? input.Substring(0, input.IndexOf("/")) : Folder = input;

            switch (Folder)
            {
                case "MusicPoint":
                    input = "SpawnPoint";
                    break;
                default:
                    input = Folder;
                    break;
            }

            return GetType("OSATypes." + input + "Template");
        }

        /// <summary>
        /// Load vector.
        /// </summary>
        public static Vector2 LoadVector(String Xin, String Yin)
        {
            // X
            float x = float.TryParse(Xin, out x) == true ? float.Parse(Xin) : 0f;
            //x = LoadFloat(p_XmlVectorNode, "Left", x);
            //x = LoadFloat(p_XmlVectorNode, "Width", x);

            // Y
            float y = float.TryParse(Yin, out y) == true ? float.Parse(Yin) : 0f;
            //y = LoadFloat(p_XmlVectorNode, "Top", y);
            //y = LoadFloat(p_XmlVectorNode, "Height", y);

            return new Vector2(x, y);
        }

        public static Rectangle LoadRectangle(string xin, string yin, string widthin, string heightin)
        {
            int x = int.TryParse(xin, out x) ? int.Parse(xin) : 0;
            int y = int.TryParse(yin, out y) == true ? int.Parse(yin) : 0;
            int width = int.TryParse(widthin, out width) == true ? int.Parse(widthin) : 0;
            int height = int.TryParse(heightin, out height) == true ? int.Parse(heightin) : 0;


            return new Rectangle(x, y, width, height);
        }


        public static Rectangle LoadRectangle(OSATypes.VisualsSpritesSpriteTextureRegion item)
        {
            return LoadRectangle(item.Left, item.Top, item.Width, item.Height);
        }

        public static Vector2 LoadVector(OSATypes.VisualsAnimationsAnimationDisplayOffset item)
        {
            return LoadVector(item.Width, item.Height);
        }

        public static Vector2 LoadVector(OSATypes.TileTemplateAcceleration item)
        {
            return LoadVector(item.X, item.Y);
        }

        public static Vector2 LoadVector(OSATypes.TileTemplateTileSize item)
        {
            return LoadVector(item.Width, item.Height);
        }

        public static Vector2 LoadVector(OSATypes.TileSetTileSize item)
        {
            return LoadVector(item.Width, item.Height);
        }

        public static Vector2 LoadVector(OSATypes.StartVelocity item)
        {
            return LoadVector(item.X, item.Y);
        }

        public static Vector2 LoadVector(OSATypes.TemplateSize item)
        {
            return LoadVector(item.Width, item.Height);
        }

        public static Vector2 LoadVector(OSATypes.GridPosition item)
        {
            return LoadVector(item.Left, item.Top);
        }

        public static Vector2 LoadVector(OSATypes.RoomSize item)
        {
            return LoadVector(item.Width, item.Height);
        }
    }
}
