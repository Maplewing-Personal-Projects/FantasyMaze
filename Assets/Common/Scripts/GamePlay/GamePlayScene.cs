using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Maplewing.FantansyMaze
{
    using Rayark.Mast;
    using MazeSystem;

    public class GamePlayScene : MonoBehaviour
    {
        private MazeSystem _mazeSystem;

        [SerializeField]
        private MapGenerator _mapGenerator;

        [SerializeField]
        private PlayerView _playerView;

        [SerializeField]
        private CharacterStatusView _characterStatusView;

        [SerializeField]
        private Text _timeText;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private StageClearView _stageClearView;

        private MapInformation _mapInformation;
        private bool _isUnlockItem = false;

        private Executor _executor;

        void Start()
        {
            _mapInformation = StageSelectToGamePlayArgumentTunnel.CurrentMap;
            _mazeSystem = new MazeSystem(
                (new MazeLoader(_mapInformation.MapFile)).LoadMap(),
                _mapInformation.TransportItems,
                _mapInformation.LockItems);
            _mazeSystem.OnUnlockItem += _MemorizeIsUnlockItem;

            _mapGenerator.GenerateMap(_mazeSystem.Map, _mapInformation.TransportItems, _mapInformation.LockItems);
            _playerView.GeneratePlayer(_mazeSystem.PlayerData.Position, _mazeSystem.PlayerData.FaceToDirection);
            _UpdateCamera();
            _executor = new Executor();
            _executor.Add(_Main());
        }
        
        void Update()
        {
            if (_executor == null) return;
            _executor.Resume(Time.deltaTime);

            if (!_mazeSystem.IsExit)
            {
                _UpdateCamera();
                _UpdateTime();
            }
        }

        private void OnDestroy()
        {
            _mazeSystem.OnUnlockItem -= _MemorizeIsUnlockItem;
        }

        private IEnumerator _Main()
        {
            while (!_mazeSystem.IsExit)
            {
                yield return _UpdatePlayer();
            }

            int costTime = _mazeSystem.CurrentTime;
            if (PlayerPrefs.HasKey(_mapInformation.BestTimeKey))
            {
                if (costTime < PlayerPrefs.GetInt(_mapInformation.BestTimeKey)) {
                    PlayerPrefs.SetInt(_mapInformation.BestTimeKey, costTime);
                    _stageClearView.SetNewRecord(true);
                }
            }
            else
            {
                PlayerPrefs.SetInt(_mapInformation.BestTimeKey, costTime);
                _stageClearView.SetNewRecord(true);
            }

            _stageClearView.SetTime(string.Format("{0}:{1:00}", costTime / 60, costTime % 60));
            _stageClearView.ShowStageClear();
            _characterStatusView.PlayHappyAnimation();

            while (!Input.anyKeyDown) yield return null;
            SceneManager.LoadScene("StageSelect");
        }

        private IEnumerator _UpdatePlayer()
        {
            int xOffset = 0, yOffset = 0;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                --yOffset; 
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                ++yOffset;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                --xOffset;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ++xOffset;
            }

            if(xOffset != 0 || yOffset != 0)
            {
                Direction direction = 
                    (xOffset > 0)? Direction.Right :
                        ((xOffset < 0)? Direction.Left :
                            ((yOffset > 0)? Direction.Down : Direction.Up));
                if (_mazeSystem.MovePlayer(direction))
                {
                    yield return _playerView.Move(direction);

                    if (_isUnlockItem)
                    {
                        _mapGenerator.UpdateLockItems(_mapInformation.LockItems);
                        _isUnlockItem = false;
                    }
                }
                else
                {
                    _characterStatusView.PlayHitAnimation();
                    yield return null;
                }

                _playerView.Set(_mazeSystem.PlayerData.Position, _mazeSystem.PlayerData.FaceToDirection);
            }
            else
            {
                yield return null;
            }
        }

        private void _UpdateCamera()
        {
            _camera.transform.localPosition = new Vector3(
                _playerView.PlayerPosition.x,
                _playerView.PlayerPosition.y + MapGenerator.MAP_OBJECT_SIZE,
                _camera.transform.localPosition.z);

            _characterStatusView.SetPositionByCamera(_camera);
        }

        private void _UpdateTime()
        {
            _mazeSystem.UpdateTime(Time.deltaTime);

            int currentTime = _mazeSystem.CurrentTime;
            _timeText.text = (currentTime / 60).ToString() + ":" + string.Format("{0:00}", currentTime % 60);
        }

        private void _MemorizeIsUnlockItem(int index)
        {
            _mapInformation.LockItems[index].IsUnlocked = true;
            _isUnlockItem = true;
        }
    }

}