
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestaurantReservations
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        public readonly string[] tableSeats = { "1", "2", "3", "4", "5" };
        public readonly string[] tableNames = { "Table 1", "Table 2", "Table 3", "Table 4", "Table 5", "Table 6", "Table 7", "Table 8", "Table 9" };
        public readonly string[] Rtimes = { "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00" };
        public string? getName, getTableNr, getDate, getTime; 
        public int getSeatsNr;


        public List<Reservation> reservationsList = new List<Reservation>();
        //{
        //new Reservation("Ionut","2022-11-25","18:00","Table 3",4),
        //new Reservation("Ionut2","2022-11-25","12:00","Table 3",4),
        //new Reservation("Catalin","2022-11-28","20:00","Table 5",2),
        //new Reservation("Luca","2022-12-03","19:00", "Table 1",5),
        //new Reservation("Cristina","2022-12-25","17:00","Table 2",1),
        //new Reservation("Anna","2022-10-27","18:00","Table 5",5)
        //};

        public List<Reservation> inputList = new List<Reservation>();
        
      
        public  BookingPage()
        {
            InitializeComponent();
            nrPersonsBox.ItemsSource = tableSeats;
            tablesBox.ItemsSource = tableNames;
            timeBox.ItemsSource = Rtimes;

            Task task = ReadFromFile();            
            

        }

        private void saveReservation_Click(object sender, RoutedEventArgs e)
        {
            SaveReservations(); 
        }

        private void showReservations_Click(object sender, RoutedEventArgs e)
        {
             
            UpdateReservationsList();
            DisplayContent(reservationsList);
            
        }

        private void cancelReservation_Click(object sender, RoutedEventArgs e)
        {
            DeleteReservation();

        }

        private async void saveToFile_Click(object sender, RoutedEventArgs e)
        {
            await WriteToFile(reservationsList);

        }

        private void modifyReservation_Click(object sender, RoutedEventArgs e)
        {
            ModifyReservation();
        }
        private void DisplayContent(List<Reservation> lista)
        {              
            printReservations.ItemsSource = lista;
        }

        private bool SaveReservations()
        {
            if (IsValid())
            {
                getName = nameBox.Text.First().ToString().ToUpper() + nameBox.Text.Substring(1);
                getDate = datePick.Text;
                getTime = timeBox.Text;
                getTableNr = tablesBox.Text;
                getSeatsNr =int.Parse(nrPersonsBox.Text);                
                Reservation newItem = new Reservation(getName, getDate, getTime, getTableNr,getSeatsNr);
                if (CheckReservation(newItem))
                    MessageBox.Show("Table is already booked! Try again!", " Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    reservationsList.Add(newItem);
                    inputList.Add(newItem);
                    printReservations.ItemsSource = null;
                    DisplayContent(inputList);
                    printReservations.Items.Refresh();
                    MessageBox.Show("Your reservation has been saved!", "Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
                MessageBox.Show("Empty or invalid field! Try Again!", "Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Error);
            ClearFields();
            return true;
        }
        
        private void UpdateReservationsList()
        {
            
            reservationsList = reservationsList.OrderBy(x => x.Date).ThenBy(x => x.Time).ToList();
            foreach (Reservation reservation in reservationsList)
                if (DateTime.Parse(reservation.Date) < DateTime.Today)
                    MessageBox.Show($"{reservation.Print()} has been deleted because its passed due!", "Delete old Reservations", MessageBoxButton.OK, MessageBoxImage.Information);
            reservationsList.RemoveAll(x => DateTime.Parse(x.Date) < DateTime.Today);
            reservationsList.Distinct().ToList();   


        }
        private void ClearFields()
        {
            nameBox.Clear();
            datePick.Text = "";
            timeBox.Text = "";
            tablesBox.Text = "";
            nrPersonsBox.Text = "";

        }

        private async Task ReadFromFile()
        {
            string fileName = "ReservationsDatabase.json";
            using FileStream openStream = File.OpenRead(fileName);
            List<Reservation>? reservationJson = await JsonSerializer.DeserializeAsync<List<Reservation>>(openStream);
            if (reservationJson != null)
            {
                foreach (Reservation reservation in reservationJson)
                {
                    if (!reservationsList.Contains(reservation))
                        reservationsList.Add(reservation);

                }

            }
        }
        
        private async Task WriteToFile(List<Reservation> lista)
        {
            string fileName = "ReservationsDatabase.json";            
            using FileStream createStream = File.Create(fileName);           
            await JsonSerializer.SerializeAsync(createStream, lista);
            await createStream.DisposeAsync();
            MessageBox.Show($"Saved to {fileName}","Saave Reservations",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private bool IsValid()
        {
            bool result;
            if (string.IsNullOrEmpty(nameBox.Text) && string.IsNullOrEmpty(datePick.Text) && string.IsNullOrEmpty(timeBox.Text) &&
                string.IsNullOrEmpty(tablesBox.Text) && string.IsNullOrEmpty(nrPersonsBox.Text))
                result = false;
            else
                result = true;

            return result;

        }

        private void modifyBtn_Click(object sender, RoutedEventArgs e)
        {
            
            SaveReservations();
            printReservations.ItemsSource = null;
            UpdateReservationsList();
            DisplayContent(reservationsList);
            printReservations.Items.Refresh();
            
            modifyBtn.Visibility = Visibility.Hidden;
            
        }

        private bool CheckReservation(Reservation reservation)
        {
            bool result = true;
            if (reservationsList.Any(x => (x.Date.Equals(reservation.Date)) && (x.Time.Equals(reservation.Time)) &&
                            (x.TableNumber.Equals(reservation.TableNumber))
                            && ((x.nrOfSeats + reservation.nrOfSeats) > 5)))
                result = true;
            else
                result = false;
            return result;

        }
        private void DeleteReservation()
        {
            if (printReservations.SelectedItem == null)
                MessageBox.Show("You need to first select a reservation from the list to delete it!", "Delete Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                Reservation? reservation = printReservations.SelectedItem as Reservation;
                if (reservation != null)
                {
                    reservationsList.Remove(reservation);
                    printReservations.ItemsSource= null;
                    UpdateReservationsList();
                    DisplayContent(reservationsList);
                    printReservations.Items.Refresh();
                    MessageBox.Show("Your selected reservation has been canceled!", "Delete Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }

        private void ModifyReservation()
        {
           
            if (printReservations.SelectedItem == null)
                MessageBox.Show("You need to first select a reservation from the list to delete it!", "Delete Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                Reservation? reservation = printReservations.SelectedItem as Reservation;
                if (reservation != null)
                {
                    string msg = "Are you sure you want to modify the selected reservation?\n(Selected reservation will be removed!)";
                    MessageBoxResult result = MessageBox.Show(msg, "Modify Reservation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        nameBox.Text = reservation.Name;
                        datePick.Text = reservation.Date;
                        timeBox.Text = reservation.Time;
                        tablesBox.Text = reservation.TableNumber;
                        nrPersonsBox.Text = reservation.nrOfSeats.ToString();
                        modifyBtn.Visibility = Visibility.Visible;
                        reservationsList.Remove(reservation);
                        if(inputList.Contains(reservation))
                            inputList.Remove(reservation);
                    }
                }
            }
            
           
        }

        
    }
}
