using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public Reservation(string Name,string Date, string Time, string TableNumber, int nrOfSeats)
        {
            this.Name = Name;
            this.Date = Date;
            this.Time = Time;           
            this.TableNumber = TableNumber;
            this.nrOfSeats = nrOfSeats;
        }
        public override string ToString()
        {
            return  Name + "'s reservation on " + Date + " at " + Time;
        }



    }
}
