using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverText;
    public int score { get; private set; }
    public void ScoreSystem(int Count)
    {
        if (Count == 1) score += 40;
        else if (Count == 2) score += 100;
        else if (Count == 3) score += 300;
        else if (Count == 4) score += 1000;
    }

    public void GameOverUI(bool TurnOnUI)
    {
        gameOverText.gameObject.SetActive(TurnOnUI);
    }

    void Start()
    {   
        gameOverText.gameObject.SetActive(false);
    }
    void Update()
    {
        scoreText.text = "Score: " + score;
    }
}
