using System;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Characters
{
    public class CharacterModule : Entity
    {
        private Dictionary<Type, List<ICharacter>> _characters;

        public IReadOnlyCollection<ICharacter> Characters
        {
            get
            {
                List<ICharacter> result = new List<ICharacter>();
                foreach (var entry in _characters)
                {
                    result.AddRange(entry.Value);
                }
                return result;
            }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _characters = new Dictionary<Type, List<ICharacter>>();
        }

        public T AddCharacter<T>(CharacterGenParam param) where T : ICharacter, new()
        {
            T inst = this.AddEntity<T>(param);
            if (!_characters.TryGetValue(typeof(T), out List<ICharacter> list))
            {
                list = new List<ICharacter>();
                _characters.Add(typeof(T), list);
            }
            list.Add(inst);
            inst.Start();
            return inst;
        }
    }
}
