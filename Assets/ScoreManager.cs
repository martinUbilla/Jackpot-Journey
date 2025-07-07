using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int currentScore = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreToWin = 1000; // Puntaje objetivo
    [SerializeField] private PlayerWin playerWin;   // Asignar en el inspector
    private int scoreDesdeUltimoDebuff = 0;
    private bool partidaFinalizada = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        if (partidaFinalizada) return; // 🚫 Ya se terminó la partida

        currentScore += amount;
        scoreDesdeUltimoDebuff += amount;
        scoreText.text = "Puntaje: " + currentScore.ToString();

        if (currentScore >= scoreToWin)
        {
            partidaFinalizada = true; // ✅ Marcamos que ya se terminó
            playerWin.Win();
            ChatManager.Instance?.FinalizarPartida();
        }

        if (scoreDesdeUltimoDebuff >= 300)
        {
            ChatManager.Instance?.EnviarSenalDebuff();
            scoreDesdeUltimoDebuff = 0;
        }
    }

}
