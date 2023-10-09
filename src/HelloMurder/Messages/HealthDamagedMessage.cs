using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder.Messages
{
    public readonly struct HealthDamagedMessage : IMessage
    {
        public readonly int DamageDealt;
        public HealthDamagedMessage(int damageDealt) : this()
        {  
            DamageDealt = damageDealt;
        }
    }
}
