using Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Children
{
    public class ChildrenResponse
    {
        public List<Child> Children { get; set; }
        public int Count { get; set; }
    }
}
