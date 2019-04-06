using UnityEngine;
using UnityEngine.UI;

namespace Maplewing.FantansyMaze
{
    public class Selector : MonoBehaviour
    {
        [SerializeField]
        private float _itemHeight = -100;

        [SerializeField]
        private float _itemPadding = -20;

        [SerializeField]
        private GameObject _itemPrefab;

        [SerializeField]
        private RectTransform _contentTransform;

        private GameObject[] _items;

        public int SelectedIndex
        {
            get; private set;
        }

        public void SetItems(string[] items)
        {
            Clear();

            _items = new GameObject[items.Length];
            for(int i = 0; i < items.Length; ++i)
            {
                _items[i] = Instantiate(_itemPrefab, _contentTransform);
                _items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    50, _itemHeight * i + _itemPadding * (i + 1));
                _items[i].GetComponent<Text>().text = items[i];
            }

            SelectedIndex = 0;
            _items[SelectedIndex].GetComponent<Text>().color = Color.red;
        }

        public bool SelectItem(int index)
        {
            if (index < 0 || index >= _items.Length) return false;

            SelectedIndex = index;
            for (int i = 0; i < _items.Length; ++i)
            {
                _items[i].GetComponent<Text>().color = (i == SelectedIndex) ? Color.red : Color.white;
            }

            _contentTransform.anchoredPosition = new Vector2(
                _contentTransform.anchoredPosition.x,
                _itemHeight * SelectedIndex + _itemPadding * SelectedIndex);

            return true;
        }

        public void Clear()
        {
            if (_items == null) return;

            foreach(var item in _items)
            {
                Destroy(item);
            }

            _items = null;
        }
    }
}