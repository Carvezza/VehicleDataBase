using System;

public interface IDataManager
{
    void RequestData();
    void SaveData(VehicleDataBaseStorage storage);
    event Action<VehicleDataBaseStorage> DataLoaded;
}
