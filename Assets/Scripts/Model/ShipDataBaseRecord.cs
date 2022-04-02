public class ShipDataBaseRecord : VehicleDataBaseRecord
{
    public ShipDataBaseRecord(int id, string name, string iconName, float mass, int capacity, float maxVelocity, float displacement) : base(id, name, iconName, mass, capacity, maxVelocity)
    {
        Displacement = displacement;
    }
    public float Displacement { get; set; }
}
