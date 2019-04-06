using System.Collections;
using UnityEngine;

namespace Maplewing.FantansyMaze
{
    using MazeSystem;

    public class PlayerView : MonoBehaviour
    {
        private const float PLAYER_Z_OFFSET = MapGenerator.Z_SCALE * 0.1f;
        private const float MOVE_TIME = 0.2f;

        public Vector2 PlayerPosition
        {
            get { return _playerObject.transform.localPosition; }
        }

        [SerializeField]
        private GameObject _playerPrefab;

        private GameObject _playerObject;

        public void GeneratePlayer(Coordinate position, Direction faceToDirection)
        {
            if (_playerObject != null) Destroy(_playerPrefab);

            _playerObject = Instantiate(_playerPrefab, transform);
            _playerObject.transform.localPosition = new Vector3(
                position.X * MapGenerator.MAP_OBJECT_SIZE,
                -position.Y * MapGenerator.MAP_OBJECT_SIZE,
                -position.Y * MapGenerator.Z_SCALE - PLAYER_Z_OFFSET);

            var animator = _playerObject.GetComponent<Animator>();
            animator.Play(faceToDirection.ToString());
        }

        public IEnumerator Move(Direction direction)
        {
            var animator = _playerObject.GetComponent<Animator>();
            animator.Play(direction.ToString());

            Vector2 directionMove = new Vector2(
                (direction == Direction.Left) ? -1 : ((direction == Direction.Right) ? 1 : 0),
                (direction == Direction.Up) ? 1 : ((direction == Direction.Down) ? -1 : 0)) * MapGenerator.MAP_OBJECT_SIZE;

            Vector2 directionMovePerOneSecond = directionMove / MOVE_TIME;
            Vector3 finalPosition = new Vector3(
                _playerObject.transform.localPosition.x + directionMove.x,
                _playerObject.transform.localPosition.y + directionMove.y,
                (_playerObject.transform.localPosition.y + directionMove.y) / MapGenerator.MAP_OBJECT_SIZE
                * MapGenerator.Z_SCALE - PLAYER_Z_OFFSET);

            float moveZ = Mathf.Min(_playerObject.transform.localPosition.y,
                _playerObject.transform.localPosition.y + directionMove.y) / MapGenerator.MAP_OBJECT_SIZE
                * MapGenerator.Z_SCALE - PLAYER_Z_OFFSET;
            
            float currentTime = 0;
            Vector3 originPosition = _playerObject.transform.localPosition;
            while (currentTime <= MOVE_TIME)
            {
                float ratio = currentTime / MOVE_TIME;
                _playerObject.transform.localPosition = new Vector3(
                    Mathf.Lerp(originPosition.x, finalPosition.x, ratio),
                    Mathf.Lerp(originPosition.y, finalPosition.y, ratio),
                    moveZ);
                yield return null;
                currentTime += Time.deltaTime;
            }
            _playerObject.transform.localPosition = finalPosition;
        }

        public void Set(Coordinate position, Direction faceToDirection)
        {
            _playerObject.transform.localPosition = new Vector3(
                position.X * MapGenerator.MAP_OBJECT_SIZE,
                -position.Y * MapGenerator.MAP_OBJECT_SIZE,
                -position.Y * MapGenerator.Z_SCALE - PLAYER_Z_OFFSET);

            var animator = _playerObject.GetComponent<Animator>();
            var directionState = faceToDirection.ToString();
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(directionState))
            {
                animator.Play(directionState);
            }
        }
    }
}