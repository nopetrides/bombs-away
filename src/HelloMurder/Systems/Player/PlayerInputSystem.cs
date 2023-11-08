using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder;
using Murder.Helpers;
using HelloMurder.Components;
using HelloMurder.Messages;
using HelloMurder.Core;
using HelloMurder.Core.Input;
using Bang.Interactions;
using System.Numerics;
using Murder.Utilities;
using HelloMurder.Services;
using Murder.Core.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using HelloMurder.Core.Sounds;

namespace HelloMurder.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(PlayerComponent), typeof(AgentComponent))]
    public class PlayerInputSystem : IUpdateSystem, IFixedUpdateSystem
    {
        private Vector2 _cachedInputAxis = Vector2.Zero;
        
        private bool _previousCachedAttack = false;
        private bool _cachedAttack = false;

        public void FixedUpdate(Context context)
        {
            foreach (Entity entity in context.Entities)
            {
                PlayerComponent player = entity.GetComponent<PlayerComponent>();
                var library = LibraryServices.GetLibrary();

                bool moved = _cachedInputAxis.HasValue();

                if (moved)
                {
                    Direction direction = DirectionHelper.FromVector(_cachedInputAxis);

                    var finalPosition = entity.GetGlobalTransform().Vector2;
                    finalPosition += _cachedInputAxis;
                    finalPosition = ClampBounds(library.Bounds, finalPosition);
                    entity.SetGlobalPosition(finalPosition);

                    entity.SetAgentImpulse(_cachedInputAxis, direction);


                    HelloMurderSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Roll, _cachedInputAxis.X);
                    HelloMurderSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Thrust, -_cachedInputAxis.Y);
                }
                else
                {
                    entity.SetFacing(Direction.Up);

                    HelloMurderSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Roll, 0f);
                    HelloMurderSoundPlayer.Instance.SetGlobalParameter(LibraryServices.GetLibrary().Thrust, 0f);
                }
                
                if (!_previousCachedAttack && _cachedAttack)
                {
                    _previousCachedAttack = true;
                    entity.SendMessage(new AgentInputMessage(InputButtons.Attack));
                }
                else if (_previousCachedAttack && !_cachedAttack)
                {
                    _previousCachedAttack = false;
                    entity.SendMessage(new AgentReleaseInputMessage(InputButtons.Attack));
                }
            }
        }

        public void Update(Context context)
        {
            _cachedInputAxis = Game.Input.GetAxis(InputAxis.Movement).Value;

            _cachedAttack = Game.Input.Down(InputButtons.Attack);

            if (Game.Input.PressedAndConsume(InputButtons.Pause) 
                && !context.World.IsPaused 
                && context.World.GetEntitiesWith(typeof(DoNotPauseComponent)).Count() == 0) 
            {
                LibraryServices.GetPauseMenuPrefab().Create(context.World);

                HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().UiNavigate, Murder.Core.Sounds.SoundProperties.None);
            }
        }

        private Vector2 ClampBounds(IntRectangle bounds, Vector2 position)
        {
            if (position.X < bounds.X) position.X = bounds.X;
            if (position.Y < bounds.Y) position.Y = bounds.Y;

            var maxWidth = bounds.Width+bounds.X;
            var maxHeight = bounds.Height+bounds.Y;

            if (position.X > maxWidth) position.X = maxWidth;
            if (position.Y > maxHeight) position.Y = maxHeight;

            return position;
        }
    }
}
