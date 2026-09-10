using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDatabaseManager : MonoBehaviour
{
    #region Instance
    private static CardDatabaseManager _instance;
    public static CardDatabaseManager Instance => _instance;
    #endregion

    #region References
    [SerializeField] private CardDatabase _db;
    #endregion

    public List<CardData> Cards => _db.datas;

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

    public CardData GetCard(int id)
    {
        foreach (var data in _db.datas)
            if (data.id == id)
                return data;

        return null;
    }

}
