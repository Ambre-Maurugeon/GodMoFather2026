using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    #region Instance
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    #endregion

    private bool isGameOn = false;
    public static bool isMiniGameOn = false;
    private int _monsterAngryMeter = 0;
    [SerializeField] private DodgeGame dodgeGame;
    [SerializeField] private MonsterAngryMeter monsterAngryMeter;
    [Header("SCORE")]
    private int _player1Score = 0;
    private int _player2Score = 0;
    private int _player1Multiplier = 1;
    private int _player2Multiplier = 1;
    private bool _isPlayer1Turn = false;
    private bool _isPlayer2Turn = false;
    [SerializeField] private int _scoreGoal = 1000; //1k

    [Header("Text Link")]
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI scoreGoalText;
    [SerializeField] private TextMeshProUGUI player1MultiText;
    [SerializeField] private TextMeshProUGUI player2MultiText;
    [SerializeField] private TextMeshProUGUI angryMeterText;
    [SerializeField] private TextMeshProUGUI diceResultText;
    [Header("Colors")]
    [SerializeField] private GameObject iconPlayer1;
    [SerializeField] private GameObject iconPlayer2;
    [SerializeField] private Color myTurnColor;
    [SerializeField] private Color afkColor;

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

    void Start()
    {
        dodgeGame = FindFirstObjectByType<DodgeGame>();
        monsterAngryMeter = FindFirstObjectByType<MonsterAngryMeter>();

        isGameOn = true;

        _isPlayer1Turn = true;
        _isPlayer2Turn = false;

        CardManager.Instance?.CreateDeck();
        StartCoroutine("StartGame");
        AudioManager.Instance?.PlayMusic(SoundType.MainTheme);
    }

    IEnumerator StartGame()
    {
        string winner = "None";


        while (isGameOn)
        {
            // Endgame
            if (_player1Score >= _scoreGoal || _player2Score >= _scoreGoal)
            {
                ChangePlayerTurn(0);
                isGameOn = false;
                winner = _player1Score > _player2Score ? "Player 1" : "Player 2";
                Debug.Log($"Winner is {winner}");
            }

            if (_isPlayer1Turn && !_isPlayer2Turn)
            {
                // PlayTurn();
            }
            else if (_isPlayer2Turn && !_isPlayer1Turn)
            {
                // PlayTurn();
            }

            yield return null;
        }
    }

    public void UpdateScore(int score)
    {
        if (_isPlayer1Turn)
            _player1Score += score * _player1Multiplier;
        else if (_isPlayer2Turn)
            _player2Score += score * _player2Multiplier;

        if (score != 0)
            AudioManager.Instance.PlaySFX(SoundType.PointsPerdus);
        UpdateText();
    }

    public void UpdateMultiplier(int multiplier)
    {
        if (_isPlayer1Turn)
        {
            _player1Multiplier += multiplier;
            player1MultiText.text = "x" +  _player1Multiplier.ToString();
        }
        else if (_isPlayer2Turn)
        {
            _player2Multiplier += multiplier;
            player2MultiText.text = "x" +  _player2Multiplier.ToString();
        }
    }


    public void PlayTurn()
    {
        if (_isPlayer1Turn && !_isPlayer2Turn)
        {
            ChangePlayerTurn(2);
        }
        else if (_isPlayer2Turn && !_isPlayer1Turn)
        {
            ChangePlayerTurn(1);
        }
        //_monsterAngryMeter++;

        //MonsterAngryMeterDice(_monsterAngryMeter);
        UpdateText();
    }
    public void IncrementAngryMeter()
    {
        _monsterAngryMeter++;
        monsterAngryMeter.SetMeter(this._monsterAngryMeter);
    }
    void ChangePlayerTurn(int changeTo)
    {
        switch (changeTo)
        {
            case 0:
                _isPlayer1Turn = false;
                _isPlayer2Turn = false;
                if (iconPlayer1 != null)
                    iconPlayer1.GetComponent<Image>().color = afkColor;
                if (iconPlayer2 != null)
                    iconPlayer2.GetComponent<Image>().color = afkColor;
                break;
            case 1:
                _isPlayer1Turn = true;
                _isPlayer2Turn = false;
                if (iconPlayer1 != null)
                    iconPlayer1.GetComponent<Image>().color = myTurnColor;
                if (iconPlayer2 != null)
                    iconPlayer2.GetComponent<Image>().color = afkColor;
                break;
            case 2:
                _isPlayer1Turn = false;
                _isPlayer2Turn = true;
                if (iconPlayer1 != null)
                    iconPlayer1.GetComponent<Image>().color = afkColor;
                if (iconPlayer2 != null)
                    iconPlayer2.GetComponent<Image>().color = myTurnColor;
                break;
            default:
                _isPlayer1Turn = true;
                _isPlayer2Turn = false;
                if (iconPlayer1 != null)
                    iconPlayer1.GetComponent<Image>().color = myTurnColor;
                if (iconPlayer2 != null)
                    iconPlayer2.GetComponent<Image>().color = afkColor;
                Debug.Log($"{changeTo} is not valid");
                break;
        }
    }
    public void MonsterAngryMeterDice()
    {
        monsterAngryMeter.SetMeter(this._monsterAngryMeter);

        if (_monsterAngryMeter <= 0) return;

        int[] results = new int[_monsterAngryMeter];
        bool hasSix7 = false;

        for (int i = 0; i < _monsterAngryMeter; i++)
        {
            results[i] = Random.Range(1, 7);

            if (results[i] == 6)
            {
                hasSix7 = true;
            }
        }

        if (hasSix7)
        {
            Debug.Log("Prepare to dodge !");
            this._monsterAngryMeter = 0;
            DodgeGame.scoreLost = 0;

            dodgeGame.OnGameEnded += HandleDodgeResult;
            dodgeGame.StartGame();

            monsterAngryMeter.SetMeter(this._monsterAngryMeter);
        }
        if (diceResultText != null)
            diceResultText.text = "Dice roll : " + string.Join(", ", results);
    }
    private void HandleDodgeResult(int lostScore)
    {
        if (lostScore != 0) AudioManager.Instance.PlaySFX(SoundType.PointsPerdus);
        dodgeGame.OnGameEnded -= HandleDodgeResult;

        // Reverse removing score (it's the other's turn)
        if (_isPlayer1Turn && !_isPlayer2Turn)
            _player2Score -= lostScore;
        if (!_isPlayer1Turn && _isPlayer2Turn)
            _player1Score -= lostScore;

        Debug.Log($"Total Lost: {DodgeGame.scoreLost}");
        UpdateText();
    }
    void UpdateText()
    {
        if (scoreGoalText != null)
            scoreGoalText.text = $"Goal {_scoreGoal}";
        if (player1ScoreText != null)
            player1ScoreText.text = $"{_player1Score}";
        if (player2ScoreText != null)
            player2ScoreText.text = $"{_player2Score}";
        if (angryMeterText != null)
            angryMeterText.text = $"Angry meter : {_monsterAngryMeter}";

        // Player's turn
        if (_isPlayer1Turn && !_isPlayer2Turn)
        {
            currentTurnText.text = "Player 1's turn";
        }
        else if (_isPlayer2Turn && !_isPlayer1Turn)
        {
            currentTurnText.text = "Player 2's turn";
        }
    }

    public void ResetMultiplicators()
    {
        _player1Multiplier = 1;
        _player2Multiplier = 1;
    }
    public int WhosTurn()
    {
        if (_isPlayer1Turn && !_isPlayer2Turn)
            return 1;
        if (!_isPlayer1Turn && _isPlayer2Turn)
            return 2;
        else
            return 0;
    }

}
