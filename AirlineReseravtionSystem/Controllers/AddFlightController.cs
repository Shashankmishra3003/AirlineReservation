using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirlineReseravtionSystem.Data;
using Microsoft.AspNetCore.Mvc;
using AirlineReseravtionSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace AirlineReseravtionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddFlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private Dictionary<int, string> EconomySeatDictionary6, EconomySeatDictionary12, EconomySeatDictionary18,EconomyStatusDictionary;
        private Dictionary<int, string> FirstSeatDictionary,FirstStatusDictionary;
        public AddFlightController(ApplicationDbContext context)
        {
            _context = context;
            InitializeDictionary();      

        }

        //----< This method Initializes all the different dictionaries needed for setting the seat numbers when
        //      user selects the number of seats while adding a new flight,
        //      There are 2 type of Dictionaries, one for seat numbers and other for initial seat status.
        //      According to the design fist class compartment is in the beginning, so depending upon the number of
        //      first class seats the Economy class seat number will vary there there are 3 dictionaries for Economy class >----
        private void InitializeDictionary()
        {
            EconomySeatDictionary6 = new Dictionary<int, string>
            {
                { 24,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F" },
                { 36,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F"},
                { 48,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F,8A,8B,8C,8D,8E,8F,9A,9B,9C,9D,9E,9F"}
            };
            EconomySeatDictionary12 = new Dictionary<int, string>
            {
                { 24,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F" },
                { 36,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F"},
                { 48,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F,8A,8B,8C,8D,8E,8F,9A,9B,9C,9D,9E,9F"}
            };
            EconomySeatDictionary18 = new Dictionary<int, string>
            {
                { 24,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F" },
                { 36,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F"},
                { 48,"2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F,4A,4B,4C,4D,4E,4F,5A,5B,5C,5D,5E,5F,6A,6B,6C,6D,6E,6F,7A,7B,7C,7D,7E,7F,8A,8B,8C,8D,8E,8F,9A,9B,9C,9D,9E,9F"}
            };
            FirstSeatDictionary = new Dictionary<int, string>
            {
                { 6,"1A,1B,1C,1D,1E,1F" },
                { 12,"1A,1B,1C,1D,1E,1F,2A,2B,2C,2D,2E,2F"},
                { 18,"1A,1B,1C,1D,1E,1F,2A,2B,2C,2D,2E,2F,3A,3B,3C,3D,3E,3F"}
            };
            EconomyStatusDictionary = new Dictionary<int, string>
            {
                {24,"O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O" },
                {36,"O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O" },
                {48,"O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O" }
            };
            FirstStatusDictionary = new Dictionary<int, string>
            {
                {6,"O,O,O,O,O,O" },
                {12,"O,O,O,O,O,O,O,O,O,O,O,O" },
                {18,"O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O,O" }
            };
        }

        [HttpGet]
        public IEnumerable<Flights> GetFlights()
        {
            List<Flights> flightList = new List<Flights>();
            var flights = _context.Flights.ToList();
            foreach (var f in flights)
            {
                flightList.Add(new Flights()
                {
                    FlightNumber = f.FlightNumber,
                    FlightName = f.FlightName,
                    Source = f.Source,
                    Destination = f.Destination,
                    ArrivesOn = f.ArrivesOn,
                    DepartsOn = f.DepartsOn,
                    DepartureDate = f.DepartureDate,
                    EconomyNos = f.EconomyNos,
                    PriceEconomy = f.PriceEconomy,
                    FirstNos = f.FirstNos,
                    PriceFirst = f.PriceFirst
                });
            }
            return flightList;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> AddFlight()
        {
            var request = HttpContext.Request;

            var flight = new Flights
            {
                FlightNumber = Int32.Parse(request.Form["flightNumber"]),
                FlightName = request.Form["flightName"],
                Source = request.Form["source"],
                Destination = request.Form["destination"],
                ArrivesOn = request.Form["arrival"],
                DepartsOn = request.Form["departure"],
                DepartureDate =DateTime.Parse(request.Form["departureDate"]),
                EconomyNos = Int32.Parse(request.Form["economySeats"]),
                FirstNos = Int32.Parse(request.Form["firstSeats"]),
                PriceEconomy = Int32.Parse(request.Form["economyPrice"]),
                PriceFirst = Int32.Parse(request.Form["firstPrice"])
            };
            _context.Flights.Add(flight);
            _context.SaveChanges();

            //----< After saving the New Flight information into the Database, the seating arrangement for the flight 
            //      must be added into the FlightSeating table. We get the seat number and the status into a string and 
            //      add that string to the database along with flight number >----

            int economyNOS = Int32.Parse(request.Form["economySeats"]);
            int firstNOS = Int32.Parse(request.Form["firstSeats"]);

            string economySeatString,economySeatStatusString,firstSeatString,firstStatusString;

            if (firstNOS == 6)
            {
                economySeatString = EconomySeatDictionary6[economyNOS];
                economySeatStatusString = EconomyStatusDictionary[economyNOS];
                firstSeatString = FirstSeatDictionary[firstNOS];
                firstStatusString = FirstStatusDictionary[firstNOS];
            }
            else if(firstNOS == 12)
            {
                economySeatString = EconomySeatDictionary12[economyNOS];
                economySeatStatusString = EconomyStatusDictionary[economyNOS];
                firstSeatString = FirstSeatDictionary[firstNOS];
                firstStatusString = FirstStatusDictionary[firstNOS];
            }
            else
            {
                economySeatString = EconomySeatDictionary18[economyNOS];
                economySeatStatusString = EconomyStatusDictionary[economyNOS];
                firstSeatString = FirstSeatDictionary[firstNOS];
                firstStatusString = FirstStatusDictionary[firstNOS];
            }

            var seating = new FlightSeating
            {
                FlightNumber = Int32.Parse(request.Form["flightNumber"]),
                FirstClassSeatNumbers = firstSeatString,
                FirstClassSeatStatus = firstStatusString,
                EconomyClassSeatNumbers = economySeatString,
                EconomyClassSeatStatus = economySeatStatusString
            };
            _context.FlightSeatings.Add(seating);
            _context.SaveChanges();

            return Ok();
        }
    }

    
}