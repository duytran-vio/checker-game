using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView photonView;
    [SerializeField]private PlayerType playerType;
    void Start(){
        photonView = GetComponent<PhotonView>();
        playerType = PhotonNetwork.IsMasterClient ? PlayerType.PLAYER : PlayerType.OPPONENT;
    }

    void Update(){
        if (photonView.IsMine){
            InputManager.HandleMouseInput(playerType);
        }
    }
}
