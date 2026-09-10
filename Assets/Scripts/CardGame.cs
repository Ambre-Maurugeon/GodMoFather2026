
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    #region Instance
    private static CardGame _instance;
    public static CardGame Instance => _instance;
    #endregion

    //CardData[] arena= new CardData[2];

    List<CardController> arena = new();
    // previousCard 

    List<CardController> hand = new();

    [SerializeField] private Transform _playedCard01;
    [SerializeField] private Transform _playedCard02;

    [SerializeField]private Transform _arenaParent;

    [SerializeField] private int _jackCombo = 1;
    [SerializeField] private int _knightCombo = 2;
    [SerializeField] private int _queenCombo = 3;
    [SerializeField] private int _kingCombo = 4;


    #region Main
    private void Awake()
    {
        // Instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            _instance = this;
    }
    #endregion

    public void AddInArena(CardController controller)
    {
        arena.Add(controller);

        if (arena.Count == 2) 
            Combo();

    }

    private void Combo()
    {
        //Debug.Log("Combo");

        // oudler TO EDIT
        //int tempoMultiplicateur = 1;
        //if (arena[0].MyData.CardType == CARD_TYPE.OUDLER || arena[0].MyData.CardType == CARD_TYPE.OUDLER)
        //{
        //    Debug.Log("oudler");
        //        tempoMultiplicateur = 1; // depend du card number
        //}

        int cardNumber01 = arena[0].MyData.CardNumber;
        int cardNumber02 = arena[1].MyData.CardNumber;

        int[] cardNumbers = new int[] { cardNumber01, cardNumber02 };

        int tempoScore = 0;

        //none
        if (cardNumber01 <= 10 && cardNumber02 <= 10)
            tempoScore++;
        // king
        else if (cardNumbers.Contains(14))
            tempoScore += _kingCombo;
        // queen
        else if (cardNumbers.Contains(13))
            tempoScore += _queenCombo;
        //knight
        else if (cardNumbers.Contains(12))
            tempoScore += _knightCombo;
        //jack
        else if (cardNumbers.Contains(11))
            tempoScore += _jackCombo;



    }

    public void SelectCard(CardController controller)
    {
        if (hand.Count >= 2) return;

        hand.Add(controller);
        controller.transform.SetParent(_arenaParent, true);
        controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y + 75, controller.transform.position.z); // up

        if(hand.Count >=2)
            Invoke("PlayCards", 0.5f);
    }

    public void RemoveCard(CardController controller)
    {
        hand.Remove(controller);
        controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y - 75, controller.transform.position.z); // down
    }

    private void PlayCards()
    {
        ClearArena();

        arena = new List<CardController>(hand);

        // Visuals => move card to the middle
        hand[0].transform.position = _playedCard01.position;
        hand[1].transform.position = _playedCard02.position;

        // Combo Count
        Combo();
    }

    private void ClearArena()
    {
        arena.Clear();
    }

    private void ClearHand()
    {
        hand.Clear();
    }



    }
