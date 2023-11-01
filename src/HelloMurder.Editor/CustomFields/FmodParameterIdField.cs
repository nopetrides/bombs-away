using ImGuiNET;
using HelloMurder.Editor.Utilities;
using Murder.Core.Sounds;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using HelloMurder.Core.Sounds.Fmod;
using Murder.Editor.CustomEditors;
using Murder.Editor;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using HelloMurder.Core.Sounds;
using Murder;

namespace HelloMurder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ParameterId))]
    internal class FmodParameterIdField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(
            EditorMember member, object? fieldValue)
        {
            bool modified = false;
            ParameterId parameter = (ParameterId)fieldValue!;

            ImmutableArray<ParameterLabel> labels = ImmutableArray<ParameterLabel>.Empty;
            if (Game.Instance.SoundPlayer is HelloMurderSoundPlayer player)
            {
                labels = player.ListAllParameterLabels(parameter);
            }

            // Skip showing these in prefab or world editors: we have enough information as it is.
            bool showDescription = Architect.Instance.ActiveScene is EditorScene editor && editor.EditorShown is not AssetEditor;
            ImGui.BeginChild($"selection_parameters", new System.Numerics.Vector2(
                x: 300, y: ImGui.GetFontSize() * (showDescription ? (labels.Length * 1.75f + 1.5f) : 1.5f)));

            SoundParameterKind kind = SoundParameterKind.All;
            if (AttributeExtensions.TryGetAttribute(member, out SoundParameterAttribute? soundAttribute))
            {
                kind = soundAttribute.Kind;
            }

            if (!Utilities.FmodHelpers.IsValidParameter(parameter))
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "\uf071");
                ImGuiHelpers.HelpTooltip("Parameter not found in any of the banks");

                ImGui.SameLine();
            }

            ParameterId? newParameter = kind switch
            {
                SoundParameterKind.Local => SearchBoxExtensions.SearchFmodLocalParameters(id: member.Name, parameter),
                SoundParameterKind.Global => SearchBoxExtensions.SearchFmodGlobalParameters(id: member.Name, parameter),
                _ => SearchBoxExtensions.SearchFmodParameters(id: member.Name, parameter)
            };

            if (newParameter is not null)
            {
                modified = true;
                parameter = newParameter.Value;
            }

            ImGuiHelpers.HelpTooltip("Parameter id");

            if (showDescription && labels.Length != 0)
            {
                using TableMultipleColumns t = new($"labels_fmod_parameter", ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Sortable, -1, -1);

                foreach (var label in labels)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(label.Label);

                    ImGui.TableNextColumn();

                    ImGui.Text(label.Value.ToString());
                    ImGui.TableNextRow();
                }
            }

            ImGui.EndChild();

            return (modified, parameter);
        }
    }
}
