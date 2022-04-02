using UnityEngine;

public class BikeDataBaseRecord : VehicleDataBaseRecord
{
    public BikeDataBaseRecord(int id, string name, string iconName, float mass, int capacity, float maxVelocity, Vector3Int size) : base(id, name, iconName, mass, capacity, maxVelocity)
    {
        Size = size;
    }
    public Vector3Int Size { get; set; }
}
