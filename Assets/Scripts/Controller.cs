using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Link between data and view
public class Controller : MonoBehaviour
{
    [SerializeField]
    private VehicleDataBaseStorage _vehicleDataBase;
    [SerializeField]
    private ViewRenderer _viewRenderer;
    [SerializeField]
    private DataSerializer _serializer;
    public event Action<List<DataViewContext>> DataChanged;

    private DataViewContext GenerateDataViewContext(VehicleDataBaseRecord record)
    {
        if (record is ShipDataBaseRecord)
        {
            return GenerateShipDataViewContext(record as ShipDataBaseRecord);
        }
        else if (record is CarDataBaseRecord)
        {
            return GenerateCarDataViewContext(record as CarDataBaseRecord);
        }
        else if (record is BikeDataBaseRecord)
        {
            return GenerateBikeDataViewContext(record as BikeDataBaseRecord);
        }
        else if (record is PlaneDataBaseRecord)
        {
            return GeneratePlaneDataViewContext(record as PlaneDataBaseRecord);
        }
        else throw new ArgumentException("");
    }
    private DataViewContext GenerateShipDataViewContext(ShipDataBaseRecord record)
    {
        return new DataViewContext(record.Name, 
                                   record.IconName,
                                   ("Масса", record.Mass.ToString()),
                                   ("Вместительность", record.Capacity.ToString()),
                                   ("Максимальная скорость", record.MaxVelocity.ToString()),
                                   ("Водоизмещение", record.Displacement.ToString()));
    }
    private DataViewContext GenerateCarDataViewContext(CarDataBaseRecord record)
    {
        return new DataViewContext(record.Name,
                                   record.IconName,
                                   ("Масса", record.Mass.ToString()),
                                   ("Вместительность", record.Capacity.ToString()),
                                   ("Максимальная скорость", record.MaxVelocity.ToString()),
                                   ("Цвет", record.Color.ToString()));
    }
    private DataViewContext GenerateBikeDataViewContext(BikeDataBaseRecord record)
    {
        return new DataViewContext(record.Name,
                                   record.IconName,
                                   ("Масса", record.Mass.ToString()),
                                   ("Вместительность", record.Capacity.ToString()),
                                   ("Максимальная скорость", record.MaxVelocity.ToString()),
                                   ("Габариты", $"{record.Size.x}x{record.Size.y}x{record.Size.z}"));
    }
    private DataViewContext GeneratePlaneDataViewContext(PlaneDataBaseRecord record)
    {
        return new DataViewContext(record.Name,
                                   record.IconName,
                                   ("Масса", record.Mass.ToString()),
                                   ("Вместительность", record.Capacity.ToString()),
                                   ("Максимальная скорость", record.MaxVelocity.ToString()),
                                   ("Подъёмная сила", record.LiftingForce.ToString()));
    }

    private List<DataViewContext> QuerryAll()
    {
        List<DataViewContext> dataViewContexts = new List<DataViewContext>();
        foreach (VehicleDataBaseRecord record in _vehicleDataBase.GetAll())
        {
            DataViewContext context = GenerateDataViewContext(record);
            dataViewContexts.Add(context);
        }
        return dataViewContexts;
    }
    public void QuerryData()
    {
        StartCoroutine(LoadData());
    }
    public void OnDataChanged(VehicleDataBaseStorage dataBaseStorage)
    {
        List<DataViewContext> dataViewContexts = new List<DataViewContext>();
        foreach (VehicleDataBaseRecord record in _vehicleDataBase.GetAll())
        {
            DataViewContext context = GenerateDataViewContext(record);
            dataViewContexts.Add(context);
        }
        DataChanged?.Invoke(dataViewContexts);
    }
    private IEnumerator LoadData()
    {
        _vehicleDataBase = null;
        _serializer.DataLoaded += (t) => _vehicleDataBase = t;
        _serializer.RequestData();
        while (_vehicleDataBase == null)
        {
            yield return null;
        }
        OnDataChanged(_vehicleDataBase);
    }
}
