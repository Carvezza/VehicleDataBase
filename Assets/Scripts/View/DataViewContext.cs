using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataViewContext
{
    public DataViewContext(string name, string iconName, params (string, string)[] stats)
    {
        Name = name;
        IconName = iconName;
        Stats = stats;
    }

    public string Name { get; set; }
    public string IconName { get; set; }
    public (string,string)[] Stats { get; set; }
}
