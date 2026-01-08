#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using MH.GameScene.Configs;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorGridEditPanelItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private RectTransform itemsNode;

        [SerializeField]
        private GameObject itemPrefab;

        public MapEditorUI EditorUI { get; set; }

        private void Awake()
        {
            itemPrefab.SetActive(false);
        }

        public void SetTile(string tileName)
        {
            title.text = tileName;
        }

        public MapEditorGridItem AddItem(int id, string layer, string res, MapEditorSelectItem wrapper)
        {
            GameObject inst = GameObject.Instantiate(itemPrefab);
            inst.SetActive(true);
            inst.transform.SetParent(itemsNode);
            MapEditorGridItem item = inst.GetComponent<MapEditorGridItem>();
            item.EditorUI = EditorUI;
            item.Init(id, layer, res, wrapper);
            return item;
        }

        public MapEditorGridItem AddItem(int id, string layer, ItemConfig itemTable)
        {
            GameObject inst = GameObject.Instantiate(itemPrefab);
            inst.SetActive(true);
            inst.transform.SetParent(itemsNode);
            MapEditorGridItem item = inst.GetComponent<MapEditorGridItem>();
            item.EditorUI = EditorUI;
            item.Init(id, layer, itemTable.Res, new MapEditorSelectItem(SelectItemType.Common, itemTable));
            return item;
        }
    }
}
#endif