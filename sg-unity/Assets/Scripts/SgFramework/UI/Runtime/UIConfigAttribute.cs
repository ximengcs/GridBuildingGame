using System;

namespace SgFramework.UI
{
    public class UIConfigAttribute : Attribute
    {
        public int Layer { get; set; }
        public string ResourceKey { get; set; }

        public UIConfigAttribute(int layer, string resourceKey)
        {
            Layer = layer;
            ResourceKey = resourceKey;
        }
    }
}