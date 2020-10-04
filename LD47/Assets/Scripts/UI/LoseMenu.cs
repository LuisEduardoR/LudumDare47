using UnityEngine;

using TMPro;

public class LoseMenu : MonoBehaviour
{
    
    [SerializeField] protected TMP_Text finalStats;


    public void Show() {

        gameObject.SetActive(true);

    }

    public void Hide() {

        gameObject.SetActive(false);
        
    }

    public void UpdateFinalStats(int points, int money) {

        // Calculates the total amount of money earned in the game (equal to points / 10).
        int totalMoney = points / 10;

        // Calculates the amount of money spent.
        int spentMoney = totalMoney - money;

        string finalText = "";
        finalText += "Total points: "       + points;
        finalText += "<br>Total money: "    + totalMoney + "$";
        finalText += "<br>Money spent: "    + spentMoney + "$";

        finalStats.text = finalText;

    }

}
