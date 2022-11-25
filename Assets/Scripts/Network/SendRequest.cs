using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SendRequest : MonoSingleton<SendRequest>
{
    public void SendRequestMovement(int userId, PlayerType playerType, Vector2Int fromCell, Vector2Int toCell)
    {
        object[] content = {userId, playerType, fromCell.x, fromCell.y, toCell.x, toCell.y};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EventCode.MOVEMENT, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
