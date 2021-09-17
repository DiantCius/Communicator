using Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Features.Activities
{
    public class ActivityResponse
    {
        public List<Activity> Activities { get; set; }
        public int Count { get; set; }
    }
}
