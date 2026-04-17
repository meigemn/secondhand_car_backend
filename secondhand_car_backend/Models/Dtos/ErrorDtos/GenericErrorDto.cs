using secondhand_car_backend.Utils;

namespace secondhand_car_backend.Models.Dtos.ErrorDtos
{
    public class GenericErrorDto
    {
        public ResponseCodes Id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public GenericErrorDto()
        {
            // Inicializa a Ok (0) para indicar que no hay error
            Id = ResponseCodes.Ok;
            Location = string.Empty;
            Description = string.Empty;
        }

    }
}
