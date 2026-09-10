using System.Collections.Generic;
using NaughtyAttributes;
using NUnit.Framework;
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
    [Foldout("References"), SerializeField] private GameObject _cardPref;

    #endregion

    #region Fields

    private List<CardData> deck = new List<CardData>();

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
    }

    private void Start()
    {

    }
    #endregion

    [Button]
    public void CreateDeck()
    {
        ClearDeck();

        // fill deck
        List<CardData> drawPile = new List<CardData>(_dbMgr.Cards);

        while (drawPile.Count > 0)
        {
            // get rd
            int rd = Random.Range(0, drawPile.Count);
            CardData rdCard = drawPile[rd];

            // add rd
            deck.Add(rdCard);
            CardController controller = Instantiate(_cardPref, _deckParent.transform).GetComponent<CardController>();
            controller.UpdateCardInfo(rdCard);

            // update pile
            drawPile.RemoveAt(rd);
        }

    }


    public void ClearDeck()
    {
        deck.Clear();

        foreach (Transform child in _deckParent.transform)
            Destroy(child.gameObject);
    }

    //private void ShuffleDeck()
    //{

    //}

}
