using SpendWiseAPI.Models;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
}
