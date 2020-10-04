using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using TMPro;

public class StoreMenu : MonoBehaviour
{

    protected int currentCar;

    [SerializeField] protected int maxAdditionalCars = 7;

    [SerializeField] protected Button nextButton = null;
    [SerializeField] protected Button previousButton;

    [SerializeField] protected Color nonFocusedColor = Color.grey;
    [SerializeField] protected Color nonBoughtColor = Color.black;

    [SerializeField] protected int carPrice = 50;

    [SerializeField] protected string newCarSlot = "none";

    [SerializeField] protected TMP_Text moneyText = null;

    [System.Serializable]
    protected class FocusedCarUI {

        public string[] carNames = {"First Car (main)", "Second Car", "Third Car", "Fourth Car", "Fifth Car", "Sixth Car", "Seventh Car", "Eighth Car"};

        public TMP_Text carName;

        public Image carImage;
        public Image slotImage;

        public Image healthBarFill;

        public GameObject mainCarOptions;
        public GameObject additionalCarOptions;
        public GameObject buyCarOptions;

        public Button repairButton;
        public Button upgradeButton;
        public Button buyButton;

        public TMP_Text repairPriceText;
        public TMP_Text upgradePriceText;
        public TMP_Text buyPriceText;

        public GameObject mainCarGraphics;
        public GameObject additionalCarGraphics;


    }
    [SerializeField] FocusedCarUI focusedCarUI = new FocusedCarUI();

    [System.Serializable]
    protected class SideCarUI {

        public Image carImage;
        public Image slotImage;

        public GameObject mainCarGraphics;
        public GameObject additionalCarGraphics;

    }
    [SerializeField] SideCarUI leftCarUI = new SideCarUI();
    [SerializeField] SideCarUI rightCarUI = new SideCarUI();

    [SerializeField] protected List<CarUpgrade> carUpgrades = new List<CarUpgrade>();

    // Searchs for an upgrade on the list and returns it, returns null if the upgrade wasn't found.
    protected CarUpgrade GetUpgrade(string slotFitId) {

        foreach(CarUpgrade upgrade in carUpgrades) {
            if(upgrade.slotFitId == slotFitId)
                return upgrade;
        }

        return null;

    }

    // Searchs for an upgrade on the list and returns the next one, returns null if there's no upgrades or the current upgrade wasn't found.
    protected CarUpgrade GetNextUpgrade(string slotFitId) {

        for(int i = 0; i < carUpgrades.Count - 1; i++) {
            if(carUpgrades[i].slotFitId == slotFitId)
                return carUpgrades[i + 1];
        }

        return null;
    }

    public void Show() {

        currentCar = 1;
        gameObject.SetActive(true);

        UpdateUI();

    }

    public void Hide() {

        gameObject.SetActive(false);
        
    }

    public void Repair() {

        // Gets the price and spends the money.
        GameController.AdditionalCar car = GameController.Instance.GetAdditionalCar(currentCar - 1);
        int repairPrice = Mathf.RoundToInt(100 - car.health);
        GameController.Instance.Money -= repairPrice;

        // Repairs the car.
        GameController.Instance.RepairCar(currentCar - 1);

        UpdateUI();

    }

    public void Upgrade() {
        
        // Gets the upgrade to be made.
        GameController.AdditionalCar car = GameController.Instance.GetAdditionalCar(currentCar - 1);
        CarUpgrade upgrade = GetNextUpgrade(car.slotFitId);

        // No upgrades.
        if(upgrade == null)
            return;

        // Spends the money.
        GameController.Instance.Money -= upgrade.price;

        // Gives the upgrade.
        GameController.Instance.UpgradeCar(currentCar - 1, upgrade.slotFitId);

        UpdateUI();

    }

    public void Buy() {

        // Spends the money.
        GameController.Instance.Money -= carPrice;

        // Gives the car.
        GameController.Instance.AddCar(newCarSlot);

        UpdateUI();

    }

    public void NextCar() { 
        currentCar++; 
        UpdateUI();
    }

    public void PreviousCar() { 
        currentCar--; 
        UpdateUI();
    }

    protected void UpdateUI() {

        moneyText.text = GameController.Instance.Money.ToString();
        UpdateFocusedCarUI();
        UpdateLeftCarUI();
        UpdateRightCarUI();

    }

