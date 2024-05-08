namespace Simple.MartenDb.API.Entities;

public class CarEntity
{
    public Guid Id { get; set; }

    public Location? InitialPosition { get; set; }

    public Location CurrentPosition { get; set; }

    public int Traveled { get; set; }
}
