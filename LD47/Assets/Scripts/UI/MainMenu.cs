using UnityEngine;

using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] protected List<GameObject> submenus;
    [SerializeField] protected GameObject initialSubMneu;


    public void Show() {

        // Sets the correct submenu at start.
        foreach(GameObject submenu in submenus)
            submenu.SetActive(false);
        initialSubMneu.SetActive(true);

        // Enables the menu.
        gameObject.SetActive(true);

    }

    public void Hide() {

        gameObject.SetActive(false);
        
    }

    // Quits the game.
    public void QuitGame() {
        Debug.Log("Application.Quit()");
        Application.Quit();
    }

}
