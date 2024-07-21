namespace Journey.Infrastructure.Entities;
public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public IList<Activity> Activities { get; set; } = [];
    public User User { get; set; }
}
