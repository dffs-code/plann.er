﻿namespace Journey.Communication.Responses;
public class ResponseShortTripJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string FullAddress { get; set; }
    public string UserId { get; set; }
    public ResponseShortUserJson User { get; set; }
}
