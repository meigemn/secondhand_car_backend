namespace secondhand_car_backend.Models.Dtos.EntityDtos
{
    public class UserDto
    {

        public string Id { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public string? Email { get; set; }
    }
}
