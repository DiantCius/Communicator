using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Babysitters
{
    public class BabysitterResponse
    {
        public List<Babysitter> Babysitters { get; set; }
        public int Count { get; set; }
    }
}
