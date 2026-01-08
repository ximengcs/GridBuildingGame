using Common;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UISettingsPageLanguage : MonoBehaviour
    {
        [SerializeField] private Toggle toggleTemplate;

        private void Awake()
        {
            toggleTemplate.gameObject.SetActive(false);
            foreach (var langType in Table.LangTypeTable.DataList)
            {
                var lan = langType.id;
                var toggle = Instantiate(toggleTemplate, transform);
                toggle.GetComponentInChildren<TextMeshProUGUI>().text = langType.lang_type_name;
                toggle.SetIsOnWithoutNotify(DataController.GetLanguageSetting() == lan);
                toggle.gameObject.SetActive(true);
                toggle.onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    DataController.SetLanguageSetting(lan);
                });
            }
        }
    }
}