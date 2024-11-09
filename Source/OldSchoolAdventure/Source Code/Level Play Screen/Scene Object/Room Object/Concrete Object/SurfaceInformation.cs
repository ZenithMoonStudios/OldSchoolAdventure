using System;

namespace OSA
{
    /// <summary>
    /// Surface information class.
    /// </summary>
    public class SurfaceInformation
    {
        public SurfaceDirections Direction { get; private set; }
        public bool IsSolid { get; private set; }
        public float Friction { get; private set; }
        public float Velocity { get; private set; }
        public bool IsDie { get; private set; }
        public float TangentSpeed { get; private set; }

        public int Offense { get; private set; }
        public int Defense { get; private set; }

        public bool IsDeadly { get { return this.Offense > 0; } }

        public SurfaceInformation(SurfaceDirections p_Direction)
        {
            this.Direction = p_Direction;
        }

        public void Reset()
        {
            this.Set(false, 0f, 0f, false, 0, 0, 0f);
        }

        public void Set(SurfaceInformation p_Surface)
        {
            this.Set(p_Surface.IsSolid, p_Surface.Friction, p_Surface.Velocity, p_Surface.IsDie, p_Surface.Offense, p_Surface.Defense, p_Surface.TangentSpeed);
        }

        public void Set(bool p_IsSolid, float p_Friction, float p_Velocity, bool p_IsDie, int p_Offense, int p_Defense, float p_TangentSpeed)
        {
            this.Update(p_IsSolid, p_Friction, p_Velocity);
            this.IsDie = p_IsDie;
            this.Offense = p_Offense;
            this.Defense = p_Defense;
            this.TangentSpeed = p_TangentSpeed;
        }

        public void Update(bool p_IsSolid, float p_Friction, float p_Velocity)
        {
            this.IsSolid = p_IsSolid;
            this.Friction = p_Friction;
            this.Velocity = p_Velocity;
        }

        public void Compound(SurfaceInformation p_Surface)
        {
            this.IsSolid |= p_Surface.IsSolid;
            // Use the highest friction and velocity
            if (p_Surface.Friction > this.Friction) { this.Friction = p_Surface.Friction; }
            if (Math.Abs(p_Surface.Velocity) > Math.Abs(this.Velocity)) { this.Velocity = p_Surface.Velocity; }
            if (this.IsDie && p_Surface.IsDie)
            {
                // If surface is going to die, process tangent speed
                if (p_Surface.TangentSpeed > this.TangentSpeed) { this.TangentSpeed = p_Surface.TangentSpeed; }
            }
            else
            {
                // Is surface is not going to die, clear tangent speed in case it was previously set
                this.TangentSpeed = 0f;
            }
            // Use the lowest offense rating and highest defense rating to maximize safety
            if (this.Offense > p_Surface.Offense) { this.Offense = p_Surface.Offense; }
            if (this.Defense < p_Surface.Defense) { this.Defense = p_Surface.Defense; }
        }

        /// <summary>
        /// Load surface information.
        /// </summary>
        public static SurfaceInformation Load(SurfaceDirections p_Direction, OSATypes.TemplateSurface p_TemplateSurface, float p_DefaultFriction, int p_DefaultOffense, int p_DefaultDefense)
        {
            SurfaceInformation surfaceInfo = new SurfaceInformation(p_Direction);
            bool isDie = bool.TryParse(p_TemplateSurface.Die, out isDie) ? bool.Parse(p_TemplateSurface.Die) : false;
            bool boolTest;
            float floatTest;
            int offense = isDie ? 0 : (bool.TryParse(p_TemplateSurface.IsDeadly, out boolTest) ? bool.Parse(p_TemplateSurface.IsDeadly) : p_DefaultOffense > 0) ? p_DefaultOffense : 0;
            surfaceInfo.Set(
                bool.TryParse(p_TemplateSurface.IsSolid, out boolTest) ? bool.Parse(p_TemplateSurface.IsSolid) : true,
                float.TryParse(p_TemplateSurface.Friction, out floatTest) ? float.Parse(p_TemplateSurface.Friction) : p_DefaultFriction,
                float.TryParse(p_TemplateSurface.Velocity, out floatTest) ? float.Parse(p_TemplateSurface.Velocity) : 0f,
                isDie, offense,
                p_DefaultDefense,
                float.TryParse(p_TemplateSurface.TangentSpeed, out floatTest) ? float.Parse(p_TemplateSurface.TangentSpeed) : 0f
                );
            return surfaceInfo;
        }
    }
}
