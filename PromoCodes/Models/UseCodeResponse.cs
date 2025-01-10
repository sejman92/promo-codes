namespace PromoCodes.Models;

public class UseCodeResponse
{
    public byte Result { get; set; }
}

public enum UseCodeResponseType
{
    Success = 0,
    InvalidRequest = 1,
    CodeNotFound = 2,   
}