using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    private CardData _myData;

    [Header("Values")]
    [SerializeField] private int index;

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
}
