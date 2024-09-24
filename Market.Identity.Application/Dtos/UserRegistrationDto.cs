namespace Market.Identity.Application.Dtos;

public class UserRegistrationDto
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string CallSign { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}