using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Room object shoot information class.
    /// </summary>
    public class ShootInformation
    {
        public int Duration { get; private set; }
        public int MinWait { get; private set; }
        public int MaxWait { get; private set; }
        public bool FlipOnNegativeX { get; private set; }
        public bool FlipOnNegativeY { get; private set; }
        public string ObjectPath { get; private set; }
        public int ObjectMaxCount { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 StartVelocity { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShootInformation(OSATypes.TemplateShootInformation shootinformation)
        {

            // Load main properties
            int inttest;
            bool booltest;
            this.Duration = int.TryParse(shootinformation.Duration, out inttest) ? int.Parse(shootinformation.Duration) : 60;
            this.MinWait = int.TryParse(shootinformation.MinWait, out inttest) ? int.Parse(shootinformation.MinWait) : 240;
            this.MaxWait = int.TryParse(shootinformation.MaxWait, out inttest) ? int.Parse(shootinformation.MaxWait) : this.MinWait;
            this.FlipOnNegativeX = bool.TryParse(shootinformation.FlipOnNegativeX, out booltest) ? bool.Parse(shootinformation.FlipOnNegativeX) : true;
            this.FlipOnNegativeY = bool.TryParse(shootinformation.FlipOnNegativeY, out booltest) ? bool.Parse(shootinformation.FlipOnNegativeY) : true;

            // Load object properties
            if (shootinformation.Object != null && shootinformation.Object.Length > 0)
            {
                this.ObjectPath = shootinformation.Object[0].Path != null ? shootinformation.Object[0].Path : string.Empty;
                this.ObjectMaxCount = int.TryParse(shootinformation.Object[0].MaxCount, out inttest) ? int.Parse(shootinformation.Object[0].MaxCount) : 1;
            }
            // Load start position and velocity
            this.StartPosition = shootinformation.StartPosition != null && shootinformation.StartPosition.Length > 0 ? OSATypes.Functions.LoadVector(shootinformation.StartPosition[0].X, shootinformation.StartPosition[0].Y) : Vector2.Zero;
            this.StartVelocity = shootinformation.StartVelocity != null && shootinformation.StartVelocity.Length > 0 ? OSATypes.Functions.LoadVector(shootinformation.StartVelocity[0].X, shootinformation.StartVelocity[0].Y) : Vector2.Zero;
        }
    }
}
