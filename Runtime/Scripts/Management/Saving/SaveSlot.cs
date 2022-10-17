using System;

namespace H2DT.Management.Saving
{
    [Serializable]
    public class SaveSlot
    {
        public string id;
        public string name;
        public string createdAt;
        public string createdAtTime;

        public SaveSlot(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.createdAt = System.DateTime.Now.ToLongDateString();
            this.createdAtTime = System.DateTime.Now.ToLongTimeString();
        }
    }
}
