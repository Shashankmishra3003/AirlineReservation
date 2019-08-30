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
using Microsoft.AspNetCore.Authorization;

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

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        //----< Displays the list of all the available Flights >---------

        [AllowAnonymous]
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

        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        //----< This action searches the database for available flights
        //       based on search criteria and returns the result to the view >----
        [AllowAnonymous]
        public IActionResult AvailableFlights(FlightSearch flightSearch)
        {
            if (_context.Flights.Any(fl => fl.FlightsID == 0))
            {
                return RedirectToAction("Error", "Home");
            }
            try
            {
                TempData["DOJ"] = flightSearch.DateSearch;
                
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

        //----< This action returns the seat numbers and their availability status
        //      depending based on the flight number to the view, This page is 
        //      is available only to users who are logedIn >----

        //[Authorize(Roles = "Admin,User")]
        public IActionResult BookFlight(int? id, int ticketNum, string ticketClass)
        {
            if (id == null || ticketNum == 0 || ticketClass == null )
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                TempData["FlightId"] = id;

                IQueryable<AvailableSeats> flightSeatings = Enumerable.Empty<AvailableSeats>().AsQueryable();
                var seats = _context.FlightSeatings.Where(s => s.FlightNumber.Equals(id));

                // ----< This is where we return the Available seat numbers and the seat status of 
                //          selected class of Ticket, ie First or Economy >----

                if (ticketClass.Equals("Economy"))
                {

                    flightSeatings = seats.Select(s => new AvailableSeats
                    {
                        flightSeating = new FlightSeating
                        {
                            FlightNumber = s.FlightNumber,
                            EconomyClassSeatNumbers = s.EconomyClassSeatNumbers,
                            EconomyClassSeatStatus = s.EconomyClassSeatStatus
                        },
                        NumberOfTickets = ticketNum
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
                        NumberOfTickets = ticketNum
                    });
                }
                return View(flightSeatings);
            }
            catch(Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            
        }

        //----< This action Adds the Information entered by the user on the reservation
        //      page in to database. Here the design is to convert all the First Name,
        //      Last names and DOBs into comma seperated strings and add the strings
        //      into the respective Columns. Other Columns are Seat number, Flight number
        //      Journey date and the booking date >----

        //[Authorize(Roles = "Admin,User")]
        public IActionResult BookTicket(TicketInfo bookTicket)
        {
            string firstName = string.Join(",", bookTicket.FirstName.ToArray());
            string lastName = string.Join(",", bookTicket.LastName.ToArray());
            string DOB = string.Join(",", bookTicket.DOB.ToArray());
            string DOJ = TempData["DOJ"].ToString();
            int flightNumber = (int) TempData["FlightId"];
            string seatNumbers = string.Join(",", bookTicket.seatSelection.ToArray());

            //We need to make the sected seats unavailable to other users, inorder to change 
            // the current seat status we need to fetch it form the database, change the value
            // and update the database. We follow the logic of creating a dictionary of seats before chaneg
            // and by using using the seat numbers obtained from the view we update the dictionary.
            //Once the dictionary is updated we create a new string and update it to database.

            string[] seatNumber = { };
            string[] seatStatus = { };
            IDictionary<string, string> seatsDictionary = new Dictionary<string, string>();

            var seats = _context.FlightSeatings.Where(s => s.FlightNumber.Equals(flightNumber));

            if(bookTicket.TicketClass.Equals("Economy"))
            {
                var economySeat = seats.Select(s => s.EconomyClassSeatNumbers).Single();
                
                var economyStatus = seats.Select(s => s.EconomyClassSeatStatus).Single();
                seatNumber = economySeat.Split(",");
                seatStatus = economyStatus.Split(",");

                for (int i = 0; i < seatNumber.Length; i++)
                {
                    seatsDictionary.Add(seatNumber[i], seatStatus[i]);
                }
            }
            else
            {
                var firstSeat = seats.Select(s => s.FirstClassSeatNumbers).Single();
                var firstStatus = seats.Select(s => s.FirstClassSeatStatus).Single();
                seatNumber = firstSeat.Split(",");
                seatStatus = firstStatus.Split(",");

                for (int i = 0; i < seatNumber.Length; i++)
                {
                    seatsDictionary.Add(seatNumber[i], seatStatus[i]);
                }
            }

            //Now updating the dictionary with the new value obtained
            foreach(var s in bookTicket.seatSelection.ToArray())
            {
                Console.WriteLine(s);
                seatsDictionary[s] = "X";
            }
            string[] updatedStatusArray = seatsDictionary.Values.ToArray();
            string updatedSeatStaus = string.Join(",", updatedStatusArray.ToArray());

            //Now we update the seat status column
            var flightSeatings = _context.FlightSeatings;
            var result = flightSeatings.SingleOrDefault(s => s.FlightNumber == flightNumber);
            if (bookTicket.TicketClass.Equals("Economy"))
            {               
                if(result != null)
                {
                    result.EconomyClassSeatStatus = updatedSeatStaus;
                    _context.SaveChanges();
                }
            }
            else
            {
                if (result != null)
                {
                    result.FirstClassSeatStatus = updatedSeatStaus;
                    _context.SaveChanges();
                }
            }

            var reservarion = _context.ReservationInfos;
            if(reservarion != null)
            {
                var reservationInfo = new ReservationInfo
                {
                    FlightNumber = flightNumber,
                    JourneryDate = DOJ,
                    BookingDate = DateTime.Now,
                    FirstNames = firstName,
                    LastNames = lastName,
                    DOBs = DOB,
                    SeatNumbers = seatNumbers
                };
                _context.ReservationInfos.Add(reservationInfo);
            }

            try
            {
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return RedirectToAction("Error","Home");
            }


            return RedirectToAction("Index","Home");
        }

    }
}