namespace smooms.app.Models;

public record DeviceId : EntityIdBase<DeviceId>;

public class Device : EntityBase<DeviceId>
{
    public required string Name { get; set; }
    
    // this is the owned side of the relationship
    // this is the id (automatically detected by the name: it ends with Id and has the same Name as the navigation property)
    public required AccountId AccountId { get; set; }
    // this is the navigation property, you can use it in queries to get the related Account
    public Account Account { get; set; } = null!;
}

public class DeviceConfiguration : EntityBaseConfiguration<Device, DeviceId>
{
    
}