namespace secondhand_car_backend.Models.Dtos.RequestDtos
{
    public class LoginRequestDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? TwoFactorCode { get; set; }
        public string? TwoFactorRecoveryCode { get; set; }
    }
}
