using System.ComponentModel.DataAnnotations;

namespace secondhand_car_backend.Models.Entities
{
    public class CarPart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PartName { get; set; }
        [Required]
        public string Category { get; set; }

        public List<PartCriterion> Criteria { get; set; } = new();
    }
}
