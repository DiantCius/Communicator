using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class ChildPerson
    {
        public int ChildId { get; set; }
        public Child Child { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
