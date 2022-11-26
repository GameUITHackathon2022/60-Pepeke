using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMGameDataManager : MonoBehaviour
{
    public DTNLiveData<long> CoinLiveData = new DTNLiveData<long>("CoinLiveData",
       //Save
       (DTNLiveData<long> liveData) => {
           PlayerPrefs.SetString(liveData.name, liveData.Get() + "");
       },
       //Get
       (DTNLiveData<long> liveData) => {
           return long.Parse(PlayerPrefs.GetString(liveData.name, "10"));
       });

    public DTNLiveData<int> HeartLiveData = new DTNLiveData<int>("HeartLiveData",
        //Save
        (DTNLiveData<int> liveData) => {
            PlayerPrefs.SetInt(liveData.name, liveData.Get());
        },
        //Get
        (DTNLiveData<int> liveData) => {
            return PlayerPrefs.GetInt(liveData.name, 2);
        });

    public DTNLiveData<int> StarLiveData = new DTNLiveData<int>("StarLiveData",
        //Save
        (DTNLiveData<int> liveData) => {
            PlayerPrefs.SetInt(liveData.name, liveData.Get());
        },
        //Get
        (DTNLiveData<int> liveData) => {
            return PlayerPrefs.GetInt(liveData.name, 0);
        });
}
