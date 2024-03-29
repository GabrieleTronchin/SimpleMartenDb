namespace Simple.MartenDb.API.Models
{
    public class CarMaintenanceEvent
    {
        public int Id { get; set; }
        public Guid CarId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }
    }
}
