using PromoCodes.Models;

namespace PromoCodes.Clients;

public interface IDiscountClient
{
    Task CodesGenerated(GenerateResponse result);
    Task CodeUsed(UseCodeResponse result);
}