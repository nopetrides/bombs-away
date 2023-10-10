﻿using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Core;
using HelloMurder.Messages;
using HelloMurder.Services;
using Murder;
using Murder.Components;
using Murder.Utilities;

namespace HelloMurder.Systems.Player
{
    [Filter(typeof(PlayerComponent))]
    [Messager(typeof(AgentInputMessage))]
    public class SpawnBombSystem : IMessagerSystem
    {
        private float _timeSinceLastBomb = -1f;
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (message == null) return;

            var input = (AgentInputMessage)message;
            if (input.Button != InputButtons.Attack) return;

            // Reload time
            if (Game.Now < _timeSinceLastBomb + 1f) return;

            // Spawn a bomb
            Guid prefab = LibraryServices.GetLibrary().BombPrefab;

            Entity e = AssetServices.Create(world, prefab);
            e.SetTransform(entity.GetGlobalTransform());
            e.SetMoveToPerfect();
            // todo fix

            SpriteComponent sprite = e.GetSprite();
            sprite = sprite.Play(false, "falling", "damage", "miss");
            
            e.SetSprite(sprite);

            _timeSinceLastBomb = Game.Now;
        }
    }
}
