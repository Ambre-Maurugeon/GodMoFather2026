using System;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static event Action<GameObject> OnCardHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Card"))
        {
            OnCardHit?.Invoke(collision.gameObject);
            AudioManager.Instance.PlaySFX(SoundType.PointsPerdus);
        }
    }
}