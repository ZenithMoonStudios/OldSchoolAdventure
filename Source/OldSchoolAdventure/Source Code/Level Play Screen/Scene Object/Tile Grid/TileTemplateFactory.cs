using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Tile template factory class.
    /// </summary>
    public static class TileTemplateFactory
    {
        /// <summary>
        /// Load the tile template.
        /// </summary>
        /// <param name="p_LevelPath">Path to tile template.</param>
        /// <param name="p_ContentManager">Content manager.</param>
        /// <returns>Tile template.</returns>
        public static TileTemplate Load(string p_ObjectTypePath, ContentManager p_ContentManager)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OSATypes.TileTemplate));
#if (MOBILE)
			Stream fs = TitleContainer.OpenStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"));
#else
            FileStream fs = new FileStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"), FileMode.Open);
#endif
            OSATypes.TileTemplate tileTemplateLoad;
            tileTemplateLoad = (OSATypes.TileTemplate)serializer.Deserialize(fs);

            Size tileSize = tileTemplateLoad.TileSize != null ? new Size(int.Parse(tileTemplateLoad.TileSize.Width), int.Parse(tileTemplateLoad.TileSize.Width)) : new Size();

            Vector2 acceleration = tileTemplateLoad.Acceleration != null ? OSATypes.Functions.LoadVector(tileTemplateLoad.Acceleration) : Vector2.Zero;
            float friction = tileTemplateLoad.Friction != null ? float.Parse(tileTemplateLoad.Friction) : 0.25f;
            bool isDeadly = tileTemplateLoad.IsDeadly != null ? bool.Parse(tileTemplateLoad.IsDeadly) : false;
            int offense = tileTemplateLoad.Offense != null ? int.Parse(tileTemplateLoad.Offense) : 0;
            int defense = tileTemplateLoad.Defense != null ? int.Parse(tileTemplateLoad.Defense) : 0;
            bool compensateForGravityChanges = tileTemplateLoad.CompensateForGravityChanges != null ? bool.Parse(tileTemplateLoad.CompensateForGravityChanges) : false;
            if (isDeadly && offense == 0)
            {
                offense = 1;
            }

            SurfaceInformation leftSurface = SurfaceInformation.Load(SurfaceDirections.Left, tileTemplateLoad.Left, friction, offense, defense);
            SurfaceInformation topSurface = SurfaceInformation.Load(SurfaceDirections.Top, tileTemplateLoad.Top, friction, offense, defense);
            SurfaceInformation rightSurface = SurfaceInformation.Load(SurfaceDirections.Right, tileTemplateLoad.Right, friction, offense, defense);
            SurfaceInformation bottomSurface = SurfaceInformation.Load(SurfaceDirections.Bottom, tileTemplateLoad.Bottom, friction, offense, defense);

            return new TileTemplate(p_ObjectTypePath, tileSize, acceleration, friction, offense, defense, compensateForGravityChanges, leftSurface, topSurface, rightSurface, bottomSurface);
        }
    }
}
