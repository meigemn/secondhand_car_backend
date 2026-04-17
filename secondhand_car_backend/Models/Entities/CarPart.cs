using System.ComponentModel.DataAnnotations;

namespace secondhand_car_backend.Models.Entities
{
    public class CarPart
    {
        [Key]
        public int Id { get; set; }
        public string PartName { get; set; }
        public string Category { get; set; }

        public List<PartCriterion> Criteria { get; set; } = new();
    }
}
