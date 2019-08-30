using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineReseravtionSystem.Models
{
    public class TicketInfo
    {
        public List<string> FirstName { get; set; }
        public List<string> LastName { get; set; }
        public List<string> DOB { get; set; }
        public List<string> seatSelection { get; set; }
    }
}
