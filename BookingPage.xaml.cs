
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using Path = System.IO.Path;

namespace RestaurantReservations
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {

        private readonly string[] tableSeats = { "1", "2", "3", "4", "5" };  //items to be displyed into table seats number combobox
        private readonly string[] tableNames = { "Table 1", "Table 2", "Table 3", "Table 4", "Table 5", "Table 6", "Table 7", "Table 8", "Table 9" }; //items to be displayed into Table number combobox
        private readonly string[] Rtimes = { "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00" };  //items to be displyed into time combobox
        string? getName, getTableNr, getDate, getTime;  // variabels to read input from all comboBoxes
        int getSeatsNr;
       
        public List<Reservation> reservationsList = new List<Reservation>();  // all reserservation list, saved into ReservationsDatabase.json
        List<Reservation> inputList = new List<Reservation>();  // another list that saves only user input reservations, used only as long program runs 
        
      
        public  BookingPage()
        {
            InitializeComponent();
            nrPersonsBox.ItemsSource = tableSeats;
            tablesBox.ItemsSource = tableNames;
            timeBox.ItemsSource = Rtimes;

            Task task = ReadFromFile();

            
        }
        
        // Save Reservation button
        private void saveReservation_Click(object sender, RoutedEventArgs e)
        {
            SaveReservations(); 
        }
        //Show all Reservations button
        private void showReservations_Click(object sender, RoutedEventArgs e)
        {
             
            UpdateReservationsList();
            DisplayContent(reservationsList);           
            
        }
        // Cancel Reservation button
        private void cancelReservation_Click(object sender, RoutedEventArgs e)
        {
            DeleteReservation();

        }
        // Modify Reservation button
        private void modifyReservation_Click(object sender, RoutedEventArgs e)
        {
            ModifyReservation();
        }
        //Save to File (different then ReservationsDatabase.json) button
        private void saveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "";
                dlg.DefaultExt = ".txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string fileName = dlg.FileName;
                    using (StreamWriter sw = new StreamWriter($"{fileName}"))
                    {
                        foreach (Reservation reservation in reservationsList)
                            sw.WriteLine(reservation.ToString());
                    }
                    MessageBox.Show($"Saved as {fileName}");
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Error saving to file!" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        }



        // method to update the dataGrid output
        private void DisplayContent(List<Reservation> lista)
        {              
            printReservations.ItemsSource = lista;
        }
        //method that saves the reservation if all conditions are met
        private async void SaveReservations()
        {
            
            // read input content
            if (!string.IsNullOrEmpty(nameBox.Text) || !string.IsNullOrWhiteSpace(nameBox.Text) && Regex.IsMatch(nameBox.Text, @"^[a-zA-Z0-9]+$"))
            {
                getName = nameBox.Text.First().ToString().ToUpper() + nameBox.Text.Substring(1);
                if (datePick.SelectedDate != null)
                {
                    getDate = datePick.Text;
                    if (timeBox.SelectedItem != null)
                    {
                        getTime = timeBox.Text;
                        if (tablesBox.SelectedItem != null)
                        {
                            getTableNr = tablesBox.Text;
                            if (tablesBox.SelectedItem != null)
                            {
                                getTableNr = tablesBox.Text;
                                if (nrPersonsBox.SelectedItem != null)
                                {
                                    getSeatsNr = int.Parse(nrPersonsBox.Text);

                                    //create a new reservation 
                                    Reservation newItem = new Reservation(getName, getDate, getTime, getTableNr, getSeatsNr);
                                    // if all conditions have been met then the new reservation is saved into both input and reservations lists and then to ReservationsDatabase.json
                                    // otherwise you get a message that describes what went wrong
                                    if (CheckReservation(newItem) == false)
                                    {
                                        MessageBox.Show("Table is already booked! Try again!", " Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        tablesBox.Text = "";
                                    }
                                        
                                    else
                                    {
                                        reservationsList.Add(newItem);
                                        inputList.Add(newItem);
                                        printReservations.ItemsSource = null;
                                        DisplayContent(inputList);
                                        printReservations.Items.Refresh();
                                        await WriteToFile(reservationsList);
                                        MessageBox.Show("Your reservation has been saved!", "Saving Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                                        ClearFields();
                                    }
                                    
                                }
                                    
                                else
                                    MessageBox.Show("Number persons field Empty!");
                            }                               
                            else
                                MessageBox.Show("Table number field Empty!");
                        }                            
                        else
                            MessageBox.Show("Table number field Empty!");
                    }                        
                    else
                        MessageBox.Show("Time field Empty!");
                }                    
                else
                    MessageBox.Show("Date field Empty!");
            }
            else
                MessageBox.Show("Name field Empty!");           
            
            

            
            
        }
        // method that sorts the reservation list by date and time, removes automatically older reservations then current date and duplicates
        private void UpdateReservationsList()
        {
            
            reservationsList = reservationsList.OrderBy(x => x.Date).ThenBy(x => x.Time).ToList();
            foreach (Reservation reservation in reservationsList)
                if (DateTime.Parse(reservation.Date) < DateTime.Today)
                    MessageBox.Show($"{reservation.Print()} has been deleted because its passed due!", "Delete old Reservations", MessageBoxButton.OK, MessageBoxImage.Information);
            reservationsList.RemoveAll(x => DateTime.Parse(x.Date) < DateTime.Today);
            List<Reservation> reservations = new List<Reservation>();
            reservations = reservationsList;
            reservationsList = reservations.Distinct().ToList();
            
            


        }
        //method that clears all text input and resets comboBoxes
        private void ClearFields()
        {
            nameBox.Clear();
            datePick.Text = "";
            timeBox.Text = "";
            tablesBox.Text = "";
            nrPersonsBox.Text = "";

        }
        //method that reads all reservations from ReservationsDatabase.json
        //at first use of the app the reservationsList will be populated with some given reservations, the json file will be created afterwards and all be written in it
        //after the first add/delete/modify of a reservation
        //after first use of the app the program will always read from file an repopulate the reservationsList 
        private async Task ReadFromFile()
        {
            string fileName = "ReservationsDatabase.json";
            if (!File.Exists(fileName))
            {
                Reservation reservation  = new Reservation("Name1", "2022-11-25", "12:00", "Table 3", 4);               
                Reservation reservation1 = new Reservation("Name2", "2022-11-08", "13:00", "Table 5", 1);
                Reservation reservation2 = new Reservation("Name3", "2022-12-08", "18:00", "Table 3", 3);
                Reservation reservation3 = new Reservation("Name4", "2022-11-18", "20:00", "Table 6", 2);
                Reservation reservation4 = new Reservation("Name5", "2022-11-28", "17:00", "Table 1", 5);
                Reservation reservation5 = new Reservation("Name6", "2022-11-21", "19:00", "Table 4", 2);
                reservationsList.Add(reservation);
                reservationsList.Add(reservation1);
                reservationsList.Add(reservation2);
                reservationsList.Add(reservation3);
                reservationsList.Add(reservation4);
                reservationsList.Add(reservation5);

            }
            else 
            {
                try
                {
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
                catch (Exception ex) { MessageBox.Show("Error reading from json File" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
        //method that writes into ReservationsDatabase.json the reservationsList after each new,canceled or modified reservation
        private async Task WriteToFile(List<Reservation> lista)
        {
            try
            {
                string fileName = "ReservationsDatabase.json";
                using FileStream createStream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(createStream, lista);
                await createStream.DisposeAsync();
            }
            catch (Exception ex) { MessageBox.Show("Error writting to Json File" + ex,"Error",MessageBoxButton.OK,MessageBoxImage.Error); }
           
        }
        

   
        //after pressing the modify button a new button will show that by pressing it will save the reservation after modifying it
        private void modifyBtn_Click(object sender, RoutedEventArgs e)
        {
            
            SaveReservations();
            printReservations.ItemsSource = null;
            UpdateReservationsList();
            DisplayContent(reservationsList);
            printReservations.Items.Refresh();
            
            modifyBtn.Visibility = Visibility.Hidden;
            
        }
        //method that reads the selected (from the dataGrid) reservation and then display its content into the input fields, then removes the reservation
        //to make place for the new modified one
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
                        if (inputList.Contains(reservation))
                            inputList.Remove(reservation);
                    }
                }
            }


        }


        //method that checks if there are other reservations on same Date at same time and at same table and if there are
        // then it checks if seats number is greater then 5 (max seats number for a table) and returns a message or if
        // the number is less or equal to 5 it ask user if to continue with saving the reservation
        private bool CheckReservation(Reservation reservation)
        {
            bool result = true;
            int counter = 0;
            MessageBoxResult msgResult = new MessageBoxResult();
            int index = Array.IndexOf(Rtimes, reservation.Time);
            string previousTime = Rtimes[index-1];
            string nextTime = Rtimes[index+1];
            var nrPersons = reservationsList.Where(x => string.Equals(x.Date, reservation.Date) && 
                                            (string.Equals(x.Time, reservation.Time) || string.Equals(x.Time,previousTime) || string.Equals(x.Time,nextTime))
                                            && string.Equals(x.TableNumber, reservation.TableNumber)).Select(x => x.nrOfSeats);
            foreach (var person in nrPersons)
                counter += person;
            if (counter == 0)
                result = true;
            else
            {
                if ((counter + reservation.nrOfSeats) > 5)
                    return false;
                else if ((counter + reservation.nrOfSeats) > 0 && (counter + reservation.nrOfSeats) <= 5)
                {
                    msgResult = MessageBox.Show($"{reservation.TableNumber} has already {counter} seats reserved. Do you still want to reserv this table?",
                                            "Save Reservation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (msgResult == MessageBoxResult.Yes)
                        result = true;
                    else
                        result = false;
                }
            }
            return result;

        }
        //method that removes the selected (from the dataGrid) reservation from the reservationList and file 
        private async void DeleteReservation()
        {
            if (printReservations.SelectedItem == null)
                MessageBox.Show("You need to first select a reservation from the list to delete it!", "Delete Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                Reservation? reservation = printReservations.SelectedItem as Reservation;
                if (reservation != null)
                {
                    reservationsList.Remove(reservation);
                    if (inputList.Contains(reservation))
                        inputList.Remove(reservation);
                    printReservations.ItemsSource= null;
                    UpdateReservationsList();
                    DisplayContent(reservationsList);
                    printReservations.Items.Refresh();
                    await WriteToFile(reservationsList);
                    MessageBox.Show("Your selected reservation has been canceled!", "Delete Reservation", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }

        

        
    }
}
