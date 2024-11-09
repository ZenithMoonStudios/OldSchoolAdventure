namespace OSA
{
    public class DoorTarget
    {
        public string RoomPath { get; set; }
        public string ObjectName { get; set; }

        public DoorTarget()
        {
        }

        public bool IsEmpty { get { return string.IsNullOrEmpty(this.RoomPath); } }

        public void Set(Door p_Door)
        {
            if (p_Door != null)
            {
                this.Set(p_Door.Room.ObjectTypePath, p_Door.Name);
            }
            else
            {
                this.Clear();
            }
        }

        public void Set(DoorTarget p_DoorTarget)
        {
            if (p_DoorTarget != null)
            {
                this.Set(p_DoorTarget.RoomPath, p_DoorTarget.ObjectName);
            }
            else
            {
                this.Clear();
            }
        }

        public void Set(string p_RoomPath, string p_ObjectName)
        {
            this.RoomPath = p_RoomPath;
            this.ObjectName = p_ObjectName;
        }

        public void Clear()
        {
            this.Set(null, null);
        }
    }
}
