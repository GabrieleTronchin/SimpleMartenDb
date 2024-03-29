namespace EventSource.Presentation.Rider.Events
{
    public record UpdateLocationRequest
    {
        public required int Latitute { get; init; }
        public required int Longitude { get; init; }

    }
}
