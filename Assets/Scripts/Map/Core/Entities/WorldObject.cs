
using UnityEngine;

namespace MH.GameScene.Core.Entites
{
    public class WorldObject : MonoBehaviour
    {
        private World _world;

        public void Set(World world)
        {
            _world = world;
        }

        private void Update()
        {
            _world.Update(Time.deltaTime);
        }
    }
}
