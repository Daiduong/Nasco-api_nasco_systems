using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Data
{
    [Table("LadingHistory")]
    public partial class LadingHistory
    {
        public long Id { get; set; }
        public Nullable<long> LadingId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<int> PostOfficeId { get; set; }
        public Nullable<int> OfficerId { get; set; }
        public string Mobile { get; set; }
        public string Note { get; set; }
        public Nullable<int> TypeReason { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lng { get; set; }
        public string Location { get; set; }
        [ForeignKey("Status"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Status CurrenSttStatus { get; set; }
        [ForeignKey("PostOfficeId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual PostOffice PostOffice { get; set; }
        [ForeignKey("OfficerId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Officer Officer { get; set; }
        [ForeignKey("TypeReason"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual TypeReason CurrentTypeReason { get; set; }
        [ForeignKey("LadingId"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual Lading Lading { get; set; }
    }
}
