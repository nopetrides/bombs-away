using ImGuiNET;
using BombsAway.Attributes;
using BombsAway.Editor.Utilities;
using Murder.Core.Sounds;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder;

namespace BombsAway.Editor.CustomFields
{
    [CustomFieldOf(typeof(SoundEventId))]
    internal class FmodSoundEventIdField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(
            EditorMember member, object? fieldValue)
        {
            SoundEventId description = (SoundEventId)fieldValue!;

            FmodIdKind kind = FmodIdKind.Event;
            if (AttributeExtensions.TryGetAttribute(member, out FmodIdAttribute? fmodId))
            {
                kind = fmodId.Kind;
            }

            switch (kind)
            {
                case FmodIdKind.Event:
                    if (!FmodHelpers.IsValidEvent(description))
                    {
                        ImGui.TextColored(Game.Profile.Theme.Warning, "\uf071");
                        ImGuiHelpers.HelpTooltip("Event not found in any of the banks");

                        ImGui.SameLine();
                    }

                    if (SearchBoxExtensions.SearchFmodSounds(id: member.Name, description) is SoundEventId newEvent)
                    {
                        return (modified: true, newEvent);
                    }

                    break;

                case FmodIdKind.Bus:
                    if (!FmodHelpers.IsValidBus(description))
                    {
                        ImGui.TextColored(Game.Profile.Theme.Warning, "\uf071");
                        ImGuiHelpers.HelpTooltip("Bus not found in any of the banks");

                        ImGui.SameLine();
                    }

                    if (SearchBoxExtensions.SearchFmodBuses(id: member.Name, description) is SoundEventId newBus)
                    {
                        return (modified: true, newBus);
                    }

                    break;
            }

            return (modified: false, description);
        }
    }
}
