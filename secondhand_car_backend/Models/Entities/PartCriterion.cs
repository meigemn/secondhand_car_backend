namespace secondhand_car_backend.Models.Entities
{
    public class PartCriterion
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsPositive { get; set; }

        //claves ajenas
        public int CarPartId { get; set; }
        public CarParts CarParts { get; set; }
    }
}
