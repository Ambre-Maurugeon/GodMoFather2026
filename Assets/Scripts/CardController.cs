using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    private CardData _myData;
    public CardData MyData => _myData;

    private bool _isSelected = false;

    // -- Components --
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
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
        if (!CardManager.Instance.CanInteract) return;

        if(_isSelected)
        {
            _isSelected = false;
            CardGame.Instance?.RemoveCard(this);
        }
        else
        {
            _isSelected = true;
            CardGame.Instance?.SelectCard(this);
        }
    }
    
    #endregion
    }
