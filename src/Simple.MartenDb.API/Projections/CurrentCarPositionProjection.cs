using Marten.Events;
using Marten.Events.Projections;
using Simple.MartenDb.API.Models;

namespace Simple.MartenDb.API.Projections;



public class CurrentCarPositionProjection : EventProjection
{
    public CurrentCarPositionProjection()
    {
        ProjectionName = "CarActualLocation";
    }


    public CurrentCarPosition Create(UpdateLocationRequest lastLocation, IEvent e)
    {
        return new CurrentCarPosition(e.Id, new Location(lastLocation.Longitude, lastLocation.Latitute));
    }
}
