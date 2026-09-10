using NaughtyAttributes;
using UnityEngine;

public class MonsterAngryMeter : MonoBehaviour
{
    #region Instance
    private static MonsterAngryMeter _instance;
    public static MonsterAngryMeter Instance => _instance;
    #endregion

    #region References
    [Foldout("References"), SerializeField] private Transform _monsterMeterParent;
    [Foldout("References"), SerializeField] private GameObject _bareMeterPref;
    #endregion

    #region Main
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    [Button]
    public void Create()
    {
        Instantiate(_bareMeterPref, _monsterMeterParent);
    }

    [Button]
    public void Clear()
    {
        foreach (Transform child in _monsterMeterParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetMeter(int meters)
    {
        Clear();
        int i = 0;

        while (i != meters)
        {
            i++;
            Create();
        }
    }
}