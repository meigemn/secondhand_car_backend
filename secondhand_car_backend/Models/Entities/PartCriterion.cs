using System.ComponentModel.DataAnnotations;

namespace secondhand_car_backend.Models.Entities
{
    public class PartCriterion
    {   [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsPositive { get; set; }

        //FK
        [Required]
        public int CarPartId { get; set; }
        public CarPart CarPart { get; set; }
    }
}
