using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Game condition class.
    /// </summary>
    public class GameCondition
    {
        public bool HasItem { get; private set; }
        public string CollectibleObjectTypePath { get; private set; }
        public int Quantity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameCondition(bool p_HasItem, string p_CollectibleObjectTypePath, int p_Quantity)
        {
            this.HasItem = p_HasItem;
            this.CollectibleObjectTypePath = p_CollectibleObjectTypePath;
            this.Quantity = p_Quantity;
        }

        /// <summary>
        /// Whether the condition is true.
        /// </summary>
        public bool IsTrue(CollectibleStore p_Store)
        {
            bool result = false;
            if (this.HasItem)
            {
                result = (p_Store.Quantity(this.CollectibleObjectTypePath) >= this.Quantity);
            }
            else
            {
                result = (p_Store.Quantity(this.CollectibleObjectTypePath) < this.Quantity);
            }
            return result;
        }
    }

    /// <summary>
    /// Game condition list.
    /// </summary>
    public class GameConditionList : List<GameCondition>
    {
        /// <summary>
        /// Whether all conditions are true.
        /// </summary>
        public bool IsTrue(CollectibleStore p_Store)
        {
            bool result = true;
            for (int i = 0; i < this.Count; i++)
            {
                GameCondition condition = this[i];
                if (!condition.IsTrue(p_Store))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public static class XmlGameConditionListReader
    {
        public static GameConditionList Load(OSATypes.Conditions[] p_conditions)
        {
            GameConditionList conditions = new GameConditionList();
            if (p_conditions != null && p_conditions.Length > 0)
            {
                foreach (OSATypes.Conditions p_XmlConditionsNode in p_conditions)
                    if (p_XmlConditionsNode.Condition != null && p_XmlConditionsNode.Condition.Count > 0)
                        for (int i = 0; i < p_XmlConditionsNode.Condition.Count; i++)
                        {
                            OSATypes.ConditionsCondition xmlConditionNode = p_XmlConditionsNode.Condition[i];
                            bool hasItem = xmlConditionNode != null && bool.TryParse(xmlConditionNode.Has, out hasItem) ? bool.Parse(xmlConditionNode.Has) : true;
                            string item = xmlConditionNode.Item != null ? xmlConditionNode.Item : string.Empty;
                            int quantity = xmlConditionNode.Quantity != null && int.TryParse(xmlConditionNode.Quantity, out quantity) ? int.Parse(xmlConditionNode.Quantity) : 1;
                            conditions.Add(new GameCondition(hasItem, item, quantity));
                        }
            }
            return conditions;
        }

        public static GameConditionList Load(List<OSATypes.Conditions> p_conditions)
        {
            GameConditionList conditions = new GameConditionList();
            if (p_conditions != null && p_conditions.Count > 0)
            {
                foreach (OSATypes.Conditions p_XmlConditionsNode in p_conditions)
                    if (p_XmlConditionsNode.Condition != null && p_XmlConditionsNode.Condition.Count > 0)
                        for (int i = 0; i < p_XmlConditionsNode.Condition.Count; i++)
                        {
                            OSATypes.ConditionsCondition xmlConditionNode = p_XmlConditionsNode.Condition[i];
                            bool hasItem = xmlConditionNode != null && bool.TryParse(xmlConditionNode.Has, out hasItem) ? bool.Parse(xmlConditionNode.Has) : true;
                            string item = xmlConditionNode.Item != null ? xmlConditionNode.Item : string.Empty;
                            int quantity = xmlConditionNode.Quantity != null && int.TryParse(xmlConditionNode.Quantity, out quantity) ? int.Parse(xmlConditionNode.Quantity) : 1;
                            conditions.Add(new GameCondition(hasItem, item, quantity));
                        }
            }
            return conditions;
        }

    }

}
