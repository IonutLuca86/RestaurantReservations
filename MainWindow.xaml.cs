﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
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