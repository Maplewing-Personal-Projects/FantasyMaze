using UnityEngine;
using UnityEngine.UI;

namespace Maplewing.FantansyMaze
{
    public class InformationPanel : MonoBehaviour
    {
        [SerializeField]
        private Text _stageNameText;

        [SerializeField]
        private Text _bestScoreText;

        public void SetStageNameText(string text)
        {
            _stageNameText.text = "Stage: " + text;
        }

        public void SetBestScoreText(string text)
        {
            _bestScoreText.text = "Best Time: " + text;
        }
    }
}