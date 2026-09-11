using System;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static event Action<GameObject> OnCardHit;
    public Sprite P1;
    public Sprite P2;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Card"))
        {
            OnCardHit?.Invoke(collision.gameObject);
            AudioManager.Instance.PlaySFX(SoundType.PointsPerdus);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || spriteRenderer == null) return;

        int turn = GameManager.Instance.WhosTurn();

        if (turn == 1)
        {
            spriteRenderer.sprite = P1;
        }
        else if (turn == 2)
        {
            spriteRenderer.sprite = P2;
        }
    }
}