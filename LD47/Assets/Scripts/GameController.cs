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

        }

        public void UpdatePoints(int value) {
            pointsText.text = value.ToString().PadLeft(8, '0');
        }

        public void UpdateMoney(int value) {
            moneyText.text = value.ToString();
        }

        public void UpdateFinalStats(int points, int money) {
            loseMenu.UpdateFinalStats(points, money);
        }

    }
    [Header("UI")]
    public UI ui;

    [System.Serializable]
    public struct AdditionalCar {

        public string slotFitId;
        public float health;

    }

    [Header("Cars & Slots")]    

    [Tooltip("Prefab for the main car")]
    public GameObject mainCarPrefab;

    [Tooltip("Prefab for additional cars")]
    public GameObject additionalCarPrefab;

    [Tooltip("Slot gameobjects avaliable to be fitted on cars")]
    public List<SlotFitInfo> slotFits;

    // Current additional cars.
    // TODO: make private after tests.
    public List<AdditionalCar> additionalCars;
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

    [SerializeField] protected int money;
    public int Money {
        get { return money; }
        set {
            money = value;
            ui.UpdateMoney(value);
        }
    }

    void Start()
    {
        
        // Creates the singleton.
        if(Instance != null)
            Debug.LogWarning("More than one instance of singleton found!");
        instance = this;

        // Marks this object to persist through loads.
        DontDestroyOnLoad(gameObject);        

        // Assigns the function to be called when laoding a scene.
        SceneManager.sceneLoaded += OnLoadScene;

        // Resets time.
        Time.timeScale = 1;

        // Creates the dictionary for slot fits.
        slotDictionary = new Dictionary<string, SlotFitInfo>();
        foreach(SlotFitInfo fit in slotFits) {
            if(slotDictionary.ContainsKey(fit.id))
                Debug.LogError("Duplicated SlotFitInfo ID!");
            else {
                slotDictionary.Add(fit.id, fit);
            }
        }

        currentState = GameState.MainMenu;
        ui.Update(currentState);

        // Initializes points and money.
        Points = 0;
        Money = 0;

        ResetGame();

    }

    // Executes code when a scene is loaded.
    public void OnLoadScene(Scene scene, LoadSceneMode mode) {

        switch(scene.name) {
            case "Gameplay":
                StartLevel();
                break;
            case "MainMenu":
                SceneManager.sceneLoaded -= OnLoadScene;
                Destroy(gameObject);
                break;
        }

    }

    // Resets the game to an initial state.
    public void ResetGame() {

        // Resets the list of cars.
        //additionalCars = new List<AdditionalCar>();
        // TODO: reset cars at start of the game

    }

    // Loads a new scene.
    public void LoadScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // Starts a gameplay level.
    public void StartLevel() {

        CreateTrain();

        currentState = GameState.Gameplay;
        ui.Update(currentState);
        
        // Initializes the enemy spawner.
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        // TODO: multiple levels
        spawner.Initialize(levels[0]);

    }

    // Creates the train.
    protected void CreateTrain() {

        // Intantiates the main car.
        GameObject previousCar = Instantiate(mainCarPrefab, Vector3.zero, new Quaternion());
        Car previousCarScript = previousCar.GetComponent<Car>();
        previousCar.transform.name = "car_0";

        // Instantiates and fits the slots of the remaining cars.
        for(int i = 0; i < additionalCars.Count; i++) {

            // Creates a new car.
            GameObject currentCar = Instantiate(additionalCarPrefab, Vector3.zero, new Quaternion());
            Car currentCarScript = currentCar.GetComponent<Car>();     

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

        // Waits for some time before displaying the scree.
        yield return new WaitForSeconds(2.5f);

        DisplayLoseScreen();

    }

    protected void DisplayLoseScreen() {

        Time.timeScale = 0;
        ui.UpdateFinalStats(Points, Money);
        ui.Update(currentState);

    }

}
