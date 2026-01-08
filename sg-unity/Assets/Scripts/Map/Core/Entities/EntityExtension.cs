
namespace MH.GameScene.Core.Entites
{
    public static class EntityExtension
    {
        public static T AddEntity<T>(this IEntity entity, object data = null) where T : IEntity, new()
        {
            return entity.World.AddEntity<T>(entity, data);
        }

        public static void RemoveEntity(this IEntity entity)
        {
            entity.World.RemoveEntity(entity);
        }

        public static T GetCom<T>(this ComponentBase com) where T : IComponent
        {
            return com.Entity.GetCom<T>();
        }
    }
}
