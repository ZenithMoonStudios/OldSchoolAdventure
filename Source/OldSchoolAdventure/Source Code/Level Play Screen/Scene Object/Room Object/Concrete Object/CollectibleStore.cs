using Destiny;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Collectible store class.
    /// </summary>
    public class CollectibleStore
    {
        public delegate void CollectNotifier(string p_CollectibleObjectPath, int p_Quantity);
        public delegate void UseNotifier(string p_CollectibleObjectPath);
        public event UseNotifier OnUse;

        /// <summary>
        /// Stored objects.
        /// </summary>
        public SerializableDictionary<string, int> ObjectTypePathToCount { get; set; }  = null;

        /// <summary>
        /// Collectibles that have been collected.
        /// </summary>
        public List<string> HasCollectedHistory { get; set; } = null;

        /// <summary>
        /// Doors that have been unlocked.
        /// </summary>
        public List<string> HasUnlockedHistory { get; set; } = null;

        /// <summary>
        /// Active power.
        /// </summary>
        public CollectiblePower Power { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CollectibleStore()
        {
            this.ObjectTypePathToCount = new SerializableDictionary<string, int>();
            this.HasCollectedHistory = new List<string>();
            this.HasUnlockedHistory = new List<string>();
        }

        /// <summary>
        /// Determine whether the store contains at least one of the specified object.
        /// </summary>
        /// <param name="p_LevelPath">Object type path.</param>
        /// <returns>Whether the store contains at least one of the specified object.</returns>
        public bool Has(string p_ObjectTypePath)
        {
            // Lookup dictionary
            bool result = this.ObjectTypePathToCount.ContainsKey(p_ObjectTypePath) && this.ObjectTypePathToCount[p_ObjectTypePath] > 0;

            // If not in dictionary, check whether the specified path matches the current power
            result |= this.HasPower(p_ObjectTypePath);

            return result;
        }

        bool HasPower(string p_ObjectTypePath)
        {
            return (this.Power != null && this.Power.ObjectTypePath == p_ObjectTypePath);
        }

        /// <summary>
        /// Determine how many of the specified object are in the store.
        /// </summary>
        /// <param name="p_LevelPath">Object type path.</param>
        /// <returns>How many of the specified object are in the store.</returns>
        public int Quantity(string p_ObjectTypePath)
        {
            int quantity = this.ObjectTypePathToCount.ContainsKey(p_ObjectTypePath) ? this.ObjectTypePathToCount[p_ObjectTypePath] : 0;
            if (quantity == 0 && this.HasPower(p_ObjectTypePath))
            {
                quantity = 1;
            }
            return quantity;
        }

        /// <summary>
        /// Whether the specified object has ever been collected.
        /// </summary>
        public bool HasCollected(Collectible p_Collectible)
        {
            return this.HasCollectedHistory.Contains(p_Collectible.RepeatableId);
        }

        /// <summary>
        /// Whetther the specified door has ever been unlocked.
        /// </summary>
        public bool HasUnlocked(Door p_Door)
        {
            return this.HasUnlockedHistory.Contains(p_Door.RepeatableId);
        }

        /// <summary>
        /// Collect the specified object.
        /// </summary>
        public void Collect(Collectible p_Collectible)
        {
            if (p_Collectible.Power != null)
            {
                // Discard old power
                if (this.Power != null)
                {
                    this.Use(this.Power.OwnerPath);
                }

                // Set new power
                this.Power = p_Collectible.Power;
            }
            else
            {
                this.Collect(p_Collectible.ObjectTypePath, 1);
                this.HasCollectedHistory.Add(p_Collectible.RepeatableId);
            }
        }

        /// <summary>
        /// Collect the specified object.
        /// </summary>
        public void Collect(string p_ObjectTypePath, int p_Quantity)
        {
            if (!this.ObjectTypePathToCount.ContainsKey(p_ObjectTypePath))
            {
                this.ObjectTypePathToCount.Add(p_ObjectTypePath, 0);
            }
            this.ObjectTypePathToCount[p_ObjectTypePath] += p_Quantity;
        }

        /// <summary>
        /// Use the specified object.
        /// </summary>
        public void Use(string p_ObjectTypePath)
        {
            this.Use(p_ObjectTypePath, 1);
        }

        /// <summary>
        /// Use the specified object.
        /// </summary>
        public void Use(string p_ObjectTypePath, int p_Quantity)
        {
            bool doUse = true;
            if (doUse && this.Has(p_ObjectTypePath))
            {
                this.ObjectTypePathToCount[p_ObjectTypePath] -= p_Quantity;
                if (this.OnUse != null)
                {
                    this.OnUse(p_ObjectTypePath);
                }
            }
        }

        /// <summary>
        /// Remember that a door has been unlocked.
        /// </summary>
        public void UnlockDoor(Door p_Door)
        {
            this.HasUnlockedHistory.Add(p_Door.RepeatableId);
        }

        /// <summary>
        /// Audio manager.
        /// </summary>
        AudioManager AudioManager { get { return PlatformerGame.Instance.AudioManager; } }
    }
}
