using System;

namespace Maplewing.FantansyMaze
{
    using MazeSystem;

    [Serializable]
    public class MapInformation
    {
        public string Id;
        public string MapName;
        public string MapFile;
        public TransportItem[] TransportItems;
        public LockItem[] LockItems;

        public string BestTimeKey
        {
            get { return "best_time_" + Id; }
        }
    }
}