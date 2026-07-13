using System;

namespace ecommerce.api;

public class ApiErrorResponse
{
    public int StatusCode {get; set;}
    public string Message {get; set;}
    public string? Details {get; set;}

}
