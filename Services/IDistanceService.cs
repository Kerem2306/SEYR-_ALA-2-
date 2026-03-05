namespace SEYRİ_ALA.Services
{
    public interface IDistanceService
    {
        // İki koordinat arasındaki mesafeyi (KM) hesaplar
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}