    protected void UpdateFocusedCarUI() {

        // Gets info on the current car.
        GameController.AdditionalCar car = GameController.Instance.GetAdditionalCar(currentCar - 1);

        // Sets the selected car UI.
        focusedCarUI.carName.text = focusedCarUI.carNames[currentCar];

        // Checks if the car has been bought. 
        bool boughtCar = (car != null);

        // Gets the current slot and upgrades.
        CarUpgrade currentSlot = (currentCar != 0 && car != null) ? GetUpgrade(car.slotFitId) : null;
        CarUpgrade upgrade = (currentCar != 0 && car != null) ? GetNextUpgrade(car.slotFitId) : null;

        Debug.Log("Focused: " + (car != null && currentSlot != null) + " " + ((currentSlot != null) ? currentSlot.slotFitId : "null"));

        // Gets prices.
        int repairPrice = (boughtCar) ? Mathf.RoundToInt(100 - car.health) : 0;
        int upgradePrice = (upgrade != null) ? upgrade.price : - 100;

        // Sets the store interaction buttons UI.
        focusedCarUI.repairButton.interactable  = (repairPrice > 0 && repairPrice <= GameController.Instance.Money);
        focusedCarUI.upgradeButton.interactable = (upgrade != null && upgradePrice <= GameController.Instance.Money);
        focusedCarUI.buyButton.interactable     = (carPrice <= GameController.Instance.Money);

        // Sets the prices and text colors.
        focusedCarUI.repairPriceText.text  = (repairPrice > 0) ? repairPrice + "$" : "---";
        focusedCarUI.upgradePriceText.text = (upgrade != null) ? upgradePrice + "$" : "---";
        focusedCarUI.buyPriceText.text     = carPrice + "$";;

        focusedCarUI.repairPriceText.color  = (repairPrice <= GameController.Instance.Money)    ? Color.green : Color.red;
        focusedCarUI.upgradePriceText.color = (upgradePrice <= GameController.Instance.Money)   ? Color.green : Color.red;
        focusedCarUI.buyPriceText .color    = (carPrice <= GameController.Instance.Money)       ? Color.green : Color.red;


        // For the main car.
        if(currentCar == 0) {

            focusedCarUI.carImage.color = Color.white;
            focusedCarUI.healthBarFill.rectTransform.localScale = Vector3.one;

            focusedCarUI.mainCarOptions.SetActive(true);
            focusedCarUI.additionalCarOptions.SetActive(false);
            focusedCarUI.buyCarOptions.SetActive(false);

            focusedCarUI.mainCarGraphics.SetActive(true);
            focusedCarUI.additionalCarGraphics.SetActive(false);

        // For the additional cars.
        } else {

            // Sets the health bar.
           
            focusedCarUI.healthBarFill.rectTransform.localScale = boughtCar ? new Vector3(car.health / 100.0f, 1, 1) : new Vector3(0, 1, 1);

            // Sets the UI.
            focusedCarUI.carImage.color  = boughtCar ? Color.white : nonBoughtColor;
            Color slotcolor = boughtCar ? Color.white : new Color(0, 0, 0, 0);
            if(currentSlot == null || currentSlot.slotFitId == "none")
                slotcolor.a = 0;
            Debug.Log("checks " + (boughtCar && currentSlot != null));
            focusedCarUI.slotImage.color = slotcolor;
            focusedCarUI.slotImage.sprite = (boughtCar && currentSlot != null) ? currentSlot.sprite : null;

            // Shows the correct car options.
            focusedCarUI.mainCarOptions.SetActive(false);
            focusedCarUI.additionalCarOptions.SetActive(boughtCar);
            focusedCarUI.buyCarOptions.SetActive(!boughtCar);

            // Shows the correct car graphics.
            focusedCarUI.mainCarGraphics.SetActive(false);
            focusedCarUI.additionalCarGraphics.SetActive(true);

        }

        // Enables/disables the next and previous buttons when necessary.
        nextButton.interactable = (currentCar < maxAdditionalCars && (currentCar == 0 || car != null));
        previousButton.interactable = (currentCar != 0);

    }

    // Wrappers
    protected void UpdateLeftCarUI() { UpdateSideCar(1); }
    protected void UpdateRightCarUI() { UpdateSideCar(-1); }

    protected void UpdateSideCar(int offset) {

        // Gets info on the current car.
        GameController.AdditionalCar car = GameController.Instance.GetAdditionalCar(currentCar + offset - 1);

        // Gets the components for the correct car.

        Image carImage = (offset == 1) ? leftCarUI.carImage : rightCarUI.carImage;
        Image slotImage = (offset == 1) ? leftCarUI.slotImage : rightCarUI.slotImage;
        GameObject mainCarGraphics  = (offset == 1) ?  leftCarUI.mainCarGraphics : rightCarUI.mainCarGraphics;
        GameObject additionalCarGraphics  = (offset == 1) ?  leftCarUI.additionalCarGraphics : rightCarUI.additionalCarGraphics;

        // For the main car.
        if(currentCar + offset == 0) {

            carImage.color = nonFocusedColor;

            // Shows the correct car graphics.
            mainCarGraphics.SetActive(true);
            additionalCarGraphics.SetActive(false);

        // For the additional cars.
        } else {

            // Checks if the car has been bought and sets the upgrades available.
            bool boughtCar = (car != null);
            CarUpgrade currentSlot = (currentCar != 0 && car != null) ? GetUpgrade(car.slotFitId) : null;

            // Sets the UI.
            carImage.color  = boughtCar ? nonFocusedColor : nonBoughtColor;
            Color slotcolor = boughtCar ? nonFocusedColor : new Color(0, 0, 0, 0);
            if(currentSlot == null || currentSlot.slotFitId == "none")
                slotcolor.a = 0;
            slotImage.color = slotcolor;
            slotImage.sprite = (boughtCar && currentSlot != null) ? currentSlot.sprite : null;

            // Shows the correct car graphics and hides invalid cars.
            bool validCar = (currentCar + offset) >= 0 && (currentCar + offset) <= maxAdditionalCars;
            mainCarGraphics.SetActive(false && validCar);
            additionalCarGraphics.SetActive(true && validCar);

        }

    }

}
