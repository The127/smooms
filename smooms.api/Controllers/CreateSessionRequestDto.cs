namespace smooms.api.Controllers;

public class CreateSessionRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}