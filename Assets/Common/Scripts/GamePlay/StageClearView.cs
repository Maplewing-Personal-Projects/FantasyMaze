using UnityEngine;
using UnityEngine.UI;

namespace Maplewing.FantansyMaze
{

    public class StageClearView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _canvasGameObject;

        [SerializeField]
        private GameObject _newRecord;

        [SerializeField]
        private Text _timeText;

        void Start()
        {
            _canvasGameObject.SetActive(false);
            _newRecord.SetActive(false);
        }
        
        public void SetNewRecord(bool isActive)
        {
            _newRecord.SetActive(isActive);
        }

        public void SetTime(string time)
        {
            _timeText.text = "Cost Time: " + time;
        }
        
        public void ShowStageClear()
        {
            _canvasGameObject.SetActive(true);
        }
    }
}