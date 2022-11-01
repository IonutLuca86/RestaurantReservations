
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public List<Reservation> reservationsList = new List<Reservation>()
        {
        new Reservation("Ionut","2022-11-25","18:00","Table3",4),
        new Reservation("Ionut2","2022-11-25","12:00","Table3",4),
        new Reservation("Catalin","2022-11-28","20:00","Table5",2),
        new Reservation("Luca","2022-12-03","19:00", "Table1",5),
        new Reservation("Cristina","2022-12-25","17:00","Table2",1),
        new Reservation("Anna","2022-10-27","18:00","Table5",5)
        };

        MessageBoxResult result;
        public BookingPage()
        {
            InitializeComponent();
            nrPersonsBox.ItemsSource = tableSeats;
            tablesBox.ItemsSource = tableNames;
            timeBox.ItemsSource = Rtimes;
           
        }

        private void saveReservation_Click(object sender, RoutedEventArgs e)
        {
            SaveReservations(); 
        }

        private void showReservations_Click(object sender, RoutedEventArgs e)
        {
             DisplayContent();
        }

        private void cancelReservation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveToFile_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DisplayContent()
        {
            reservationsList = reservationsList.OrderBy(x => x.Date).ThenBy(x => x.Time).ToList();
            foreach (Reservation reservation in reservationsList)
                if (DateTime.Parse(reservation.Date) < DateTime.Today)
                    result = MessageBox.Show($"{reservation.ToString()} has been deleted because its passed due!", "Delete old Reservations", MessageBoxButton.OK, MessageBoxImage.Information);
            reservationsList.RemoveAll(x => DateTime.Parse(x.Date) < DateTime.Today);
            printReservations.ItemsSource = reservationsList;
        }

        private void SaveReservations()
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
                    result = MessageBox.Show("Table is already booked! Try again!", " Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    reservationsList.Add(newItem);
                    printReservations.Items.Add(newItem);
                    result = MessageBox.Show("Your reservation has been saved!", "Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
                result = MessageBox.Show("Empty or invalid field! Try Again!", "Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Error);
            ClearFields();
        }
        

        private void ClearFields()
        {
            nameBox.Clear();
            datePick.Text = "";
            timeBox.Text = "";
            tablesBox.Text = "";
            nrPersonsBox.Text = "";

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
        
    }
}
