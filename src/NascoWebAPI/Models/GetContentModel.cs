using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class GetContentModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public bool IsPush { get; set; }
        public bool IsEnabled { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
