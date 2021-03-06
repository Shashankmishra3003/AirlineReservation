﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdministratorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HttpClient httpClient { get; set; }
        private string baseUrl;
        public ObservableCollection<Flight> flightDetailsList { get; set; }
        public ObservableCollection<Ticket> ticketDetailsList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            baseUrl = "https://localhost:44357/api/AddFlight";
            httpClient = new HttpClient();
        
            this.Loaded += MainWindow_Loaded;
        }
        public MainWindow(string url)
        {
            baseUrl = url;
            httpClient = new HttpClient();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = GetAllFlights();

        }

        //----< This method creates a byte array of all the fields entered by the admin.
        //After ading the byte arrays to multipart form data content,the data is sent to the database. >----
        public async Task<HttpResponseMessage> sendNewFlightInfo(Flight flight)
        {
            MultipartFormDataContent multipart = new MultipartFormDataContent();

            byte[] num = Encoding.ASCII.GetBytes(flight.flightNumber.ToString());
            ByteArrayContent flightNumber = new ByteArrayContent(num);

            byte[] name = Encoding.ASCII.GetBytes(flight.flightName);
            ByteArrayContent flightName = new ByteArrayContent(name);

            byte[] src = Encoding.ASCII.GetBytes(flight.source);
            ByteArrayContent source = new ByteArrayContent(src);

            byte[] desti = Encoding.ASCII.GetBytes(flight.destination);
            ByteArrayContent destination = new ByteArrayContent(desti);

            byte[] arr = Encoding.ASCII.GetBytes(flight.arrival);
            ByteArrayContent arrival = new ByteArrayContent(arr);

            byte[] dept = Encoding.ASCII.GetBytes(flight.departure);
            ByteArrayContent departure = new ByteArrayContent(dept);

            byte[] date = Encoding.ASCII.GetBytes(flight.departureDate);
            ByteArrayContent departureDate = new ByteArrayContent(date);

            byte[] ecoS = Encoding.ASCII.GetBytes(flight.economySeats.ToString());
            ByteArrayContent economySeats = new ByteArrayContent(ecoS);

            byte[] ecoP= Encoding.ASCII.GetBytes(flight.economyPrice.ToString());
            ByteArrayContent economyPrice = new ByteArrayContent(ecoP);

            byte[] firS = Encoding.ASCII.GetBytes(flight.firstSeats.ToString());
            ByteArrayContent firstSeats = new ByteArrayContent(firS);

            byte[] firP = Encoding.ASCII.GetBytes(flight.firstPrice.ToString());
            ByteArrayContent firstPrice = new ByteArrayContent(firP);


            multipart.Add(flightNumber, "flightNumber");
            multipart.Add(flightName, "flightName");
            multipart.Add(source, "source");
            multipart.Add(destination, "destination");
            multipart.Add(arrival, "arrival");
            multipart.Add(departure, "departure");
            multipart.Add(departureDate, "departureDate");
            multipart.Add(economySeats, "economySeats");
            multipart.Add(economyPrice, "economyPrice");
            multipart.Add(firstSeats, "firstSeats");
            multipart.Add(firstPrice, "firstPrice");

            return await httpClient.PostAsync(baseUrl, multipart);
        }

        //Method to handle the button click
        public async void btnNewFlight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedEco = txtEcoSeats.Text;
                string selectedFir = txtFirSeats.Text;
                var flight = new Flight()
                {
                    flightNumber = Int32.Parse(txtFlightNumber.Text),
                    flightName = txtFlightName.Text,
                    source = txtSource.Text,
                    destination = txtDestination.Text,
                    arrival = txtArrival.Text,
                    departure = txtDeparture.Text,
                    departureDate = txtDate.Text,
                    economySeats = Int32.Parse(selectedEco),
                    economyPrice = Int32.Parse(txtEcoPrice.Text),
                    firstSeats = Int32.Parse(selectedFir),
                    firstPrice = Int32.Parse(txtFirPrice.Text)
                };
                string url = "https://localhost:44357/api/AddFlight";
                MainWindow client = new MainWindow(url);

                Task<HttpResponseMessage> tup = client.sendNewFlightInfo(flight);
                MessageBox.Show("Flight Successfully Added!","Result", MessageBoxButton.OK,MessageBoxImage.Information);
                _ = GetAllFlights();
                _ = GetAllTickets();

            }
            catch(Exception)
            {
                MessageBox.Show("Flight Not Added, Please try Again!!");
            }

        }

        //----< Getting the list of Flights from the Database >----
        public async Task GetAllFlights()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl);
            var flightList = new List<string>();
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                flightDetailsList = new ObservableCollection<Flight>();
                foreach (var item in jArr)
                {
                    string departure = item["departureDate"].ToString().Substring(0, item["departureDate"].ToString().IndexOf(" ")+1);
                    flightDetailsList.Add(new Flight()
                    {
                        flightNumber = Int32.Parse(item["flightNumber"].ToString()),
                        flightName = item["flightName"].ToString(),
                        source = item["source"].ToString(),
                        destination = item["destination"].ToString(),
                        departureDate = departure,
                        departure = item["departsOn"].ToString(),
                        arrival = item["arrivesOn"].ToString(),
                        economySeats = Int32.Parse(item["economyNos"].ToString()),
                        firstSeats = Int32.Parse(item["firstNos"].ToString()),
                        economyPrice = Int32.Parse(item["priceEconomy"].ToString()),
                        firstPrice = Int32.Parse(item["priceFirst"].ToString())
                    });
                }
                    
            }
            flightListView.ItemsSource = flightDetailsList;

            await GetAllTickets();
        }

        //----< Gets the List of all the flights booked by all the users >----
        public async Task GetAllTickets()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl + "/1");
            var ticketList = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                ticketDetailsList = new ObservableCollection<Ticket>();
                foreach (var item in jArr)
                {
                    string departure = item["journeyDate"].ToString().Substring(0, item["journeyDate"].ToString().IndexOf(" ") + 1);
                    ticketDetailsList.Add(new Ticket()
                    {
                        ReservationInfoID = Int32.Parse(item["reservationInfoID"].ToString()),
                        FlightNumber = Int32.Parse(item["flightNumber"].ToString()),
                        JourneryDate = departure,
                        BookingDate = item["bookingDate"].ToString(),
                        FirstName = item["firstNames"].ToString(),
                        LastName = item["lastNames"].ToString(),
                        DOB = item["doBs"].ToString(),
                        SeatNumber = item["seatNumbers"].ToString(),
                    });
                }
                    
            }
            ticketListView.ItemsSource = ticketDetailsList;
        }
    }


    public class Flight
    {
        public int flightNumber { get; set; }
        public string flightName { get; set; }
        public string source { get; set; }
        public string destination { get; set;}
        public string arrival { get; set; }
        public string departure { get; set; }
        public string departureDate { get; set; }
        public int economySeats { get; set; }
        public int economyPrice { get; set; }
        public int firstSeats { get; set;}
        public int firstPrice { get; set; }
    }

    public class Ticket
    {
        public int ReservationInfoID { get; set; }
        public int FlightNumber { get; set; }
        public string JourneryDate { get; set; }
        public string BookingDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB{ get; set; }
        public string SeatNumber { get; set; }
    }
}
