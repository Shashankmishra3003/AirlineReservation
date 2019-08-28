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

                new Flights{FlightNumber=4567,FlightName="Air India",Source="Delhi",Destination="Mumbai",DepartsOn=new DateTime().AddHours(15).AddMinutes(00).ToString("HH:mm"),
                                DepartureDate=new DateTime().AddDays(23).AddMonths(07).AddYears(2018),ArrivesOn=new DateTime().AddHours(19).AddMinutes(00).ToString("HH:mm"), EconomyNos=120,FirstNos=10,PriceEconomy=200,PriceFirst=500}
            };

            foreach(Flights f in flights)
            {
                context.Flights.Add(f);
            }
            context.SaveChanges()
;        }
    }
}
