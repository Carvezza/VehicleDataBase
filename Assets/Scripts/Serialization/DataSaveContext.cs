using UnityEngine;
using System;

[Serializable]
public class DataSaveContext
{
    public DataSaveContext(VehicleDataBaseRecord record)
    {
        type = record.GetType().Name;
        id = record.ID;
        name = record.Name;
        iconName = record.IconName;
        mass = record.Mass;
        capacity = record.Capacity;
        maxVelocity = record.MaxVelocity;
        property = type switch
        {
            "VehicleDataBaseRecord" => string.Empty,
            "ShipDataBaseRecord" => (record as ShipDataBaseRecord).Displacement.ToString(),
            "CarDataBaseRecord" => ((int)(record as CarDataBaseRecord).Color).ToString(),
            "BikeDataBaseRecord" => Vector3IntToString((record as BikeDataBaseRecord).Size),
            "PlaneDataBaseRecord" => (record as PlaneDataBaseRecord).LiftingForce.ToString(),
            _ => string.Empty,
        };
    }
    public DataSaveContext(ShipDataBaseRecord record)
    {
        type = typeof(ShipDataBaseRecord).ToString();
        id = record.ID;
        name = record.Name;
        iconName = record.IconName;
        mass = record.Mass;
        capacity = record.Capacity;
        maxVelocity = record.MaxVelocity;
        property = record.Displacement.ToString();
    }
    public DataSaveContext(CarDataBaseRecord record)
    {
        type = typeof(CarDataBaseRecord).ToString();
        id = record.ID;
        name = record.Name;
        iconName = record.IconName;
        mass = record.Mass;
        capacity = record.Capacity;
        maxVelocity = record.MaxVelocity;
        property = record.Color.ToString();
    }
    public DataSaveContext(BikeDataBaseRecord record)
    {
        type = typeof(BikeDataBaseRecord).ToString();
        id = record.ID;
        name = record.Name;
        iconName = record.IconName;
        mass = record.Mass;
        capacity = record.Capacity;
        maxVelocity = record.MaxVelocity;
        property = Vector3IntToString(record.Size);
    }
    public DataSaveContext(PlaneDataBaseRecord record)
    {
        type = typeof(PlaneDataBaseRecord).ToString();
        id = record.ID;
        name = record.Name;
        iconName = record.IconName;
        mass = record.Mass;
        capacity = record.Capacity;
        maxVelocity = record.MaxVelocity;
        property = record.LiftingForce.ToString();
    }
    [SerializeField]
    public string type;
    [SerializeField]
    public int id;
    [SerializeField]
    public string name;
    [SerializeField]
    public string iconName;
    [SerializeField]
    public float mass;
    [SerializeField]
    public int capacity;
    [SerializeField]
    public float maxVelocity;
    [SerializeField]
    public string property;

    public static VehicleDataBaseRecord GenerateRecord(DataSaveContext context)
    {
        VehicleDataBaseRecord record = null;
        record = context.type switch
        {
            "VehicleDataBaseRecord" => new VehicleDataBaseRecord(context.id, context.name, context.iconName, context.mass, context.capacity, context.maxVelocity),
            "ShipDataBaseRecord" => new ShipDataBaseRecord(context.id, context.name, context.iconName, context.mass, context.capacity, context.maxVelocity, float.Parse(context.property)),
            "CarDataBaseRecord" => new CarDataBaseRecord(context.id, context.name, context.iconName, context.mass, context.capacity, context.maxVelocity, (ColorEnum)int.Parse(context.property)),
            "BikeDataBaseRecord" => new BikeDataBaseRecord(context.id, context.name, context.iconName, context.mass, context.capacity, context.maxVelocity, StringToVector3Int(context.property)),
            "PlaneDataBaseRecord" => new PlaneDataBaseRecord(context.id, context.name, context.iconName, context.mass, context.capacity, context.maxVelocity, float.Parse(context.property)),
            _ => null,
        };
        return record;
    }
    private static string Vector3IntToString(Vector3Int vector) => $"{vector.x}|{vector.y}|{vector.z}";
    private static Vector3Int StringToVector3Int(string str)
    {
        var numbers = str.Split('|');
        return new Vector3Int(int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2]));
    }
}
