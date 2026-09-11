
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    #region Instance
    private static CardGame _instance;
    public static CardGame Instance => _instance;
    #endregion

    // -- ARENA --
    List<CardController> arena = new();
    CardController previousCard;

    [SerializeField] private Transform _playedCard01;
    [SerializeField] private Transform _playedCard02;
    [SerializeField] private Transform _playedCard03;

    [SerializeField]private Transform _playedCardsParent;

    // -- HAND --

    List<CardController> hand = new();

    // -- SCORE --
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _textScoreJ1;
    [SerializeField] private TextMeshProUGUI _textScoreJ2;
    private int _scoreJ1;
    private int _scoreJ2;

    // -- COMBO --
    [Header("Combos")]
    [SerializeField] private int _basicCombo = 1;
    [SerializeField] private int _jackCombo = 2;
    [SerializeField] private int _knightCombo = 3;
    [SerializeField] private int _queenCombo = 4;
    [SerializeField] private int _kingCombo = 5;


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

        Debug.Log(cardNumber01 + "," + cardNumber02);

        int[] cardNumbers = new int[] { cardNumber01, cardNumber02 };

        int tempoScore = _scoreJ1;

        //none
        if (cardNumber01 <= 10 && cardNumber02 <= 10)
            tempoScore+= _basicCombo;
        // king
        else if (cardNumbers.Contains(50))
            tempoScore += _kingCombo;
        // queen
        else if (cardNumbers.Contains(40))
            tempoScore += _queenCombo;
        //knight
        else if (cardNumbers.Contains(30))
            tempoScore += _knightCombo;
        //jack
        else if (cardNumbers.Contains(20))
            tempoScore += _jackCombo;

        _scoreJ1 = tempoScore;
        _textScoreJ1.text = tempoScore.ToString();

        // finir le pli
        Invoke("PrepareArenaForNextCombo", 1.5f);
    }

    private void PrepareArenaForNextCombo()
    {
        if(previousCard) Destroy(previousCard.gameObject);

        previousCard = arena[arena.Count - 1];

        // delete all except the last one
        for (int i = 0; i < arena.Count - 1; i++)
        {
            Destroy(arena[i].gameObject);
        }

        previousCard.transform.position = _playedCard01.position;

        // clear
        arena.Clear();
        ClearHand();

    }


    public void SelectCard(CardController controller)
    {
        if (hand.Count >= 2) return;

        hand.Add(controller);
        controller.transform.SetParent(_playedCardsParent, true);
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
        arena = new List<CardController>(hand);

        // Visuals => move card to the middle
        // can do a list of place card
        if (!previousCard)
        {
            hand[0].transform.position = _playedCard01.position;
            hand[1].transform.position = _playedCard02.position;
        }
        else
        {
            hand[0].transform.position = _playedCard02.position;
            hand[1].transform.position = _playedCard03.position;
        }

        // Combo Count
        Combo();
    }

    [Button]
    private void ClearArena()
    {
        foreach (var c in arena)
            Destroy(c.gameObject);

        arena.Clear();
    }

    private void ClearHand()
    {
        hand.Clear();
    }



    }
