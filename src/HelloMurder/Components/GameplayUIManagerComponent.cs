﻿using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace HelloMurder.Components
{
    [Unique]
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct GameplayUIManagerComponent : IComponent
    {
        private readonly int _score;
        public int CurrentScore => _score;

        public GameplayUIManagerComponent(int score) 
        {
            _score = score;
        }

        public GameplayUIManagerComponent AddScore(int scoreToAdd)
        {
            return new GameplayUIManagerComponent(_score + scoreToAdd);
        }

    }
}
