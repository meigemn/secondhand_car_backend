namespace secondhand_car_backend.Models.Dtos.CreateDtos
{
    public class CreatePartCriterionDto
    {
        public string Description { get; set; }
        public bool IsPositive { get; set; }
        public int CarPartId { get; set; }
    }
}
