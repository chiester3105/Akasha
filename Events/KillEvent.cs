using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akasha.Events
{
    public class KillEvent
    {
        public readonly PersistentID killer;
        public readonly PersistentID victim;
        public readonly string weaponName;
        public KillEvent(PersistentID killer, PersistentID victim, string weaponName)
        {
            this.killer = killer;
            this.victim = victim;
            this.weaponName = weaponName;
        }
    }
}
