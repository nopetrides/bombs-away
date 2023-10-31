using ImGuiNET;
using HelloMurder.Editor.Utilities;
using Murder.Core.Sounds;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace HelloMurder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ParameterId))]
    internal class FmodParameterIdField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(
            EditorMember member, object? fieldValue)
        {
            ParameterId parameter = (ParameterId)fieldValue!;
            
            if (ImGuiHelpers.DeleteButton("No parameter selected."))
            {
                return (modified: true, new ParameterId());
            }
            
            ImGui.SameLine();

            if (SearchBoxExtensions.SearchFmodParameters(id: string.Empty, parameter) is ParameterId newParameter)
            {
                return (modified: true, newParameter);
            }
            
            return (modified: false, parameter);
        }
    }
}
