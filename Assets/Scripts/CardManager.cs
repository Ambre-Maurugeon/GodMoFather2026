using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    #region Instance
    private static CardManager _instance;
    public static CardManager Instance => _instance;
    #endregion

    #region References
    [SerializeField] private CardDatabaseManager _dbMgr;

    [Foldout("References"), SerializeField] private GameObject _deckParent;
    [Foldout("References"), SerializeField] private GameObject _cardsParent;
    [Foldout("References"), SerializeField] private GameObject _cardPref;

    #endregion

    #region Fields

    [SerializeField] private int _deckCount = 10;

    private List<CardData> _deck = new List<CardData>();

    // Sorted cards
    private List<CardData> _basics = new List<CardData>();
    private List<CardData> _jacks = new List<CardData>();
    private List<CardData> _knights = new List<CardData>();
    private List<CardData> _queens = new List<CardData>();
    private List<CardData> _kings = new List<CardData>();
    private List<CardData> _oudlers = new List<CardData>();

    public bool CanInteract { get; set; }


    #endregion

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


        // Sort Cards at init
        SortCardsByType();

    }

    private void Start()
    {

        CanInteract = true;
    }
    #endregion


    private void SortCardsByType()
    {
        foreach (var card in _dbMgr.Cards)
        {
            // oudler
            if (card.CardType == CARD_TYPE.OUDLER)
            {
                _oudlers.Add(card);
                continue;
            }

            // others
            switch (card.CardNumber)
            {
                case 14: // KING
                    _kings.Add(card);
                    break;
                case 13: // QUEEN
                    _queens.Add(card);
                    break;
                case 12: // KNIGHT
                    _knights.Add(card);
                    break;
                case 11: // JACK
                    _jacks.Add(card);
                    break;

                default: // BASIC
                    _basics.Add(card);
                    break;

            }
        }
    }

    [Button]
    public async void CreateDeck()
    {
        ClearDeck();
        CardGame.Instance?.ResetMultiplicators();

        await Awaitable.NextFrameAsync();
        await Awaitable.NextFrameAsync();
        await Awaitable.NextFrameAsync();
        await Awaitable.NextFrameAsync();
        await Awaitable.NextFrameAsync();

        // -- FILL DECK --
        for (int i = 0; i < _deckCount; i++)
        {
            // -- GET RD --
                CardData rdCard = GetRandomCard();

            // -- ADD RD --
            _deck.Add(rdCard);
            CardController controller = Instantiate(_cardPref, _cardsParent.transform).GetComponent<CardController>();

            // place card at the right position in the deck
            Transform[] places = _deckParent.GetComponentsInChildren<Transform>();
            controller.transform.position = places[i + 1].position;

            controller.UpdateCardInfo(rdCard);

            AudioManager.Instance?.PlaySFX(SoundType.AudioCardDraw);
        }
    }

    private CardData GetRandomCard()
    {
        CardData rdCard = null;
        int r = Random.Range(0, 100);

        if (r < 5 && _oudlers.Count != 0)
        // rd ds oudler
        {
            int rd = Random.Range(0, _oudlers.Count);
            rdCard = _oudlers[rd];
        }
        else if (r < 15 && _knights.Count != 0)
        // rd ds knights
        {
            int rd = Random.Range(0, _knights.Count);
            rdCard = _knights[rd];
        }
        else if (r < 25 && _queens.Count != 0)
        // rd ds queens
        {
            int rd = Random.Range(0, _queens.Count);
            rdCard = _queens[rd];
        }
        else if (r < 35 && _kings.Count != 0)
        // rd ds kings
        {
            int rd = Random.Range(0, _kings.Count);
            rdCard = _kings[rd];
        }
        else if (r < 55 && _jacks.Count != 0)
        // rd ds jacks
        {
            int rd = Random.Range(0, _jacks.Count);
            rdCard = _jacks[rd];
        }
        else if (_basics.Count != 0)
        // rd ds basics
        {
            int rd = Random.Range(0, _basics.Count);
            rdCard = _basics[rd];
        }

        return rdCard;
    }

    public void CloseRound()
    {
        CheckDeckCount();

        CanInteract = true;
    }

    public void ClearDeck()
    {
        _deck.Clear();

        foreach (Transform child in _cardsParent.transform)
            Destroy(child.gameObject);
    }

    private void CheckDeckCount()
    {
        if (_cardsParent.GetComponentsInChildren<Transform>().Length <= 1)
            CreateDeck();
    }


    //private void ShuffleDeck()
    //{

    //}

}
