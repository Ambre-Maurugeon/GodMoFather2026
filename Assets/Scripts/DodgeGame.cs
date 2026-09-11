using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AnimationType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Fade
}

[Serializable]
public class AnimatedTarget
{
    public GameObject targetObject;
    public AnimationType animType;
    public float amount = 500f; // Offset distance (pixels/units) or Target Alpha (0 to 1) for Fade
    public float duration = 3f;

    [HideInInspector] public Vector2 initialAnchoredPos;
    [HideInInspector] public Vector3 initialWorldPos;
    [HideInInspector] public Color initialColor;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    public void CacheInitialState()
    {
        if (targetObject == null) return;

        rectTransform = targetObject.GetComponent<RectTransform>();
        canvasGroup = targetObject.GetComponent<CanvasGroup>();
        spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (rectTransform != null)
            initialAnchoredPos = rectTransform.anchoredPosition;

        initialWorldPos = targetObject.transform.position;

        if (canvasGroup != null)
            initialColor = new Color(1, 1, 1, canvasGroup.alpha);
        else if (spriteRenderer != null)
            initialColor = spriteRenderer.color;
    }
}

public class DodgeGame : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Custom Transitions (Inspector List)")]
    [SerializeField] private List<AnimatedTarget> transitionElements = new List<AnimatedTarget>();

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private float minPos = -5f;
    [SerializeField] private float maxPos = 5f;
    [SerializeField] private float playerSpeedPerTile = 5f;
    [SerializeField] private float eventDuration = 15f;

    [Header("Player Intro/Outro Anim")]
    [SerializeField] private float playerEntryOffsetY = 6f;
    [SerializeField] private float playerAnimDuration = 0.4f;
    [SerializeField] private GameObject monsterFalse;
    [SerializeField] private GameObject monsterTrue;

    [Header("Cards")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardSpeed = 2f;
    [SerializeField] private float cardPerSecond = 0.55f;
    [SerializeField] private int minCardPosSpawn = -3;
    [SerializeField] private int maxCardPosSpawn = 3;
    [SerializeField] private int spawnY = 10;
    [SerializeField] private int despawnY = -6;

    [Header("Score")]
    [SerializeField] private int lostPerHit = 30;
    [HideInInspector] public static int scoreLost = 0;
    public event Action<int> OnGameEnded;

    private readonly List<GameObject> _activeCards = new List<GameObject>();
    private bool _isGameRunning;
    private bool _isTransitioning;

    private Vector3 _playerRestPos;
    private Coroutine _gameTimerCoroutine;
    private Coroutine _spawnRoutine;

    private void OnEnable()
    {
        PlayerCollision.OnCardHit += HandleCardHit;
    }

    private void OnDisable()
    {
        PlayerCollision.OnCardHit -= HandleCardHit;
    }

    private void HandleCardHit(GameObject cardObject)
    {
        if (!_isGameRunning) return;

        scoreLost += lostPerHit;

        // Destroy card after get hit
        if (_activeCards.Contains(cardObject))
        {
            _activeCards.Remove(cardObject);
        }

        Destroy(cardObject);
    }

    void Awake()
    {
        // Cache initial positions and styles for all assigned elements
        for (int i = 0; i < transitionElements.Count; i++)
        {
            transitionElements[i].CacheInitialState();
        }

        if (playerTransform != null)
        {
            _playerRestPos = playerTransform.position;
            // Spawn player off-screen initially
            playerTransform.position = _playerRestPos - new Vector3(0f, playerEntryOffsetY, 0f);
        }

        if (playerRenderer == null && playerTransform != null)
            playerRenderer = playerTransform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (!_isGameRunning || _isTransitioning) return;

        HandlePlayerMovement();
        HandleCardsMovement();
    }

    public void StartGame()
    {
        monsterFalse.SetActive(false);
        monsterTrue.SetActive(true);
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX(SoundType.MonsterGrowl);
        AudioManager.Instance.PlayMusic(SoundType.MiniJeu);
        if (_isGameRunning || _isTransitioning) return;
        CardGame.Instance?.HidePreviousCard(true);
        StartCoroutine(StartSequenceRoutine());
    }

    public void StopGame()
    {
        monsterFalse.SetActive(true);
        monsterTrue.SetActive(false);
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX(SoundType.MonsterGrowl);
        AudioManager.Instance.PlayMusic(SoundType.MainTheme);
        if (!_isGameRunning && !_isTransitioning) return;
        StartCoroutine(StopSequenceRoutine());
        CardGame.Instance?.HidePreviousCard(false);

        OnGameEnded?.Invoke(DodgeGame.scoreLost);
    }

    private IEnumerator StartSequenceRoutine()
    {
        _isTransitioning = true;

        // Animate all custom elements (Hide / Shift)
        List<Coroutine> anims = new List<Coroutine>();
        for (int i = 0; i < transitionElements.Count; i++)
        {
            anims.Add(StartCoroutine(PlayTransition(transitionElements[i], false)));
        }

        for (int i = 0; i < anims.Count; i++)
            yield return anims[i];

        // Player introduction
        yield return MovePlayerToRoutine(_playerRestPos, playerAnimDuration);
        yield return PlayerPreviewRoutine();

        _isTransitioning = false;
        _isGameRunning = true;

        _spawnRoutine = StartCoroutine(SpawnCardsRoutine());
        _gameTimerCoroutine = StartCoroutine(GameTimerRoutine());
    }

    private IEnumerator StopSequenceRoutine()
    {
        _isGameRunning = false;
        _isTransitioning = true;

        if (_gameTimerCoroutine != null) StopCoroutine(_gameTimerCoroutine);
        if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);

        ClearCards();

        // Player exit sequence
        Vector3 centerPos = new Vector3(0f, _playerRestPos.y, _playerRestPos.z);
        yield return MovePlayerToRoutine(centerPos, playerAnimDuration);

        Vector3 offscreenPos = centerPos - new Vector3(0f, playerEntryOffsetY, 0f);
        yield return MovePlayerToRoutine(offscreenPos, playerAnimDuration);

        // Reset all custom elements back to their initial state
        List<Coroutine> anims = new List<Coroutine>();
        for (int i = 0; i < transitionElements.Count; i++)
        {
            anims.Add(StartCoroutine(PlayTransition(transitionElements[i], true)));
        }

        for (int i = 0; i < anims.Count; i++)
            yield return anims[i];

        _isTransitioning = false;
    }

    private IEnumerator PlayTransition(AnimatedTarget item, bool reverse)
    {
        if (item.targetObject == null) yield break;

        float elapsed = 0f;

        if (item.animType == AnimationType.Fade)
        {
            float startAlpha = reverse ? item.amount : (item.canvasGroup != null ? item.canvasGroup.alpha : item.initialColor.a);
            float targetAlpha = reverse ? item.initialColor.a : item.amount;

            while (elapsed < item.duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / item.duration);
                float a = Mathf.Lerp(startAlpha, targetAlpha, t);

                if (item.canvasGroup != null) item.canvasGroup.alpha = a;
                else if (item.spriteRenderer != null)
                {
                    Color c = item.spriteRenderer.color;
                    c.a = a;
                    item.spriteRenderer.color = c;
                }
                yield return null;
            }
        }
        else
        {
            Vector3 offset = Vector3.zero;
            switch (item.animType)
            {
                case AnimationType.MoveUp: offset = Vector3.up * item.amount; break;
                case AnimationType.MoveDown: offset = Vector3.down * item.amount; break;
                case AnimationType.MoveLeft: offset = Vector3.left * item.amount; break;
                case AnimationType.MoveRight: offset = Vector3.right * item.amount; break;
            }

            if (item.rectTransform != null)
            {
                Vector2 start = reverse ? item.initialAnchoredPos + (Vector2)offset : item.initialAnchoredPos;
                Vector2 end = reverse ? item.initialAnchoredPos : item.initialAnchoredPos + (Vector2)offset;

                while (elapsed < item.duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / item.duration);
                    item.rectTransform.anchoredPosition = Vector2.Lerp(start, end, t);
                    yield return null;
                }
                item.rectTransform.anchoredPosition = end;
            }
            else
            {
                Vector3 start = reverse ? item.initialWorldPos + offset : item.initialWorldPos;
                Vector3 end = reverse ? item.initialWorldPos : item.initialWorldPos + offset;

                while (elapsed < item.duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / item.duration);
                    item.targetObject.transform.position = Vector3.Lerp(start, end, t);
                    yield return null;
                }
                item.targetObject.transform.position = end;
            }
        }
    }

    private IEnumerator PlayerPreviewRoutine()
    {
        Vector3 basePos = playerTransform.position;

        yield return MovePlayerToRoutine(basePos + Vector3.up * 0.5f, 0.1f);
        yield return MovePlayerToRoutine(basePos, 0.1f);

        yield return MovePlayerToRoutine(basePos + Vector3.right * 0.5f, 0.1f);
        yield return MovePlayerToRoutine(basePos, 0.1f);

        yield return MovePlayerToRoutine(basePos + Vector3.left * 0.5f, 0.1f);
        yield return MovePlayerToRoutine(basePos, 0.1f);

        if (playerRenderer != null)
        {
            Color origColor = playerRenderer.color;
            yield return FadeRoutine(origColor, 0.2f, 0.15f);
            yield return FadeRoutine(origColor, 1f, 0.15f);
        }
    }

    private IEnumerator MovePlayerToRoutine(Vector3 targetPos, float duration)
    {
        Vector3 startPos = playerTransform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            playerTransform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        playerTransform.position = targetPos;
    }

    private IEnumerator FadeRoutine(Color baseColor, float targetAlpha, float duration)
    {
        Color startColor = playerRenderer.color;
        Color endColor = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            playerRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        playerRenderer.color = endColor;
    }

    private IEnumerator GameTimerRoutine()
    {
        yield return new WaitForSeconds(eventDuration);
        StopGame();
    }

    private void ClearCards()
    {
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

            card.transform.position += Vector3.down * (cardSpeed * Time.deltaTime);

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
            int randomX = UnityEngine.Random.Range(minCardPosSpawn, maxCardPosSpawn + 1);
            Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

            GameObject card = Instantiate(cardPrefab, spawnPos, Quaternion.identity);
            _activeCards.Add(card);

            yield return new WaitForSeconds(1f / cardPerSecond);
        }
    }
}