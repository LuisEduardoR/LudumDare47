using UnityEngine;

using TMPro;

public class WinMenu : MonoBehaviour
{
    
    [SerializeField] protected TMP_Text titleText = null;
    [SerializeField] protected TMP_Text finalLevelStats = null;


    public void Show() {

        gameObject.SetActive(true);

    }

    public void Hide() {

        gameObject.SetActive(false);
        
    }

    public void UpdateFinalStats(int oldPoints, int curPoints, int oldMoney, int curMoney, int level) {

        titleText.text = "Level " + level + " completed!";

        string winText = "";
        winText += "Points earned: "        + (curPoints - oldPoints);
        winText += "<br>Total points: "     + curPoints;
        winText += "<br>";
        winText += "<br>Money earned: "     + (curMoney - oldMoney);
        winText += "<br>Total money: "      + curMoney;

        finalLevelStats.text = winText;

    }

}
