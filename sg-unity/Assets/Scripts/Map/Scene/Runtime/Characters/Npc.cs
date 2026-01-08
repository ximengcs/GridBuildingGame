using System;
using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Core.PathFinding;
using MH.GameScene.Runtime.Characters;

namespace MH.GameScene.Runtime.Entities
{
    public class Npc : Entity, ICharacter
    {
        private int _npcId;
        private PathFindingCom _pathFinder;
        private IMapScene _scene;
        private DestinationItem _destItem;
        private PathViewCom _pathView;
        private Vector2Int _index;
        private Action _positionChangeEvent;

        public Vector2Int Index
        {
            get => _index;
            set
            {
                _index = value;
                _positionChangeEvent?.Invoke();
            }
        }

        public int NpcId => _npcId;

        public event Action PositionChangeEvent
        {
            add => _positionChangeEvent += value;
            remove => _positionChangeEvent -= value;
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            CharacterGenParam param = (CharacterGenParam)data;
            _npcId = param.NpcId;
            _index = param.Index;
        }

        protected override void OnStart()
        {
            base.OnStart();
            AddCom<NpcItemView>();
            _scene = World.FindEntity<IMapScene>();
            _scene.RegisterInitFinish(SceneFinishHandler);
        }

        private void SceneFinishHandler()
        {
            _pathView = AddCom<PathViewCom>();
            _pathFinder = _scene.GetCom<PathFindingCom>();
            _destItem = _scene.GetFirstItem<DestinationItem>();
            _destItem.GridChangeEvent += GridChangeHandler;
            GeneratePath();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _destItem.GridChangeEvent -= GridChangeHandler;
        }

        private void GridChangeHandler()
        {
            GeneratePath();
        }

        private void GeneratePath()
        {
            IGridEntity grid = _scene.GetGrid(_index);
            IPath<IGridEntity> path = _pathFinder.Find(grid, _destItem.MainGrid);
            _pathView.SetPath(path);
        }
    }
}
