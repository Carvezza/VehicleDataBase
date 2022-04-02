public class PlaneDataBaseRecord : VehicleDataBaseRecord
{
    public PlaneDataBaseRecord(int id, string name, string iconName, float mass, int capacity, float maxVelocity, float liftingForce) : base(id, name, iconName, mass, capacity, maxVelocity)
    {
        LiftingForce = liftingForce;
    }
    public float LiftingForce { get; set; }
}
