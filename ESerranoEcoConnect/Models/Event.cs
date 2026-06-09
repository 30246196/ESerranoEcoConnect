using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models
{
    public class Event
    {
        
            public int EventId { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public DateTime EventDate { get; set; }

            public DateTime CreatedAt { get; set; }
        
    }


}