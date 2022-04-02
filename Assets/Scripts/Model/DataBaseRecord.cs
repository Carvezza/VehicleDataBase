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
    public int ID { get; set; }
    public string Name { get; set; }
    public string IconName { get; set; }
    public float Mass { get; set; }
    public int Capacity { get; set; }
    public float MaxVelocity { get; set; }
}
