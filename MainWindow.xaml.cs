
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

namespace RestaurantReservations
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        

        public MainWindow()
        {
            InitializeComponent();
            CheckIfExists();
           
        }

        private void CheckIfExists()
        {
            if (!File.Exists("ReservationsDatabase.json"))
            {
                MessageBox.Show("First use of the app! Please select the ReservationDatabase file!", "Select File", MessageBoxButton.OK, MessageBoxImage.Information);
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "";
                dlg.DefaultExt = ".json";
                dlg.Filter = "Json file (.json)|*.json";
                dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string fileName = dlg.FileName;
                    string path = dlg.InitialDirectory + "ReservationsDatabase.json";
                    System.IO.File.Copy(fileName, path, true);

                }
                
            }
        }
        private void reserv_Click(object sender, RoutedEventArgs e)
        {
            MainW.NavigationService.Navigate(new BookingPage());
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            string msg = "Don't forget to save all reservations to ReservationsDatabase!\nAll Progress will be LOST!\n\nClose anyway?";
            MessageBoxResult result = MessageBox.Show(msg, "Restaurant Table Reservation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }




        }
    }
}
