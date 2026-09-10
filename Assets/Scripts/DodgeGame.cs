using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class DodgeGame : MonoBehaviour
{
    [Header("Move UI")]
    [SerializeField] private RectTransform moveDeck;
    [SerializeField] private float _moveDistance = 500f;
    [SerializeField] private float _moveDuration = 2f;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minPos = -5f;
    [SerializeField] private float maxPos = 5f;
    [SerializeField] private float playerSpeedPerTile = 5f;
    [SerializeField] private float eventDuration = 15f;

    [Header("Cards")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardSpeed = 2f;
    [SerializeField] private float cardPerSecond = 0.55f;
    [SerializeField] private int minCardPosSpawn = -3;
    [SerializeField] private int maxCardPosSpawn = 3;
    [SerializeField] private int spawnY = 10;
    [SerializeField] private int despawnY = -6;
    [Header("Score")]
    [SerializeField] private int damagePerHit = 30;
    private readonly List<GameObject> _activeCards = new List<GameObject>();
    private bool _isGameRunning;

    void Start()
    {
    }

    void Update()
    {
        if (!_isGameRunning) return;

        HandlePlayerMovement();
        HandleCardsMovement();
    }

    public void StartGame()
    {
        _isGameRunning = true;
        StartCoroutine(SpawnCardsRoutine());
        StartCoroutine(GameTimerRoutine());
    }

    public void StopGame()
    {
        _isGameRunning = false;
        StopAllCoroutines();
        ClearCards();
        Debug.Log("Game Over: Event Finished!");
    }

    private IEnumerator GameTimerRoutine()
    {
        // Wait for the duration of the event
        yield return new WaitForSeconds(eventDuration);
        StopGame();
    }

    private void ClearCards()
    {
        // Destroy remaining active cards
        for (int i = 0; i < _activeCards.Count; i++)
        {
            if (_activeCards[i] != null)
                Destroy(_activeCards[i]);
        }
        _activeCards.Clear();
    }

    private void HandlePlayerMovement()
    {
        float horizontal = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.qKey.isPressed)
                horizontal -= 1f;

            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                horizontal += 1f;
        }

        Vector3 newPos = playerTransform.position + Vector3.right * (horizontal * playerSpeedPerTile * Time.deltaTime);
        newPos.x = Mathf.Clamp(newPos.x, minPos, maxPos);
        playerTransform.position = newPos;
    }

    private void HandleCardsMovement()
    {
        for (int i = _activeCards.Count - 1; i >= 0; i--)
        {
            GameObject card = _activeCards[i];

            if (card == null)
            {
                _activeCards.RemoveAt(i);
                continue;
            }

            // Move card downwards
            card.transform.position += Vector3.down * (cardSpeed * Time.deltaTime);

            // Despawn check
            if (card.transform.position.y < despawnY)
            {
                Destroy(card);
                _activeCards.RemoveAt(i);
            }
        }
    }

    private IEnumerator SpawnCardsRoutine()
    {
        while (_isGameRunning)
        {
            int randomX = Random.Range(minCardPosSpawn, maxCardPosSpawn + 1);
            Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

            GameObject card = Instantiate(cardPrefab, spawnPos, Quaternion.identity);
            _activeCards.Add(card);

            yield return new WaitForSeconds(1f / cardPerSecond);
        }
    }

    private IEnumerator MoveDeck(float distance, float duration)
    {
        Vector2 startPos = moveDeck.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(0f, distance);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            moveDeck.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        moveDeck.anchoredPosition = targetPos;
    }

    public void UIAway()
    {
        StartCoroutine(MoveDeck(_moveDistance, _moveDuration));
    }
}