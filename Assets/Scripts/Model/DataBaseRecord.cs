using System;

public class VehicleDataBaseRecord
{
    public VehicleDataBaseRecord(int id, string name, string iconName, float mass, int capacity, float maxVelocity)
    {
        ID = id;
        Name = name;
        Mass = mass;
        Capacity = capacity;
        MaxVelocity = maxVelocity;
        IconName = iconName;
    }
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string IconName { get; private set; }
    public float Mass { get; private set; }
    public int Capacity { get; private set; }
    public float MaxVelocity { get; private set; }
}
