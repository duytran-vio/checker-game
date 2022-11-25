using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class ReceiveRequest : MonoBehaviour
{
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)EventCode.MOVEMENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userId = (int)data[0];
            PlayerType playerType = (PlayerType)data[1];
            Vector2Int fromCell = new Vector2Int((int)data[2], (int)data[3]);
            Vector2Int toCell = new Vector2Int((int)data[4], (int)data[5]);
            GameManager.Instance.MoveFromNetwork(userId, playerType, fromCell, toCell);
        }
    }
}
