﻿using ImGuiNET;
using HelloMurder.Core.Sounds;
using HelloMurder.Core.Sounds.Fmod;
using Murder;
using Murder.Core.Sounds;
using Murder.Editor.ImGuiExtended;
using Murder.Services;

namespace HelloMurder.Editor.Utilities
{
    internal static class SearchBoxExtensions
    {
        public static SoundEventId? SearchFmodSounds(string id, SoundEventId initial)
        {
            string selected = "Select a sound";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.Path))
            {
                selected = initial.Path;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, SoundEventId>> candidates = new(player.ListAllEvents()
                .ToDictionary(v => v.Path, v => v.Id));

            if (ImGuiHelpers.IconButton('', $"play_sound_{id}"))
            {
                _ = SoundServices.Play(initial);
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

            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, SoundEventId>> candidates = new (player.ListAllBuses()
                .ToDictionary(v => v.Path, v => v.Id));
            
            if (SearchBox.Search(
                id: $"search_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out SoundEventId chosen))
            {
                return chosen;
            }

            return default;
        }

        public static ParameterId? SearchFmodParameters(string id, ParameterId initial)
        {
            string selected = "Select a parameter";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial.Name))
            {
                selected = initial.Name;
                hasValue = true;
            }

            if (Game.Instance.SoundPlayer is not HelloMurderSoundPlayer player)
            {
                return initial;
            }

            Lazy<Dictionary<string, ParameterId>> candidates = new (player.ListAllParameters()
                .Select(p => p.ToParameterId()).ToDictionary(v => v.Name!, v => v));

            ImGui.SameLine();

            if (SearchBox.Search(
                id: $"search_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out ParameterId chosen))
            {
                return chosen;
            }

            return default;
        }
    }
}
