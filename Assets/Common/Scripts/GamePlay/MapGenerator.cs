using System;
using System.Linq;
using UnityEngine;

namespace Maplewing.FantansyMaze
{
    using MazeSystem;

    public class MapGenerator : MonoBehaviour
    {
        [Serializable]
        public struct MapObjectPrefab
        {
            public MazeSystem.MapObject ObjectType;
            public GameObject Prefab;
        }

        public const float MAP_OBJECT_SIZE = 50f;
        public const float Z_SCALE = 0.01f;

        private static readonly Color BROWN = new Color32(150, 75, 0, 255);

        [SerializeField]
        private MapObjectPrefab[] _objectPrefabs;

        private MazeSystem.MapObject[,] _map;
        private GameObject[,] _mapObjects;

        public void GenerateMap(
            MazeSystem.MapObject[,] map, 
            TransportItem[] transportItems,
            LockItem[] lockItems)
        {
            _map = map;
            _mapObjects = new GameObject[map.GetLength(0), map.GetLength(1)];

            for(int i = 0; i < map.GetLength(0); ++i)
            {
                for(int j = 0; j < map.GetLength(1); ++j)
                {
                    _mapObjects[i, j] = Instantiate(_objectPrefabs.First(prefab => prefab.ObjectType == map[i, j]).Prefab, transform);
                    _mapObjects[i, j].transform.localPosition = new Vector3(
                                j * MAP_OBJECT_SIZE, i * -MAP_OBJECT_SIZE, -i * Z_SCALE);
                }
            }

            if (transportItems != null)
            {
                foreach (var item in transportItems)
                {
                    _mapObjects[item.Position.Y, item.Position.X]
                        .GetComponent<SpriteRenderer>().color = Color.yellow;
                }
            }

            if(lockItems != null)
            {
                UpdateLockItems(lockItems);
            }
        }

        public bool UpdateLockItems(LockItem[] lockItems)
        {
            if (lockItems == null) return false;

            var unlockItems = lockItems.Where(item => item.IsUnlocked);
            foreach(var item in unlockItems)
            {
                _mapObjects[item.Position.Y, item.Position.X]
                    .GetComponent<SpriteRenderer>().color = _objectPrefabs.First(
                        prefab => prefab.ObjectType == _map[item.Position.Y, item.Position.X])
                        .Prefab.GetComponent<SpriteRenderer>().color;
                _mapObjects[item.UnlockPosition.Y, item.UnlockPosition.X]
                    .GetComponent<SpriteRenderer>().color = _objectPrefabs.First(
                        prefab => prefab.ObjectType == _map[item.UnlockPosition.Y, item.UnlockPosition.X])
                        .Prefab.GetComponent<SpriteRenderer>().color;
            }

            var keepLockItems = lockItems.Where(item => !item.IsUnlocked);
            foreach (var item in keepLockItems)
            {
                _mapObjects[item.Position.Y, item.Position.X]
                    .GetComponent<SpriteRenderer>().color = Color.grey;
                _mapObjects[item.UnlockPosition.Y, item.UnlockPosition.X]
                    .GetComponent<SpriteRenderer>().color = BROWN;
            }
            return true;
        }

        private void OnDestroy()
        {
            if (_mapObjects == null) return;

            foreach(var mapObject in _mapObjects)
            {
                Destroy(mapObject);
            }
        }
    }
}