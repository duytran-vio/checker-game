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
        if (photonView.IsMine){
            playerType = PhotonNetwork.IsMasterClient ? PlayerType.PLAYER : PlayerType.OPPONENT;
            GameManager.Instance.SetThisUserId(photonView.ViewID);
        }
    }

    void Update(){
        if (photonView.IsMine){
            InputManager.HandleMouseInput(playerType);
        }
    }
}
