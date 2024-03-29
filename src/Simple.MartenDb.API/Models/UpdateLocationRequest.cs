namespace Simple.MartenDb.API.Models
{
    public record UpdateLocationRequest
    {
        public required int Latitute { get; init; }
        public required int Longitude { get; init; }

    }
}
