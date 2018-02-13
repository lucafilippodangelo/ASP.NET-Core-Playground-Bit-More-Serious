using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        //LD STEP28
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //LD identity option because I want that a new "Id" for the "Key" is generated once just when created
        public int Id { get; set; } //LD authomatic recorded as a primary key by convention

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
               = new List<PointOfInterest>();
    }
}
