using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder.Components
{
    [Unique]
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct GameplayStateMachineComponent : IComponent
    {
    }
}
