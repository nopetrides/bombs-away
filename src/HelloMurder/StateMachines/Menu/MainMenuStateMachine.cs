﻿using Bang.Entities;
using Bang.StateMachines;
using HelloMurder.Core;
using HelloMurder.Core.Sounds;
using HelloMurder.Services;
using Murder;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services;
using Newtonsoft.Json;
using System.Numerics;

namespace HelloMurder.StateMachines
{
    internal class MainMenuStateMachine : StateMachine
    {
        [JsonProperty, GameAssetId(typeof(WorldAsset))]
        private readonly Guid _newGameWorld = Guid.Empty;

        private MenuInfo _menuInfo = new();

        private MenuInfo GetMainMenuOptions() =>
            new MenuInfo(new MenuOption[] {new("Play"), new("Options"), new("Exit") });

        private MenuInfo GetOptionOptions() =>
            new MenuInfo(new MenuOption[] {
                new(Game.Preferences.SoundVolume == 1 ? "Sounds on" : "Sounds off"),
                new("Clear Data"),
                new("Back") });

        public MainMenuStateMachine()
        {
            State(Main);
        }

        protected override void OnStart()
        {
            Entity.SetCustomDraw(DrawMainMenu);

            _menuInfo.Select(MurderSaveServices.CanLoadSave() ? 0 : 1);
        }

        private IEnumerator<Wait> Main()
        {
            _menuInfo = GetMainMenuOptions();
            _menuInfo.Select(_menuInfo.NextAvailableOption(-1, 1));

            while (true)
            {
                int previousInput = _menuInfo.Selection;

                if (Game.Input.VerticalMenu(ref _menuInfo))
                {
                    HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().UiSelect, Murder.Core.Sounds.SoundProperties.None);

                    switch (_menuInfo.Selection)
                    {
                        case 0: //  Play
                            Game.Instance.QueueWorldTransition(_newGameWorld);
                            break;

                        case 1: // Options
                            yield return GoTo(Options);
                            break;

                        case 2: //  Exit
                            Game.Instance.QueueExitGame();
                            break;

                        default:
                            break;
                    }
                }

                if (previousInput != _menuInfo.Selection)
                {
                    HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().UiNavigate, Murder.Core.Sounds.SoundProperties.None);
                }

                yield return Wait.NextFrame;
            }
        }
        
        private IEnumerator<Wait> Options()
        {
            _menuInfo = GetOptionOptions();
            _menuInfo.Select(_menuInfo.NextAvailableOption(-1, 1));

            while (true)
            {
                int previousInput = _menuInfo.Selection;

                if (Game.Input.VerticalMenu(ref _menuInfo))
                {
                    HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().UiSelect, Murder.Core.Sounds.SoundProperties.None);
                    switch (_menuInfo.Selection)
                    {
                        case 0: // Tweak sound
                            float volume = Game.Preferences.ToggleSoundVolumeAndSave();

                            _menuInfo.Options[0] = volume == 1 ? new("Sounds on") : new("Sounds off");
                            break;

                        case 1: // Delete save data
                            Game.Data.DeleteAllSaves();
                            break;

                        case 2: // Go back
                            yield return GoTo(Main);
                            break;
                            
                        default:
                            break;
                    }
                }

                if (previousInput != _menuInfo.Selection)
                {
                    HelloMurderSoundPlayer.Instance.PlayEvent(LibraryServices.GetLibrary().UiNavigate, Murder.Core.Sounds.SoundProperties.None);
                }

                yield return Wait.NextFrame;
            }
        }

        private void DrawMainMenu(RenderContext render)
        {
            // BG
            var skin = LibraryServices.GetLibrary().SplashScreen;

            RenderServices.DrawSprite(render.UiBatch, skin,
                new Vector2(render.Camera.Size.X / 2f, render.Camera.Size.Y / 2f), new DrawInfo(0.8f)
                {
                    Origin = new Vector2(.5f, .5f)
                });

            // Menu options
            Point menuOrigin = (render.Camera.Size / 2f) + new Point(25, 73);

            _ = RenderServices.DrawVerticalMenu(
                render.UiBatch, 
                menuOrigin, 
                new DrawMenuStyle() 
                { 
                    Color = Palette.Colors[7], 
                    Shadow = Palette.Colors[1],
                    SelectedColor = Palette.Colors[9],
                    Origin = new(0.5f, 0.5f),
                    ExtraVerticalSpace = 19,
                },
                _menuInfo);
        }
    }
}
