using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class StoreMenu : MonoBehaviour
{

    protected int currentCar;

    [SerializeField] protected int maxAdditionalCars = 7;

    [SerializeField] protected Button nextButton = null;
    [SerializeField] protected Button previousButton;

    [SerializeField] protected Color nonFocusedColor = Color.grey;
    [SerializeField] protected Color nonBoughtColor = Color.black;

    [System.Serializable]
    protected class FocusedCarUI {

        public string[] carNames = {"First Car (main)", "Second Car", "Third Car", "Fourth Car", "Fifth Car", "Sixth Car", "Seventh Car", "Eighth Car"};

        public TMP_Text carName;

        public Image carImage;
        public Image slotImage;

        public Image healthBarFill;

        [HideInInspector] public Vector3 healthBarFillDefaultScale;

        public GameObject mainCarOptions;
        public GameObject additionalCarOptions;
        public GameObject buyCarOptions;

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

    public void Start() {

        // Gets the default helath bar scale.
        focusedCarUI.healthBarFillDefaultScale = focusedCarUI.healthBarFill.rectTransform.localScale;

    }

    public void Show() {

        currentCar = 1;
        gameObject.SetActive(true);

        UpdateCarUI();

    }

    public void Hide() {

        gameObject.SetActive(false);
        
    }

    public void NextCar() { 
        currentCar++; 
        UpdateCarUI();
    }

    public void PreviousCar() { 
        currentCar--; 
        UpdateCarUI();
    }

    protected void UpdateCarUI() {

        UpdateFocusedCarUI();
        UpdateLeftCar();
        UpdateRightCar();

    }

    protected void UpdateFocusedCarUI() {

        // Gets info on the current car.
        GameController.AdditionalCar car = GameController.Instance.GetAdditionalCar(currentCar - 1);

        // Sets the selected car UI.
        focusedCarUI.carName.text = focusedCarUI.carNames[currentCar];
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

            // Checks if the car has been bought. 
            bool boughtCar = (car != null);

            // Sets the health bar.
            Vector3 healthBarFillScale = focusedCarUI.healthBarFillDefaultScale;
            healthBarFillScale = boughtCar ? new Vector3(car.health / 100.0f, healthBarFillScale.x, healthBarFillScale.y) : new Vector3(0, healthBarFillScale.x, healthBarFillScale.y);
            focusedCarUI.healthBarFill.rectTransform.localScale = healthBarFillScale;

            // Sets the UI.
            focusedCarUI.carImage.color  = boughtCar ? Color.white : nonBoughtColor;
            focusedCarUI.slotImage.color = boughtCar ? Color.white : nonBoughtColor;

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
    protected void UpdateLeftCar() { UpdateSideCar(1); }
    protected void UpdateRightCar() { UpdateSideCar(-1); }

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

            // Checks if the car has been bought and sets the health bar.
            bool boughtCar = (car != null);

            // Sets the UI.
            carImage.color  = boughtCar ? nonFocusedColor : nonBoughtColor;
            slotImage.color = boughtCar ? nonFocusedColor : nonBoughtColor;

            // Shows the correct car graphics and hides invalid cars.
            bool validCar = (currentCar + offset) >= 0 && (currentCar + offset) < maxAdditionalCars;
            mainCarGraphics.SetActive(false && validCar);
            additionalCarGraphics.SetActive(true && validCar);

        }

    }

}
