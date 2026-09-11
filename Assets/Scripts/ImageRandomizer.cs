using UnityEngine;
using UnityEngine.UI;

public class ImageRandomizer : MonoBehaviour
{
    private CardDatabaseManager _db;
    private SpriteRenderer _sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _sr = GetComponent<SpriteRenderer>();

        _db = CardDatabaseManager.Instance;

        RandomImage();
    }

    private void RandomImage()
    {
        int rd = Random.Range(0,_db.Cards.Count);
        _sr.sprite = _db.GetCard(rd).Sprite;
    }
    
}
