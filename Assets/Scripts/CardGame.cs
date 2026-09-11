
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CardGame : MonoBehaviour
{
    #region Instance
    private static CardGame _instance;
    public static CardGame Instance => _instance;
    #endregion

    #region Fields
    // ---- ARENA ----

    [Header("Arena")]
    [SerializeField, FormerlySerializedAs("_playedCard01")] private Transform _arenaCard01;
    [SerializeField, FormerlySerializedAs("_playedCard02")] private Transform _arenaCard02;
    [SerializeField, FormerlySerializedAs("_playedCard03")] private Transform _arenaCard03;

    [SerializeField] private Transform _playedCardsParent;

    private List<CardController> _arena = new();

    // - LINKS -
    private CardController _previousCard;

    private int _links = 0;
    [Header("Hand && deck")]
    [Foldout("Links"), SerializeField] float OneLinkCoeff = 0.15f;
    [Foldout("Links"), SerializeField] float TwoLinksCoeff = 0.20f;
    [Foldout("Links"), SerializeField] float ThreeLinksCoeff = 0.40f;
    [Foldout("Links"), SerializeField] float FourLinksCoeff = 0.60f;
    [Foldout("Links"), SerializeField] float FiveLinksCoeff = 0.80f;


    // ---- HAND ----
    List<CardController> hand = new();

    // ---- SCORE ----
    [Header("Score")]
    //[SerializeField] private TextMeshProUGUI _textScoreJ1;
    //[SerializeField] private TextMeshProUGUI _textScoreJ2;

    //[Space(10)]
    //[SerializeField] private TextMeshProUGUI _textMultiplicateurJ1;
    //[SerializeField] private TextMeshProUGUI _textMultiplicateurJ2;
    //private int _multiplicatorJ1 = 1;
    //private int _multiplicatorJ2 = 1;

    // ---- COMBO ----
    [Header("Combos")]
    [Foldout("Combos"), SerializeField] private int _globalCoeff = 10;
    [Foldout("Combos"), SerializeField] private int _basicCombo = 1;
    [Foldout("Combos"), SerializeField] private int _jackCombo = 2;
    [Foldout("Combos"), SerializeField] private int _knightCombo = 3;
    [Foldout("Combos"), SerializeField] private int _queenCombo = 4;
    [Foldout("Combos"), SerializeField] private int _kingCombo = 5;

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
    #endregion

    private void Combo()
    {
        // check links
        CheckPreviousCard();

        // TO EDIT if J1isPlaying or if J2isPlaying
        // oudler MULTIPLICATOR
        if (_arena[0].MyData.CardType == CARD_TYPE.OUDLER || _arena[0].MyData.CardType == CARD_TYPE.OUDLER)
        {
            if (_arena[0].MyData.CardType == CARD_TYPE.OUDLER)
            {
                Debug.Log("oudler on first card");
                GameManager.Instance?.UpdateMultiplier(_arena[0].MyData.CardNumber);
                //_multiplicatorJ1 = _arena[0].MyData.CardNumber;
                //_textMultiplicateurJ1.text = _multiplicatorJ1.ToString();
            }
            else
            {
                Debug.Log("oudler on second card");
                GameManager.Instance?.UpdateMultiplier(_arena[1].MyData.CardNumber);
                //_multiplicatorJ1 = _arena[1].MyData.CardNumber;
                //_textMultiplicateurJ2.text = _multiplicatorJ2.ToString();
            }
        }
        // multiplicateur agit d�s sa main ou sur les prochains tours ? stop ici ou pas

        // SCORE
        int cardNumber01 = _arena[0].MyData.CardNumber;
        int cardNumber02 = _arena[1].MyData.CardNumber;

        Debug.Log(cardNumber01 + "," + cardNumber02);

        int[] cardNumbers = new int[] { cardNumber01, cardNumber02 };

        int tempoScore = 0;

        //none
        if (cardNumber01 <= 10 && cardNumber02 <= 10)
            tempoScore += _basicCombo * _globalCoeff;
        // king
        else if (cardNumbers.Contains(50))
            tempoScore += _kingCombo * _globalCoeff;
        // queen
        else if (cardNumbers.Contains(40))
            tempoScore += _queenCombo * _globalCoeff;
        //knight
        else if (cardNumbers.Contains(30))
            tempoScore += _knightCombo * _globalCoeff;
        //jack
        else if (cardNumbers.Contains(20))
            tempoScore += _jackCombo * _globalCoeff;

        GameManager.Instance?.UpdateScore(tempoScore);

        // clean arena and keep last card

        if (GameManager.isMiniGameOn == false)
            Invoke("PrepareArenaForNextCombo", 0.75f);
        else
            PrepareArenaForNextCombo();
    }

    private void CheckPreviousCard()
    {
        if (!_previousCard) return;

        Debug.Log("Last card type : " + _previousCard.MyData.CardType + "/n first card type in hand : " + _arena[0].MyData.CardType);
        // reset _links if failure ??

        if (_previousCard.MyData.CardType == _arena[0].MyData.CardType)
        {
            _links++;
        }
        else
            return;

        int rd = Random.Range(0, 100);

        switch (_links)
        {
            case 0: // don't pass here 
                break;
            case 1:
                Debug.Log("Anger" + OneLinkCoeff);
                if (rd <= 60)
                    GameManager.Instance?.IncrementAngryMeter();

                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;

            case 2:
                Debug.Log("Anger" + TwoLinksCoeff);
                if (rd <= 65)
                    GameManager.Instance?.IncrementAngryMeter();

                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;
            case 3:
                Debug.Log("Anger" + ThreeLinksCoeff);
                if (rd <= 70)
                    GameManager.Instance?.IncrementAngryMeter();

                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;
            case 4:
                Debug.Log("Anger" + FourLinksCoeff);
                if (rd <= 75)
                    GameManager.Instance?.IncrementAngryMeter();

                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;
            case 5:
                Debug.Log("Anger" + FiveLinksCoeff);
                if (rd <= 80)
                    GameManager.Instance?.IncrementAngryMeter();

                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;
            default:
                Debug.Log("Anger > 5");
                GameManager.Instance?.IncrementAngryMeter();
                GameManager.Instance?.MonsterAngryMeterDice();
                AudioManager.Instance.PlaySFX(SoundType.LiaisonCarte);
                break;
        }
        AudioManager.Instance.PlaySFX(SoundType.CarteEmpilee);
    }

    private void PrepareArenaForNextCombo()
    {
        // update previous card with last card
        if (_previousCard) Destroy(_previousCard.gameObject);
        _previousCard = _arena[_arena.Count - 1];

        // delete all except the last one
        for (int i = 0; i < _arena.Count - 1; i++)
        {
            Destroy(_arena[i].gameObject);
        }

        // update pos last card (previous card now)
        _previousCard.transform.position = _arenaCard01.position;

        // clear
        _arena.Clear();
        ClearHand();

        CardManager.Instance?.CloseRound();

        // Next Round
        GameManager.Instance?.PlayTurn();

    }


    public void SelectCard(CardController controller)
    {
        if (hand.Count >= 2) return;

        hand.Add(controller);
        controller.transform.SetParent(_playedCardsParent, true);
        controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y + 75, controller.transform.position.z); // up

        if (hand.Count >= 2)
        {
            CardManager.Instance.CanInteract = false;
            controller.IgnoreSelection();
            Invoke("PlayCards", 0.5f);
        }
    }

    public void RemoveCard(CardController controller)
    {
        hand.Remove(controller);
        controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y - 75, controller.transform.position.z); // down
    }

    private void PlayCards()
    {
        _arena = new List<CardController>(hand);

        // Visuals => move card to the middle
        // can do a list of place card
        if (!_previousCard)
        {
            hand[0].transform.position = _arenaCard01.position;
            hand[1].transform.position = _arenaCard02.position;
        }
        else
        {
            hand[0].transform.position = _arenaCard02.position;
            hand[1].transform.position = _arenaCard03.position;
        }

        // Combo Count
        Combo();
    }

    public void HidePreviousCard(bool hide)
    {
        _previousCard.gameObject.SetActive(!hide);
    }

    [Button]
    private void ClearArena()
    {
        foreach (var c in _arena)
            Destroy(c.gameObject);

        _arena.Clear();
    }

    private void ClearHand()
    {
        hand.Clear();
    }



}
