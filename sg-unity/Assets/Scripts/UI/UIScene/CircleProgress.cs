
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIScenes
{
    public class CircleProgress : UISceneItem
    {
        [SerializeField]
        private Image _proImg;

        public float Pro
        {
            get => _proImg.fillAmount;
            set => _proImg.fillAmount = value;
        }
    }
}
