using TMPro;
using UnityEngine;

public class contadorMonedas : MonoBehaviour
{
    public TMP_Text coinText;

    private int totalCoins = 0;
    private int collectedCoins = 0;

    private void Start()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("monedas");
        totalCoins = coins.Length;
        UpdateCoinText();
    }

    public void CoinColleted()
    {
        collectedCoins++;
        if (collectedCoins == totalCoins)
        {
            LoadNextLevel();
        }
        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        coinText.text = collectedCoins + "/" + totalCoins;
    }

    void LoadNextLevel()
    {

    }
}
