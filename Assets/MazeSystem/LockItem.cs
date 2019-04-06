using System;

namespace Maplewing.MazeSystem
{
    [Serializable]
    public class LockItem
    {
        public Coordinate Position;
        public Coordinate UnlockPosition;

        [NonSerialized]
        public bool IsUnlocked = false;
    }
}