using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    // Singleton for this class.
    private static GameController instance;
    public static GameController Instance {
        get { return instance; }
    }

    // Controls the current state of the game.
    public enum GameState { MainMenu, Cutscene, Gameplay, Paused, Win, Lose, Store }
    protected GameState currentState;
    public GameState CurrentState {
        get { return currentState; }
    }

    // Controls the game UI
    [System.Serializable]
    public class UI {

        [Tooltip("Main menu's script")]
        public MainMenu mainMenu;

        [Tooltip("UI for the gameplay")]
        public GameObject gameplay;
        public TMP_Text pointsText;
        public TMP_Text moneyText;

        [Tooltip("Pause menu's script")]
        public PauseMenu pauseMenu;

        [Tooltip("Lose menu's script")]
        public LoseMenu loseMenu;

        [Tooltip("Win menu's script")]
        public WinMenu winMenu;

        [Tooltip("Store menu's script")]
        public StoreMenu storeMenu;

        // Updates the state of the UI based on GameState.
        public void Update(GameState state) {

            // Sets the main menu UI.
            if (state == GameState.MainMenu) 
                mainMenu.Show();
            else
                mainMenu.Hide();

            // Sets the gameplay UI.
            gameplay.SetActive(state == GameState.Gameplay);

            // Sets the pause menu UI.
            if (state == GameState.Paused) 
                pauseMenu.Show();
            else
                pauseMenu.Hide();

            // Sets the lose menu UI.
            if (state == GameState.Lose) 
                loseMenu.Show();
            else
                loseMenu.Hide();

            // Sets the win menu UI.
            if (state == GameState.Win) 
                winMenu.Show();
            else
                winMenu.Hide();

            // Sets the stpre menu UI.
            if (state == GameState.Store) 
                storeMenu.Show();
            else
                storeMenu.Hide();

        }

        public void UpdatePoints(int value) {
            pointsText.text = value.ToString().PadLeft(8, '0');
        }

        public void UpdateMoney(int value) {
            moneyText.text = value.ToString() + "$";
        }

        public void UpdateLoseScreenStats(int points, int money) {
            loseMenu.UpdateFinalStats(points, money);
        }

        public void UpdateWinScreenStats(int oldPoints, int curPoints, int oldMoney, int curMoney, int level) {
            winMenu.UpdateFinalStats(oldPoints, curPoints, oldMoney, curMoney, level);
        }

    }
    [Header("UI")]
    public UI ui;

    [System.Serializable]
    public class AdditionalCar {

        public string slotFitId;
        public float health;

        public AdditionalCar(string slotFitId, float health) {
            this.slotFitId = slotFitId;
            this.health = health;
        }

    }

    [Header("Cars & Slots")]    

    [Tooltip("Prefab for the main car")]
    public GameObject mainCarPrefab;

    [Tooltip("Prefab for additional cars")]
    public GameObject additionalCarPrefab;

    [Tooltip("Slot gameobjects avaliable to be fitted on cars")]
    public List<SlotFitInfo> slotFits;

    // Searchs for an SlotFitInfo on the list and returns it, returns null if it wasn't found.
    public SlotFitInfo GetSlotFit(string slotFitId) {

        foreach(SlotFitInfo fit in slotFits) {
            if(fit.id == slotFitId)
                return fit;
        }

        return null;

    }

    // Searchs for an SlotFitInfo on the list and returns the next one, returns null if there's no upgrades or the current SlotFitInfo wasn't found.
    public SlotFitInfo GetSlotFitUpgrade(string slotFitId) {

        for(int i = 0; i < slotFits.Count - 1; i++) {
            if(slotFits[i].id == slotFitId)
                return slotFits[i + 1];
        }

        return null;
    }

    // Current additional cars info.
    private List<AdditionalCar> additionalCars;

    // Used to get references to additional car's info.
    public AdditionalCar GetAdditionalCar(int index) {
        if(additionalCars == null)
            return null;
        if(index >= additionalCars.Count || index < 0)
            return null;
        return additionalCars[index];
    }

    // List of references to the cars used on the gameplay.
    protected List<Car> gameplayCars;

    protected Dictionary<string, SlotFitInfo> slotDictionary;

    [Header("Waves")]

    [Tooltip("Assign each level waves (in order)")]
    public List<LevelWaves> levels;

    [Header("Points & Money")]

    [SerializeField] protected int points;
    public int Points {
        get { return points; }
        set {
            points = value;
            ui.UpdatePoints(value);
        }
    }
    private int oldPoints;

    [SerializeField] protected int money;
    public int Money {
        get { return money; }
        set {
            money = value;
            ui.UpdateMoney(value);
        }
    }
    private int oldMoney;

    // Level
    protected int currentLevel;

    public int GetCurrentLevel() { return currentLevel; }

    void Start()
    {
        
        // Creates the singleton.
        if(Instance != null) {
            Debug.LogWarning("More than one instance of singleton found!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Marks this object to persist through loads.
        DontDestroyOnLoad(gameObject);        

        // Assigns the function to be called when laoding a scene.
        SceneManager.sceneLoaded += OnLoadScene;

        // Creates the dictionary for slot fits.
        slotDictionary = new Dictionary<string, SlotFitInfo>();
        foreach(SlotFitInfo fit in slotFits) {
            if(slotDictionary.ContainsKey(fit.id))
                Debug.LogError("Duplicated SlotFitInfo ID!");
            else {
                slotDictionary.Add(fit.id, fit);
            }
        }

        // Creates the additional car list.
        additionalCars = new List<AdditionalCar>();
        
        ResetGame();

    }

    void Update() {

        if(currentState == GameState.Gameplay || currentState == GameState.Paused) {
            if(Input.GetButtonDown("Pause"))
                TogglePause();
        }

    }

    // Executes code when a scene is loaded.
    public void OnLoadScene(Scene scene, LoadSceneMode mode) {

        switch(scene.name) {
            case "MainMenu":
                ResetGame();
                break;
            case "Gameplay":
                StartLevel();
                break;
            case "Store":
                EnterStore();
                break;
        }

    }

    // Resets the game to an initial state.
    public void ResetGame() {

        // Resets level, points and money.
        currentLevel = 1;
        Points = 0;
        Money = 0;

        // Clears additional cars.
        additionalCars.Clear();

        // Resets time and sets the UI.
        Time.timeScale = 1;
        currentState = GameState.MainMenu;
        ui.Update(currentState);

    }

    // Loads a new scene.
    public void LoadScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // Starts a gameplay level.
    public void StartLevel() {

        CreateTrain();

        // Resets time and sets the UI.
        Time.timeScale = 1;
        currentState = GameState.Gameplay;
        ui.Update(currentState);
        
        // Saves points and money
        oldPoints = Points;
        oldMoney = Money;

        // Initializes the enemy spawner.
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        int currentLevelWave = Mathf.Clamp(currentLevel - 1, 0, levels.Count - 1);
        spawner.Initialize(levels[currentLevelWave]);

    }

    // Creates the train.
    protected void CreateTrain() {

        // Initialzies the list of cars used on gameplay.
        gameplayCars = new List<Car>();

        // Intantiates the main car and adds it to the list.
        GameObject previousCar = Instantiate(mainCarPrefab, Vector3.zero, new Quaternion());
        Car previousCarScript = previousCar.GetComponent<Car>();
        gameplayCars.Add(previousCarScript);
        previousCar.transform.name = "car_0";

        // Instantiates and fits the slots of the remaining cars.
        for(int i = 0; i < additionalCars.Count; i++) {

            // Creates a new car and adds it to the list.
            GameObject currentCar = Instantiate(additionalCarPrefab, Vector3.zero, new Quaternion());
            Car currentCarScript = currentCar.GetComponent<Car>();     
            gameplayCars.Add(currentCarScript);

            // Assigns the order of cars on the train.
            currentCarScript.previousCar = previousCarScript;
            previousCarScript.nextCar = currentCarScript;

            // Positions and names the car.
            currentCarScript.Angle = previousCarScript.Angle + currentCarScript.followAngle;
            currentCar.transform.name = "car_" + (i + 1);

            // Creates and assigns the slot.
            string slotFitId = additionalCars[i].slotFitId;
            if(slotFitId != null && slotFitId.Length > 0 && slotFitId != "none") {
                GameObject slotObject = Instantiate(slotDictionary[slotFitId].prefab, currentCarScript.slotSpawn);
                currentCarScript.slot = slotObject.GetComponent<Slot>();
            }

            // Assigns the car health.
            currentCarScript.Health = additionalCars[i].health;

            // Assigns the new previous car to be the current pne
            previousCar = currentCar;
            previousCarScript = currentCarScript;

        }

    }

    public void TogglePause() {

        // Pauses or unpauses the game.
        if(currentState == GameState.Gameplay)
            currentState = GameState.Paused;
        else if(currentState == GameState.Paused)
            currentState = GameState.Gameplay;
        else
            return;

        Time.timeScale = (currentState == GameState.Paused) ? 0 : 1;
        ui.Update(currentState);

    }

    public void LoseGame() {

        // Loses the game.
        currentState = GameState.Lose;
        ui.gameplay.SetActive(false);
        StartCoroutine(LosingGame());

    }

    protected IEnumerator LosingGame() {

        // Waits for some time before displaying the screen.
        yield return new WaitForSeconds(2.5f);

        DisplayLoseScreen();

    }

    protected void DisplayLoseScreen() {

        Time.timeScale = 0;
        ui.UpdateLoseScreenStats(Points, Money);
        ui.Update(currentState);

    }

    public void WinLevel() {

        // Checks if the game was won.
        StartCoroutine(WinningGame());

    }

    protected IEnumerator WinningGame() {

        bool ended = false;
        bool won = true;

        while(!ended) {

            // Waits for delay.
            yield return new WaitForSeconds(2.5f);

            // Ensures the game wasn't lost.
            if(currentState == GameState.Lose) {
                ended = true;
                won = false;
                continue;
            }

            // Checks if all enemies have been defeated.
            BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
            if(enemies.Length <= 0 && currentState == GameState.Gameplay) {
                ended = true;
                won = true;
                continue;
            }

        }

        // Displays the win screen if the player won.
        if(won)
            DisplayWinScreen();

    }

    protected void DisplayWinScreen() {

        // Updates game.
        Time.timeScale = 0;
        currentState = GameState.Win;

        // Saves info about the train when the game ended.
        // Counts how many cars are alive.
        int numCarsRemaining = 0;
        List<string> slotIds = new List<string>(); // Stores the Ids of the slots.
        for(int i = 1; i < gameplayCars.Count; i++) {
            if(gameplayCars[i] != null && gameplayCars[i].Health > 0) {
                numCarsRemaining++;
                slotIds.Add(additionalCars[i - 1].slotFitId); // Gets the slot ID for this car on the old list.
            } else // No need to continue, if a car is dead all remaining ones are too.
                break;
        }
        // Resets additionalCars list with the new info.
        additionalCars.Clear();
        for(int i = 1; i <= numCarsRemaining; i++) {
            additionalCars.Add(new AdditionalCar(slotIds[i - 1], gameplayCars[i].Health));
        }

        // Sets the UI.
        ui.UpdateWinScreenStats(oldPoints, Points, oldMoney, Money, currentLevel);
        ui.Update(currentState);

        // Increases the level.
        currentLevel++;

    }

    public void EnterStore() {
        currentState = GameState.Store;
        ui.Update(currentState);
    }

    public void AddCar(string carSlotId) {
        additionalCars.Add(new AdditionalCar(carSlotId, 100.0f));
    }

    public void RepairCar(int index) {
        additionalCars[index].health = 100.0f;
    }

    public void UpgradeCar(int index, string carSlotId) {
        additionalCars[index].slotFitId = carSlotId;
    }

}
