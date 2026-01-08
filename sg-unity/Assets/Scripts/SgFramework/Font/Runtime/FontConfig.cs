using TMPro;
using UnityEngine;

namespace SgFramework.Font
{
    [CreateAssetMenu]
    public class FontConfig : ScriptableObject
    {
        public TMP_FontAsset fontAsset;
        public UnityEngine.Font font;
    }
}