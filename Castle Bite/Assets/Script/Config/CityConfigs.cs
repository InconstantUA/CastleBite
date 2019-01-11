using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "_AllCitiesConfigs", menuName = "Config/City/Configs")]
public class CityConfigs : ScriptableObject
{
    public List<CityConfig> cityConfigs;

    public CityConfig this[CityID cityID]
    {
        get
        {
            return cityConfigs.First(c => c.cityID == cityID);
        }
    }
}
