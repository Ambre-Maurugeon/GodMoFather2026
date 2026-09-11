using UnityEngine;

public enum CARD_TYPE
{
    NONE = 0,
    DARK_MOON = 1,
    DARK_SUN = 2,
    LIGHT_MOON = 3,
    LIGHT_SUN = 4,
    OUDLER = 5,
}


public enum GameState
{
    INNIT,
    Player1,
    Player2,
    AngryPhase,
    Victory,
    Defeat
}

public enum COMBO
{
    NONE = 0,
    JACK = 1,
    KNIGHT = 2,
    QUEEN = 3,
    KING = 4,
    OUDLER = 5,
}