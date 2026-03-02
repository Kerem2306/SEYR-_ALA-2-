using SEYRİ_ALA.Models;

public class Comment
{
    public int Id { get; set; }

    // Şehir ile ilişki (Yeni ekledik)
    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public int? RouteId { get; set; } // Opsiyonel yaptık
    public TravelRoute? Route { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = true;
}