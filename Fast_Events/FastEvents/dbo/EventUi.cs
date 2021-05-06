using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class EventUi: Interfaces.IObjectWithId
    {
        public long Id { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Organizer { get; set; }
        [DataType(DataType.DateTime), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-dd-MMThh\\:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-dd-MMThh\\:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string PictureFilename { get; set; }
        public string OwnerUuid { get; set; }
        public Category Category { get; set; }
        public int NumberTickets { get; set; }

    }
}
