using Destiny;
using Microsoft.Xna.Framework;
using System;

namespace OSA
{
    /// <summary>
    /// Xml room object reader.
    /// </summary>
    public abstract class XmlRoomObjectReader
    {
        public XmlRoomObjectReader()
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public abstract RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_Template, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid);

        /// <summary>
        /// Get template properties.
        /// </summary>
        public static void GetTemplateProperties(OSATypes.RoomObjectTemplate p_Template, out Vector2 p_Size, out Vector2 p_Velocity, out VerticalAlignment p_VerticalAlignment)
        {
            // Get size
            p_Size = p_Template.Size != null && p_Template.Size.Length > 0 ? OSATypes.Functions.LoadVector(p_Template.Size[0]) : Vector2.Zero;

            // Get velocity
            p_Velocity = p_Template.StartVelocity != null && p_Template.StartVelocity.Length > 0 ? OSATypes.Functions.LoadVector(p_Template.StartVelocity[0]) : Vector2.Zero;

            // Get vertical alignment type
            p_VerticalAlignment = VerticalAlignment.Center;
            if (p_Template.VerticalAlignment != null)
            {
                p_VerticalAlignment = VerticalAlignmentHelper.FromString(p_Template.VerticalAlignment);
            }
        }

        /// <summary>
        /// Get initialization info.
        /// </summary>
        public static RoomObjectInitialization GetInitializationInfo(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_Template)
        {
            return GetInitializationInfo(p_ObjectTypePath, item, p_Template, Vector2.Zero, null);
        }

        /// <summary>
        /// Get initialization info.
        /// </summary>
        public static RoomObjectInitialization GetInitializationInfo(
            string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_Template,
            Vector2 p_PreferredGridPosition, TileGrid p_TileGrid
            )
        {
            // Template properties
            string renderMode = p_Template.RenderMode != null ? p_Template.RenderMode : "Default";
            Vector2 size;
            Vector2 startVelocity;
            VerticalAlignment verticalAlignment;
            GetTemplateProperties(p_Template, out size, out startVelocity, out verticalAlignment);

            // Shoot definition
            ShootInformation shootInformation = null;
            if (p_Template.ShootInformation != null && p_Template.ShootInformation.Length > 0)
            {
                shootInformation = new ShootInformation(p_Template.ShootInformation[0]);
            }

            // Object attributes
            string name = item != null && item.Name != null ? item.Name : String.Empty;

            Vector2 gridPosition = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            if (p_TileGrid != null)
            {
                gridPosition = p_PreferredGridPosition;
                if (gridPosition == Vector2.Zero && item != null)
                {
                    gridPosition = item.GridPosition != null && item.GridPosition.Count > 0 ? OSATypes.Functions.LoadVector(item.GridPosition[0]) : Vector2.Zero;
                }
                position = RoomHelper.GetPositionFromGridPosition(size, verticalAlignment, gridPosition, p_TileGrid);
            }

            // Load providers
            OSATypes.TemplateMovementProvider xmlObjectMovementProviderNode = null;
            if (item != null && item.MovementProvider != null && item.MovementProvider.Count > 0)
            {
                xmlObjectMovementProviderNode = item.MovementProvider[0];
            }
            RoomObjectMovementProvider movementProvider = null;
            if (p_Template.MovementProvider != null) movementProvider = RoomObjectMovementProviderFactory.Instance.GetProvider(
                xmlObjectMovementProviderNode, p_Template.MovementProvider[0]
                );

            // Offense and defense
            int offense = p_Template.Offense != null && int.TryParse(p_Template.Offense, out offense) ? int.Parse(p_Template.Offense) : 0;
            int defense = p_Template.Defense != null && int.TryParse(p_Template.Defense, out defense) ? int.Parse(p_Template.Defense) : 0;

            // Load audio cues
            StringToStringDictionary audioCues = null;
            if (p_Template.Audio != null)
            {
                audioCues = new StringToStringDictionary();
                for (int i = 0; i < p_Template.Audio.Length; i++)
                {
                    OSATypes.TemplateAudioCue xmlCueNode = p_Template.Audio[i].Cue[0];
                    string cueName = xmlCueNode.Name != null ? xmlCueNode.Name : string.Empty;
                    string eventName = xmlCueNode.Event != null ? xmlCueNode.Event : string.Empty;
                    audioCues.Add(eventName, cueName);
                }
            }

            // Load conditions
            GameConditionList conditions = null;
            if (item != null && item.Conditions != null && item.Conditions.Length > 0)
            {
                conditions = new GameConditionList();
                OSATypes.Conditions xmlConditionsNode = item.Conditions[0];
                for (int i = 0; i < xmlConditionsNode.Condition.Count; i++)
                {
                    OSATypes.ConditionsCondition xmlConditionNode = xmlConditionsNode.Condition[i];
                    conditions.Add(new GameCondition(bool.Parse(xmlConditionNode.Has), xmlConditionNode.Item, int.Parse(xmlConditionNode.Quantity)));
                }
            }

            // Create room object initialization
            return new RoomObjectInitialization(
                p_ObjectTypePath,
                size, position, gridPosition, startVelocity,
                name, movementProvider, verticalAlignment, renderMode,
                xmlObjectMovementProviderNode, shootInformation, offense, defense,
                audioCues, conditions
                );
        }
    }
}
