using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Models
{
    public class ExpectedTimeModel
    {
        public int Id { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public TimeSpan TimeTakeOff { get; set; }
        public int IntervalDayStart { get; set; }
        public int IntervalDayEnd { get; set; }
        public int IntervalDayTakeOff { get; set; }
        public DateTime DateTimeStart
        {
            get
            {
                return DateTime.Now.Date.AddDays(IntervalDayStart).Add(TimeStart);
            }
        }
        public DateTime DateTimeEnd
        {
            get
            {
                return DateTime.Now.Date.AddDays(IntervalDayEnd).Add(TimeEnd);
            }
        }
        public DateTime DateTimeTakeOff
        {
            get
            {
                return DateTime.Now.Date.AddDays(IntervalDayTakeOff).Add(TimeTakeOff);
            }
        }
        public ExpectedTimeModel()
        {
        }
    }
}
