namespace OSA
{
    /// <summary>
    /// Door state helper class.
    /// </summary>
    public static class DoorStateHelper
    {
        public const string c_Locked = "Locked";
        public const string c_Closed = "Closed";
        public const string c_Open = "Open";

        public static string ToString(DoorState p_State)
        {
            string result = c_Closed;
            switch (p_State)
            {
                case DoorState.Locked: result = c_Locked; break;
                case DoorState.Closed: result = c_Closed; break;
                case DoorState.Open: result = c_Open; break;
            }
            return result;
        }
    }
}
