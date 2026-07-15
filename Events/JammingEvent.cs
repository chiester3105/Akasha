using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akasha.Events
{
    public class JammingEvent
    {
        public readonly PersistentID owner;
        public JammingEvent(PersistentID owner)
        {
            this.owner = owner;
        }
    }
}
