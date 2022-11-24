using UnityEngine;
public enum PlayerType
{
    PLAYER,
    OPPONENT,
    NONE
}

public class Config
{
    public static int CellSize = 2;
    public static int TableSize = 8;

    public static PlayerType SwitchTurn(PlayerType oldTurn)
    {
        switch (oldTurn)
        {
            case PlayerType.PLAYER:
                return PlayerType.OPPONENT;
            case PlayerType.OPPONENT:
                return PlayerType.PLAYER;
            default:
                return PlayerType.NONE;
        }
    }
}