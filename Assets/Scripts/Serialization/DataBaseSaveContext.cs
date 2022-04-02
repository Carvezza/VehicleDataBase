using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DataBaseSaveContext
{
    public DataBaseSaveContext(VehicleDataBaseStorage dataBase)
    {
        _ids = dataBase.GetIDs();
        _contexts = new List<DataSaveContext>();
        foreach (VehicleDataBaseRecord record in dataBase.GetAll())
        {
            _contexts.Add(new DataSaveContext(record));
        }
    }
    [SerializeField]
    private List<int> _ids;
    [SerializeField]
    private List<DataSaveContext> _contexts;
    public List<int> IDs => _ids;
    public List<DataSaveContext> Contexts => _contexts;

    public static VehicleDataBaseStorage GenerateDataBase(DataBaseSaveContext context)
    {
        List<int> ids = new List<int>();
        List<VehicleDataBaseRecord> records = new List<VehicleDataBaseRecord>();
        foreach (int id in context.IDs)
        {
            ids.Add(id);
        }
        foreach (DataSaveContext contx in context.Contexts)
        {
            records.Add(DataSaveContext.GenerateRecord(contx));
        }
        return new VehicleDataBaseStorage(ids, records);
    }
}