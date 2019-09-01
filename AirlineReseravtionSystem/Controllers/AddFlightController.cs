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
        public AddFlightController(ApplicationDbContext context)
        {
            _context = context;
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

            return Ok();
        }
    }

    
}