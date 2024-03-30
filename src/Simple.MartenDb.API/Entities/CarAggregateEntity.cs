using Simple.MartenDb.API.Models;

namespace Simple.MartenDb.API.Entities;

public class CarAggregateEntity
{

    public Guid Id { get; set; }

    public Location? InitialPosition { get; set; }

    public Location CurrentPosition { get; set; }

    public int Traveled { get; set; }

    public void Apply(UpdateLocationRequest e)
    {
        if (InitialPosition == null)
            InitialPosition = new Location(e.Latitute, e.Longitude);

        CurrentPosition = new Location(e.Latitute, e.Longitude);
        Traveled += e.Longitude; //just a sample traveled calculation
    }
}
