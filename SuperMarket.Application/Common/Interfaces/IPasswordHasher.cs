
namespace SuperMarket.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string plainPassword);
    bool VerifyHashedPassword(string plainPassword, string hashedPassword);
}
