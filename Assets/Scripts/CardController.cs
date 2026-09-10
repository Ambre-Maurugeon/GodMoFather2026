using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    private CardData _myData;
    public CardData MyData => _myData;

    [Header("Values")]
    [SerializeField] private int _index;

    private bool _isSelected = false;

    // -- Components --
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCardInfo(CardData data)
    {
        _myData = data;
        _img.sprite =  _myData.Sprite;
    }

    #region Click

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(_isSelected)
        {
            _isSelected = false;
            CardGame.Instance.RemoveCard(this);
        }
        else
        {
            _isSelected = true;
            CardGame.Instance.SelectCard(this);
        }
    }
        #endregion
    }
