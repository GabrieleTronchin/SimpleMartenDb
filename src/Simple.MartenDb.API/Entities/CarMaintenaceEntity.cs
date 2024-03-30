using Marten.Schema;

namespace Simple.MartenDb.API.Entities;

public sealed class CarMaintenaceEntity
{
    [Identity]
    public Guid CarId { get; set; }

    public List<MaintenaceEntity> MaintainacePlanTodo { get; set; } = new List<MaintenaceEntity>();
}

public sealed class MaintenaceEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Checked { get; set; }

}
