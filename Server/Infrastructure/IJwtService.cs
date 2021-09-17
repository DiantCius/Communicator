namespace Server.Infrastructure
{
    public interface IJwtService
    {
        string GenerateToken(string username);
    }
}