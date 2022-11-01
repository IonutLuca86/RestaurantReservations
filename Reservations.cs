using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservations
{
    public interface IBooking
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Name { get; set; }
        public string TableNumber { get; set; }
        public int nrOfSeats { get; set; }

    }

   

    public class Reservation : IBooking
    {
        
        public string Time { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string TableNumber { get; set; }
        public int nrOfSeats { get; set; }


        public Reservation(string name,string date, string time, string tableNumber, int seatsNumber)
        {
            Date = date;
            Time = time;
            Name = name;
            TableNumber = tableNumber;
            nrOfSeats = seatsNumber;
        }
        //public override string ToString()
        //{            
        //    return Name + " reserved " + Table.ToString() + " on " + Date + " at " + Time;
        //}
        


    }
}
