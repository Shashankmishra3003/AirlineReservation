using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineReseravtionSystem.Models
{
    public class FlightSeating
    {
        public int FlightSeatingID { get; set; }
        public int FlightNumber { get; set; }
        public string FirstClassSeatNumbers { get; set; }
        public string FirstClassSeatStatus { get; set; }
        public string EconomyClassSeatNumbers { get; set; }
        public string EconomyClassSeatStatus { get; set; }
    }

    public class AvailableSeats
    {
        public FlightSeating flightSeating { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
