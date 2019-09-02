using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirlineReseravtionSystem.Models;

namespace AirlineReseravtionSystem.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if(context.Flights.Any())
            {
                return;
            }

            var flights = new Flights[]
            {
                new Flights{FlightNumber=1234,FlightName="United",Source="Syracuse",Destination="New Jersey",DepartsOn=new DateTime().AddHours(9).AddMinutes(10).ToString("HH:mm"),
                               DepartureDate=new DateTime().AddDays(22).AddMonths(07).AddYears(2018) ,ArrivesOn=new DateTime().AddHours(14).AddMinutes(25).ToString("HH:mm"), EconomyNos=100,FirstNos=5, PriceEconomy=100,PriceFirst=400},

                new Flights{FlightNumber=4567,FlightName="American Airline",Source="JFK",Destination="Syracuse",DepartsOn=new DateTime().AddHours(15).AddMinutes(00).ToString("HH:mm"),
                                DepartureDate=new DateTime().AddDays(26).AddMonths(07).AddYears(2018),ArrivesOn=new DateTime().AddHours(19).AddMinutes(00).ToString("HH:mm"), EconomyNos=120,FirstNos=10,PriceEconomy=200,PriceFirst=500}
            };

            foreach (Flights f in flights)
            {
                context.Flights.Add(f);
            }
            context.SaveChanges();

            var flightseating = new FlightSeating[]
            {
                new FlightSeating{FlightNumber=1234, FirstClassSeatNumbers="1A,1B,1C,1D,1E,1F,2A,2B,2C,2D,2E,2F",FirstClassSeatStatus="O,X,O,O,O,O,O,X,X,O,O,O",
                                    EconomyClassSeatNumbers="3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F",EconomyClassSeatStatus="O,X,X,O,O,O,O,X,X,O,O,O,O,X,X,O,O,O"},
                new FlightSeating{FlightNumber=4567, FirstClassSeatNumbers="1A,1B,1C,1D,1E,1F,2A,2B,2C,2D,2E,2F",FirstClassSeatStatus="O,X,O,O,O,O,O,X,X,O,O,O",
                                    EconomyClassSeatNumbers="3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F",EconomyClassSeatStatus="O,X,X,O,O,O,O,X,X,O,O,O,O,X,X,O,O,O"}
            };

            foreach(FlightSeating f in flightseating)
            {
                context.FlightSeatings.Add(f);
            }
            context.SaveChanges();
;        }
    }
}
