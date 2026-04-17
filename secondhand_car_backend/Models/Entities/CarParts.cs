using System.ComponentModel.DataAnnotations;

namespace secondhand_car_backend.Models.Entities
{
    public class CarParts
    {
        [Key]
        public int Id { get; set; }
        public string PartName { get; set; }
        public string Category { get; set; }

        public List<PartCriterion> Criteria { get; set; } = new();
    }
}
