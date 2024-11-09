namespace OSA
{
    public class CollectiblePower
    {
        public string OwnerPath;
        public bool CanFly = false;
        public bool CanAccelerateFromTile = true;
        public int Offense = 0;
        public int Defense = 0;
        public float MaxHorizontalSpeedFactor = 1f;
        public float Gravity = 0.5f;
        public bool RunRight = false;

        public string ObjectTypePath;
        public void Attach(Collectible p_Collectible)
        {
            this.ObjectTypePath = p_Collectible.ObjectTypePath;
        }

        public CollectiblePower()
        {
        }

        public CollectiblePower(string p_OwnerPath)
        {
            this.OwnerPath = p_OwnerPath;
        }
    }

    public class XmlCollectiblePowerReader
    {
        public XmlCollectiblePowerReader() { }

        public CollectiblePower CreateInstance(OSATypes.TemplatePower[] p_Power)
        {
            CollectiblePower power = null;
            if (p_Power != null && p_Power.Length > 0)
            {
                string ownerPath = p_Power[0].OwnerPath != null ? p_Power[0].OwnerPath : string.Empty;
                power = new CollectiblePower(ownerPath);
                power.CanFly = bool.TryParse(p_Power[0].CanFly, out power.CanFly) ? bool.Parse(p_Power[0].CanFly) : false;
                power.CanAccelerateFromTile = bool.TryParse(p_Power[0].CanAccelerateFromTile, out power.CanAccelerateFromTile) ? bool.Parse(p_Power[0].CanAccelerateFromTile) : true;
                power.Offense = int.TryParse(p_Power[0].Offense, out power.Offense) ? int.Parse(p_Power[0].Offense) : 0;
                power.Defense = int.TryParse(p_Power[0].Defense, out power.Defense) ? int.Parse(p_Power[0].Defense) : 0;
                power.MaxHorizontalSpeedFactor = float.TryParse(p_Power[0].MaxHorizontalSpeedFactor, out power.MaxHorizontalSpeedFactor) ? float.Parse(p_Power[0].MaxHorizontalSpeedFactor) : 1f;
                power.Gravity = float.TryParse(p_Power[0].Gravity, out power.Gravity) ? float.Parse(p_Power[0].Gravity) : 0.5f;
                power.RunRight = bool.TryParse(p_Power[0].RunRight, out power.RunRight) ? bool.Parse(p_Power[0].RunRight) : false;
            }
            return power;
        }
    }
}
