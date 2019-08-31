using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            //httpClient.BaseAddress = new Uri("https://localhost:44357/api/AddFlight");
            ////httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //this.Loaded += MainWindow_Loaded;
        }
        public MainWindow(string url)
        {
            baseUrl = url;
            httpClient = new HttpClient();
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage response = await httpClient.GetAsync("/api/AddFlight");
            response.EnsureSuccessStatusCode();
            flightListView.ItemsSource = await GetAllFlights();
            flightListView.ScrollIntoView(flightListView.ItemContainerGenerator.Items[flightListView.Items.Count - 1]);
        }

        //This method creates a byte array of all the fields entered by the admin.
        //After ading the byte arrays to multipart form data content,the data is sent to the database.
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
        public  async void btnNewFlight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var flight = new Flight()
                {
                    flightNumber = Int32.Parse(txtFlightNumber.Text),
                    flightName = txtFlightName.Text,
                    source = txtSource.Text,
                    destination = txtDestination.Text,
                    arrival = txtArrival.Text,
                    departure = txtDeparture.Text,
                    departureDate = txtDate.Text,
                    economySeats = Int32.Parse(txtEcoSeats.Text),
                    economyPrice = Int32.Parse(txtEcoPrice.Text),
                    firstSeats = Int32.Parse(txtFirSeats.Text),
                    firstPrice = Int32.Parse(txtFirPrice.Text)
                };
                string url = "https://localhost:44357/api/AddFlight";
                MainWindow client = new MainWindow(url);

                Task<HttpResponseMessage> tup = client.sendNewFlightInfo(flight);
                MessageBox.Show("Flight Successfully Added!","Result", MessageBoxButton.OK,MessageBoxImage.Information);
                //flightListView.ItemsSource = await GetAllFlights();
                //flightListView.ScrollIntoView(flightListView.ItemContainerGenerator.Items[flightListView.Items.Count - 1]);

            }
            catch(Exception)
            {
                MessageBox.Show("Flight Not Added, Please try Again!!");
            }

        }

        //Getting the list of Flights from the Database
        public async Task<IEnumerable<string>> GetAllFlights()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl);
            var flightList = new List<string>();
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    flightList.Add(item.ToString());
            }
            return flightList;
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
}
