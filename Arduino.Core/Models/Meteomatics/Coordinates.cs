namespace Arduino.Core.Models.Meteomatics
{
    internal class Coordinates
    {
        public double Lat {get;set;}
        public double Lon { get; set; }
        public List<Temperature> Dates { get; set; } = new();
    }
}
