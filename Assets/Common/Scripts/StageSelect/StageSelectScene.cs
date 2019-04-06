using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maplewing.FantansyMaze
{
    public class StageSelectScene : MonoBehaviour
    {
        [SerializeField]
        private Selector _stageSelector;

        [SerializeField]
        private InformationPanel _informationPanel;

        private MapInformation[] _mapInformations;

        void Start()
        {
            List<MapInformation> mapInformations = new List<MapInformation>();
            string mapDirectory = Path.Combine(Application.streamingAssetsPath, "maps");
            foreach(var directory in Directory.GetDirectories(mapDirectory))
            {
                string informationFileRootPath = Path.Combine(mapDirectory, directory);
                string informationFilePath = Path.Combine(informationFileRootPath, "information.json");
                if (File.Exists(informationFilePath))
                {
                    var information = JsonUtility.FromJson<MapInformation>(File.ReadAllText(informationFilePath));
                    information.MapFile = Path.Combine(informationFileRootPath, information.MapFile);
                    mapInformations.Add(information);
                }
            }

            _mapInformations = mapInformations.ToArray();
            _stageSelector.SetItems(_mapInformations.Select(info => info.MapName).ToArray());
            _UpdateInformationPanel();
        }
        
        void Update()
        {
            if(_ChangeStage())
            {
                _UpdateInformationPanel();
            }

            _SelectStage();
        }

        private bool _ChangeStage()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _stageSelector.SelectItem(_stageSelector.SelectedIndex - 1);
                return true;
            }
            
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                _stageSelector.SelectItem(_stageSelector.SelectedIndex + 1);
                return true;
            }

            return false;
        }

        private void _UpdateInformationPanel()
        {
            var mapInformation = _mapInformations[_stageSelector.SelectedIndex];
            string stageName = mapInformation.MapName;
            _informationPanel.SetStageNameText(stageName);
            
            if (PlayerPrefs.HasKey(mapInformation.BestTimeKey))
            {
                int bestTime = PlayerPrefs.GetInt(mapInformation.BestTimeKey);
                _informationPanel.SetBestScoreText(string.Format("{0}:{1:00}", bestTime / 60, bestTime % 60));
            }
            else
            {
                _informationPanel.SetBestScoreText("No Record.");
            }
        }

        private void _SelectStage()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StageSelectToGamePlayArgumentTunnel.CurrentMap = _mapInformations[_stageSelector.SelectedIndex];
                SceneManager.LoadScene("GamePlay");
            }
        }
    }
}