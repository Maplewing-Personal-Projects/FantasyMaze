using System;
using System.Linq;

namespace Maplewing.MazeSystem
{
    public class MazeSystem
    {
        public enum MapObject
        {
            Empty,
            Wall,
            Start,
            Exit
        };

        public event Action<int> OnUnlockItem;

        public MapObject[,] Map
        {
            get; private set;
        }

        public PlayerData PlayerData
        {
            get; private set;
        }

        public float CurrentAccuracyTime
        {
            get; private set;
        }

        public int CurrentTime
        {
            get { return (int)Math.Floor(CurrentAccuracyTime); }
        }

        public bool IsExit
        {
            get; private set;
        }
        
        private TransportItem[] _transportItems;
        private LockItem[] _lockItems;

        public MazeSystem(MapObject[,] map, 
            TransportItem[] transportItems,
            LockItem[] lockItems)
        {
            Map = map;
            _transportItems = transportItems;
            _lockItems = lockItems;

            for (int i = 0; i < Map.GetLength(0) && PlayerData == null; ++i)
            {
                for(int j = 0; j < Map.GetLength(1) && PlayerData == null; ++j)
                {
                    if(Map[i, j] == MapObject.Start)
                    {
                        PlayerData = new PlayerData
                        {
                            Position = new Coordinate
                            {
                                X = j,
                                Y = i
                            },
                            FaceToDirection = Direction.Down
                        };

                        break;
                    }
                }
            }
        }

        public void UpdateTime(float deltaTime)
        {
            CurrentAccuracyTime += deltaTime;
        }

        public bool MovePlayer(Direction direction)
        {
            PlayerData.FaceToDirection = direction;

            Coordinate finalPosition = new Coordinate
            {
                X = ((direction == Direction.Left) ? -1 : ((direction == Direction.Right) ? 1 : 0)) + PlayerData.Position.X,
                Y = ((direction == Direction.Up) ? -1 : ((direction == Direction.Down) ? 1 : 0)) + PlayerData.Position.Y
            };

            if (finalPosition.X < 0 || finalPosition.X >= Map.GetLength(1) ||
                finalPosition.Y < 0 || finalPosition.Y >= Map.GetLength(0) ||
                Map[finalPosition.Y, finalPosition.X] == MapObject.Wall) return false;

            if(_lockItems != null)
            {
                var lockItem = _lockItems.FirstOrDefault(item =>
                    item.Position.X == finalPosition.X && item.Position.Y == finalPosition.Y &&
                    !item.IsUnlocked);

                if (_lockItems.Any(item =>
                    item.Position.X == finalPosition.X && item.Position.Y == finalPosition.Y &&
                    !item.IsUnlocked)) return false;

                for(int itemIndex = 0; itemIndex < _lockItems.Length; ++itemIndex)
                {
                    var item = _lockItems[itemIndex];
                    if (item.IsUnlocked ||
                        item.UnlockPosition.X != finalPosition.X ||
                        item.UnlockPosition.Y != finalPosition.Y) continue;

                    item.IsUnlocked = true;
                    if (OnUnlockItem != null)
                    {
                        OnUnlockItem(itemIndex);
                    }
                }
            }

            if(Map[finalPosition.Y, finalPosition.X] == MapObject.Exit) IsExit = true;

            if (_transportItems != null)
            {
                var touchedTransportItem = _transportItems.FirstOrDefault(item =>
                    item.Position.X == finalPosition.X && item.Position.Y == finalPosition.Y);
                if (touchedTransportItem != null)
                {
                    PlayerData.Position = touchedTransportItem.TransportPosition;
                    return true;
                }
            }

            PlayerData.Position = finalPosition;
            return true;
        }
    }
}