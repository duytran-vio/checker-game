using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity);
    }

    public void Leave(){
        Debug.Log("Leave Room");
    }

}
