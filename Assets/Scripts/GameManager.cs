using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGameOn = false;
    private int _monsterAngryMeter = 0;
    [SerializeField] private DodgeGame dodgeGame;
    [SerializeField] private MonsterAngryMeter monsterAngryMeter;
    [Header("SCORE")]
    [SerializeField] private int _player1Score = 0;
    [SerializeField] private int _player2Score = 0;
    private bool _isPlayer1Turn = false;
    private bool _isPlayer2Turn = false;
    [SerializeField] private int _scoreGoal = 1000; //1k

    [Header("Text Link")]
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI scoreGoalText;
    [SerializeField] private TextMeshProUGUI angryMeterText;
    [SerializeField] private TextMeshProUGUI diceResultText;
    [Header("Colors")]
    [SerializeField] private GameObject iconPlayer1;
    [SerializeField] private GameObject iconPlayer2;
    [SerializeField] private Color myTurnColor;
    [SerializeField] private Color afkColor;

    public void Start()
    {
        dodgeGame = FindFirstObjectByType<DodgeGame>();
        monsterAngryMeter = FindFirstObjectByType<MonsterAngryMeter>();

        isGameOn = true;
        CardManager.Instance?.CreateDeck();
        _isPlayer1Turn = true;
        _isPlayer2Turn = false;
        StartCoroutine("StartGame");
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
    public void PlayTurn()
    {
        if (_isPlayer1Turn && !_isPlayer2Turn)
        {
            _player1Score += 100;
            ChangePlayerTurn(2);
        }
        else if (_isPlayer2Turn && !_isPlayer1Turn)
        {
            _player2Score += 100;
            ChangePlayerTurn(1);
        }
        _monsterAngryMeter++;
        monsterAngryMeter.SetMeter(_monsterAngryMeter);
        MonsterAngryMeterDice(_monsterAngryMeter);
        UpdateText();
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
    int[] MonsterAngryMeterDice(int multiplier)
    {
        if (multiplier <= 0) return new int[0];
        int[] results = new int[multiplier];
        bool hasSix7 = false;

        for (int i = 0; i < multiplier; i++)
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
            _monsterAngryMeter = 0;
            DodgeGame.scoreLost = 0;

            dodgeGame.OnGameEnded += HandleDodgeResult;
            dodgeGame.StartGame();
        }
        if (diceResultText != null)
            diceResultText.text = "Dice roll : " + string.Join(", ", results);
        return results;
    }
    private void HandleDodgeResult(int lostScore)
    {
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
            player1ScoreText.text = $"Player 1's score : {_player1Score}";
        if (player2ScoreText != null)
            player2ScoreText.text = $"Player 2's score : {_player2Score}";
        if (angryMeterText != null)
            angryMeterText.text = $"Monster's angry meter : {_monsterAngryMeter}";

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
}
