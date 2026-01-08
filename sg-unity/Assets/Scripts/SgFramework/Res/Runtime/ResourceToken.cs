using UnityEngine;

namespace SgFramework.Res
{
    public class ResourceToken : MonoBehaviour
    {
        /// <summary>
        /// 资源key
        /// </summary>
        public string ResourceKey { get; set; }
    
        /// <summary>
        /// 是否为激活状态
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// 是否可回收，不可回收的直接销毁
        /// </summary>
        public virtual bool CanReuse { get; set; } = true;

        public virtual void OnCreate()
        {
        
        }

        public virtual void OnGet()
        {
        
        }

        public virtual void OnUpdate(float deltaTime)
        {
        
        }

        public virtual void OnRelease()
        {
        
        }
    }
}