using ImGuiNET;
using BombsAway.Core.Sounds;
using BombsAway.Core.Sounds.Fmod;
using Murder;
using Murder.Core.Sounds;
using Murder.Editor.ImGuiExtended;
using Murder.Services;
using Murder.Utilities;

namespace BombsAway.Editor.Utilities
{
    internal static class SearchBoxExtensions
    {
        public static SoundEventId? SearchFmodSounds(string id, SoundEventId initial)
        {
            string selected = "Select a sound";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.Path))
            {
                selected = initial.EditorName;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not BombsAwaySoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, SoundEventId>> candidates = new(CollectionHelper.ToStringDictionary(
                player.ListAllEvents().Select(v => v.Id), v => v.EditorName, v => v));

            if (ImGuiHelpers.IconButton('', $"play_sound_{id}"))
            {
                if (Game.Preferences.SoundVolume == 0 || Game.Preferences.MusicVolume == 0)
                {
                    // Make sure all our sounds are on here, so there is no confusion.
                    Game.Preferences.SetSoundVolume(1);
                    Game.Preferences.SetMusicVolume(1);
                }

                SoundServices.StopAll(fadeOut: false);
                _ = SoundServices.Play(initial, SoundProperties.Persist);
            }

            ImGui.SameLine();

            if (ImGuiHelpers.IconButton('\uf04c', $"stop_sound_{id}"))
            {
                SoundServices.StopAll(fadeOut: false);
            }

            ImGui.SameLine();

            if (SearchBox.Search(
                id: $"search_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out SoundEventId chosen))
            {
                return chosen;
            }

            return default;
        }

        public static SoundEventId? SearchFmodBuses(string id, SoundEventId initial)
        {
            string selected = "Select a bus";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.Path))
            {
                selected = initial.Path;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not BombsAwaySoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, SoundEventId>> candidates = new(CollectionHelper.ToStringDictionary(
                player.ListAllBuses(), v => v.Path, v => v.Id));

            if (SearchBox.Search(
                id: $"search_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out SoundEventId chosen))
            {
                return chosen;
            }

            return default;
        }

        public static ParameterId? SearchFmodLocalParameters(string id, ParameterId initial)
        {
            string selected = "\uf002 Select local parameter";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.EditorName))
            {
                selected = initial.EditorName;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not BombsAwaySoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, ParameterId>> candidates = new(CollectionHelper.ToStringDictionary(
                player.ListAllLocalParameters(), v => v.EditorName, v => v));

            if (SearchBox.Search(
                id: $"search_local_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out ParameterId chosen))
            {
                return chosen;
            }

            return default;
        }

        public static ParameterId? SearchFmodGlobalParameters(string id, ParameterId initial)
        {
            string selected = "\uf002 Select global parameter";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.Name))
            {
                selected = initial.Name;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not BombsAwaySoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, ParameterId>> candidates = new(CollectionHelper.ToStringDictionary(
                player.ListAllGlobalParameters(), v => v.Name!, v => v));

            if (SearchBox.Search(
                id: $"search_global_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out ParameterId chosen))
            {
                return chosen;
            }

            return default;
        }

        public static ParameterId? SearchFmodParameters(string id, ParameterId initial)
        {
            string selected = "\uf002 Select global or local parameter";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.EditorName))
            {
                selected = initial.EditorName;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not BombsAwaySoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, ParameterId>> candidates = new(CollectionHelper.ToStringDictionary(
                player.ListAllParameters(), v => v.EditorName!, v => v));

            if (SearchBox.Search(
                id: $"search_all_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out ParameterId chosen))
            {
                return chosen;
            }

            return default;
        }
    }
}
