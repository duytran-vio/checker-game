using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiSingleton<T> where T : new()
{

    private static T _api;

    public static T Instance
    {
        get
        {
            if (_api == null)
            {
                _api = new T();

            }
            return _api;
        }
    }


}