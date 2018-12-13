using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class DistanceMatrixResult
    {
       public DistanceMatrixElement[] Elements { get; set; }
    }
    public class DistanceMatrixElement
    {
        public string Status { get; set; }
        public ValueText Duration { get; set; }
        public ValueText Distance { get; set; }
    }
    public class ValueText
    {
        public double Value { get; set; }
        public string Text { get; set; }
    }
}
