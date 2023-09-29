using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder.Messages
{
    /// <summary>
    /// Message used to relay collisions that apply damage
    /// </summary>
    public readonly struct DamagingCollisionMessage : IMessage
    {
        public readonly int OtherEntityId = -1;
        public readonly int DamageDealt = 1;

        public DamagingCollisionMessage(int otherEntityId, int damageDealt) : this() 
        { 
            OtherEntityId = otherEntityId;
            DamageDealt = damageDealt;
        }
    }
}
