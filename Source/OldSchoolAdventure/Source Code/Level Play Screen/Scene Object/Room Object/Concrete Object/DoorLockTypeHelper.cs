namespace OSA
{
    /// <summary>
    /// Door lock type helper class.
    /// </summary>
    public static class DoorLockTypeHelper
    {
        public const string c_None = "None";
        public const string c_Key = "Key";
        public const string c_Switch = "Switch";

        public static DoorLockType FromString(string p_String)
        {
            DoorLockType result = DoorLockType.None;
            switch (p_String)
            {
                case c_Key: result = DoorLockType.Key; break;
                case c_Switch: result = DoorLockType.Switch; break;
            }
            return result;
        }
    }
}
