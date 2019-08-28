using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using AirlineReseravtionSystem.Data;
using AirlineReseravtionSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AirlineReseravtionSystem.Controllers
{

    public class FlightsController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ApplicationDbContext _context;
        
        public FlightsController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //----< Displays the list of all the available Flights >---------

        public IActionResult ViewFlights()
        {
            try
            {
                return View(_context.Flights.ToList<Flights>());
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //----< Displays the Details of the Selected Flight >---------

        public ActionResult FlightDetails(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            Flights flight = _context.Flights.Find(id);

            if(flight == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return View(flight);
        }

        //----<Gets form to edit a specific flight detail >---------

        [HttpGet]
        public IActionResult EditFlightDetails(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            Flights flight = _context.Flights.Find(id);

            if(flight == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(flight);
        }

        //----<Post back edited result of a specific flight detail >---------

        [HttpPost]
        public IActionResult EditFlightDetails(int? id, Flights flt)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var flight = _context.Flights.Find(id);
            if(flight != null)
            {
                flight.FlightNumber = flt.FlightNumber;
                flight.FlightName = flt.FlightName;
                flight.Source = flt.Source;
                flight.Destination = flt.Destination;
                flight.DepartsOn = flt.DepartsOn;
                flight.ArrivesOn = flt.ArrivesOn;
                flight.EconomyNos = flt.EconomyNos;
                flight.FirstNos= flt.FirstNos;
                flight.PriceEconomy= flt.PriceEconomy;
                flight.PriceFirst= flt.PriceFirst;
                try
                {
                    _context.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        //----< Deletes the selected flight from the list >--------

        public IActionResult DeleteFlight(int? id)
        {
            if(id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                var flight = _context.Flights.Find(id);
                if(flight != null)
                {
                    _context.Remove(flight);
                    _context.SaveChanges();
                }
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index");
        }

        public IActionResult AvailableFlights(FlightSearch flightSearch)
        {
            if (_context.Flights.Any(fl => fl.FlightsID == 0))
            {
                return RedirectToAction("Error", "Home");
            }
            try
            {
                /*
                 * ----< The if condition checks whether there ia a flight available for selected inputs 
                 *          Also the Controller returns only the required fields to the View, This is where we are returning the price
                 *          of the selected class so that the correct price can be displayed in View>----
                 */
                int totalTicketCount = flightSearch.AdultSearch + flightSearch.ChildrenSearch + flightSearch.InfantSearch; 
                IQueryable<ReturningValue> flightOrder = Enumerable.Empty<ReturningValue>().AsQueryable();

                if (flightSearch.ClassSearch.Equals("economy"))
                {
                    var search = _context.Flights.Where(s => s.Source.Equals(flightSearch.FromSearch)
                                                     && s.Destination.Equals(flightSearch.ToSearch)
                                                     && s.DepartureDate.Equals(flightSearch.DateSearch)
                                                     && s.EconomyNos >= (flightSearch.AdultSearch + flightSearch.ChildrenSearch + flightSearch.InfantSearch));
                         flightOrder = search.OrderBy(s => s.DepartsOn)
                        .Select(s => new ReturningValue
                        {
                            FlightNumber = s.FlightNumber,
                            Name = s.FlightName,
                            Departure = s.DepartsOn,
                            Arrival = s.ArrivesOn,
                            PFirst = 0,                     //Selected class is Economy, so make First 0
                            PEconomy = s.PriceEconomy * totalTicketCount,
                            TicketSelected = totalTicketCount
                        });
                }
                if (flightSearch.ClassSearch.Equals("first"))
                {
                    var search = _context.Flights.Where(s => s.Source.Equals(flightSearch.FromSearch)
                                                     && s.Destination.Equals(flightSearch.ToSearch)
                                                     && s.DepartureDate.Equals(flightSearch.DateSearch)
                                                     && s.FirstNos >= (flightSearch.AdultSearch + flightSearch.ChildrenSearch + flightSearch.InfantSearch));
                         flightOrder = search.OrderBy(s => s.DepartsOn)
                        .Select(s => new ReturningValue
                        {
                            FlightNumber = s.FlightNumber,
                            Name = s.FlightName,
                            Departure = s.DepartsOn,
                            Arrival = s.ArrivesOn,
                            PFirst = s.PriceFirst * totalTicketCount,
                            PEconomy = 0,               //Selected class is Economy, so make Economy 0
                            TicketSelected=totalTicketCount
                        });

                   
                }
                return View(flightOrder);
            }
            catch (Exception)
            {
                //This is where we return no flights available for the given search
                return RedirectToAction("Error", "Home");
            }
        }


        public IActionResult BookFlight(int? id, int ticketCount, string flightClass)
        {
            if (id == null || ticketCount == 0 || flightClass == null )
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {

                IQueryable<AvailableSeats> flightSeatings = Enumerable.Empty<AvailableSeats>().AsQueryable();
                var seats = _context.FlightSeatings.Where(s => s.FlightNumber.Equals(id));

                // ----< This is where we return the Available seat numbers and the seat status of 
                //          selected class of Ticket, ie First or Economy >----

                if (flightClass.Equals("Economy"))
                {

                    flightSeatings = seats.Select(s => new AvailableSeats
                    {
                        flightSeating = new FlightSeating
                        {
                            FlightNumber = s.FlightNumber,
                            EconomyClassSeatNumbers = s.EconomyClassSeatNumbers,
                            EconomyClassSeatStatus = s.EconomyClassSeatStatus
                        },
                        NumberOfTickets = ticketCount
                    });
                                                                
                }
                else
                {
                    flightSeatings = seats.Select(s => new AvailableSeats
                    {
                        flightSeating = new FlightSeating
                        {
                            FlightNumber = s.FlightNumber,
                            FirstClassSeatNumbers = s.FirstClassSeatNumbers,
                            FirstClassSeatStatus = s.FirstClassSeatStatus
                        },
                        NumberOfTickets = ticketCount
                    });
                }
                return View(flightSeatings);
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            
        }


    }
}