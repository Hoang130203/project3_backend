
namespace PostService.Models
{
    public class Location
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GeoJsonPoint Coordinates { get; set; }

        internal void SetCoordinates()
        {
            throw new NotImplementedException();
        }
    }

    public class GeoJsonPoint
    {
        public string Type { get; } = "Point";
        public double[] Coordinates { get; set; }

        public GeoJsonPoint(double longitude, double latitude)
        {
            Coordinates = new[] { longitude, latitude };
        }
    }
}
