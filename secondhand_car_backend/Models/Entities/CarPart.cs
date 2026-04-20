using secondhand_car_backend.Models.Dtos.EntityDtos;
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

        public List<PartCriterionDto> Criteria { get; set; } = new();
    }
}
