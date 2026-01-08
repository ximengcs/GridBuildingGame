using MH.GameScene.Core.Entites;
using UnityEngine;

namespace MH.GameScene.Runtime.Characters
{
    public interface ICharacter : IEntity
    {
        int NpcId { get; }

        Vector2Int Index { get; }
    }
}
