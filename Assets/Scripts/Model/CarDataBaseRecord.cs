public class CarDataBaseRecord : VehicleDataBaseRecord
{
    public CarDataBaseRecord(int id, string name, string iconName, float mass, int capacity, float maxVelocity, ColorEnum color) : base(id, name, iconName, mass, capacity, maxVelocity)
    {
        Color = color;
    }
    public ColorEnum Color { get; private set; }
}
