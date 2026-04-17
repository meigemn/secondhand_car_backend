using secondhand_car_backend.Models.Entities;

namespace secondhand_car_backend.Models.Dtos.EntityDtos
{
    public class CarPartDto
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public string Category { get; set; }
        //Lista de dtos
        public List<PartCriterionDto> Criteria { get; set; } = new();
    }
}
