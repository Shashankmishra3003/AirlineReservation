using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineReseravtionSystem.Models
{
    public class Flights
    {
        public int FlightsID { get; set; }

        [Display(Name ="Flight Number")]
        public int FlightNumber { get; set; }

        [Display(Name = "Flight Name")]
        public string FlightName { get; set; }

        [Display(Name = "Source")]
        public string Source { get; set; }

        [Display(Name = "Destination")]
        public string Destination { get; set; }

        [Display(Name ="Daparture Date")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Depature Time")]
        public string DepartsOn { get; set; }

        [Display(Name = "Arrival Time")]
        public string ArrivesOn { get; set; }

        [Display(Name = "Economy Seats")]
        public int EconomyNos { get; set; }

        [Display(Name = "Firat Class Seats")]
        public int FirstNos { get; set; }

        [Display(Name = "Price Economy")]
        public int PriceEconomy { get; set; }

        [Display(Name = "Price First Class")]
        public int PriceFirst { get; set; }

    }



}
