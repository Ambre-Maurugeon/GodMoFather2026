using UnityEngine;
using System.Collections;
using System;

public enum GameState
{
    Innit,
    Player1,
    Player2,
    AngryPhase,
    Victory,
    Defeat
}
public class GameManager : MonoBehaviour
{
    private GameState gameState = GameState.Innit;
    public int chanceAngryCount = 1;
    [SerializeField] private int cardPit = 10;
    private int _DICE = 6;
    private bool isGameOn = false;
    private bool isPlayer1Turn = false;
    private bool isPlayer2Turn = false;


    public void Start()
    {
        isGameOn = true;
        Mambo();
    }
    void Mambo()
    {
        int score = 0;
        while (isGameOn)
        {
            Debug.Log("Mambo" + score);

            if (score == 10)
                isGameOn = false;

            if (isPlayer1Turn)
            {

            }
            else if (isPlayer2Turn)
            {

            }
        }
    }
    void DrawCard()
    {

    }
}
