public enum CARD_TYPE
{
    NONE = 0,
    SPADE = 1,
    DIAMOND = 2,
    HEART = 3,
    CLUB = 4
}

public enum CARD_COLOR
{
    NONE = 0,
    RED = 1,
    BLACK = 2,
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