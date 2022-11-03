using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestaurantReservations
{
    //created an interface and a class where are defined all the booking/reservation properties
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
        [JsonInclude]
        public string Name { get; set; }
        [JsonInclude]
        public string Date { get; set; }
      
        [JsonInclude]
        public string Time { get; set; }
        [JsonInclude]
        public string TableNumber { get; set; }
        [JsonInclude]
        public int nrOfSeats { get; set; }

        [JsonConstructor]        
        public Reservation(string name,string date, string time, string tableNumber, int NrOfSeats)
        {
            Name = name;
            Date = date;
            Time = time;           
            TableNumber = tableNumber;
            nrOfSeats = NrOfSeats;
        }
        public  string Print()
        {
            return  Name + "'s reservation on " + Date + " at " + Time;
        }

        public override string ToString()
        {
            return Name + " " + Date + " " + Time + " " +  TableNumber + " " + nrOfSeats;
        }

    }
}
