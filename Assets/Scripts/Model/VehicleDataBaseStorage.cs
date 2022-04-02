using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class VehicleDataBaseStorage
{
    [SerializeField]
    private List<int> _ids;
    [SerializeField]
    [SerializeReference]
    private List<VehicleDataBaseRecord> _vehicles;

    public VehicleDataBaseStorage(List<int> ids, List<VehicleDataBaseRecord> vehicles)
    {
        _ids = ids;
        _vehicles = vehicles;
    }
    public void Add(int id, VehicleDataBaseRecord record)
    {
        if (!_ids.Contains(id))
        {
            _ids.Add(id);
            _vehicles.Add(record);
        }
        else
        {
            throw new ArgumentException("");
        }
    }
    public void Delete(int id)
    {
        if (_ids.Contains(id))
        {
            int idIndex = _ids.IndexOf(id);
            _ids.RemoveAt(idIndex);
            _vehicles.RemoveAt(idIndex);
        }
        else
        {
            throw new ArgumentException("");
        }
    }
    public VehicleDataBaseRecord Get(int id)
    {
        if (_ids.Contains(id))
        {
            int idIndex = _ids.IndexOf(id);
            return _vehicles[idIndex];
        }
        else
        {
            throw new ArgumentException("");
        }
    }
    public void UpdateRecord(int id, VehicleDataBaseRecord record)
    {
        if (_ids.Contains(id))
        {
            int idIndex = _ids.IndexOf(id);
            _vehicles[idIndex] = record;
        }
        else
        {
            throw new ArgumentException("");
        }
    }
    public IEnumerable<VehicleDataBaseRecord> GetMany(params int[] ids)
    {
        if (ids.Any(t => !_ids.Contains(t)))
        {
            throw new ArgumentException("");
        }
        foreach (int id in ids)
        {
            int idIndex = _ids.IndexOf(id);
            yield return _vehicles[idIndex];
        }
    }
    public void DeleteMany(params int[] ids)
    {
        if (ids.Any(t => !_ids.Contains(t)))
        {
            throw new ArgumentException("");
        }
        foreach (int id in ids)
        {
            int idIndex = _ids.IndexOf(id);
            _ids.RemoveAt(idIndex);
            _vehicles.RemoveAt(idIndex);
        }
    }
    public List<int> GetIDs() => _ids;
    public IEnumerable<VehicleDataBaseRecord> GetAll() => _vehicles;
    public void Clear()
    {
        _ids.Clear();
        _vehicles.Clear();
    }
}
