using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Models
{
    public class Book : EntityId<int>
    {

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;



        public override string ToString()
        {
            
            return $"\n Id: {Id}, Title: {Title}, Author: {Author}, PublicationDate: {PublicationDate:yyyy-MM-dd}, ISBN: {ISBN}, Genre: {Genre}, Summary: {Summary}";

       
        }


    }
}