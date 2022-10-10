using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;

    public GameObject towerPanel, resourcePanel, preparationMenuPanel, menuPanel, gameOverPanel;

    [SerializeField]
    public Lis<FieldInfo> fi = new List<FieldInfo>(
        typeof(Projectile).GetFields(BindingFlags.Public | BindingFlags.Instance));
    public void Awake() {

        // set children active (simple way to keep the view clean, but activate them at runtime so the calls below aren't null)
        foreach (Transform child in transform) {

            child.gameObject.SetActive(true); // or false
            
            foreach (Transform child2 in child) {

                child2.gameObject.SetActive(true); // or false

            }
        }

        uiManager = this;
        towerPanel = gameObject.GetComponentInChildren<TowerPanel>().gameObject;
        resourcePanel = gameObject.GetComponentInChildren<ResourcePanel>().gameObject;
        preparationMenuPanel = gameObject.GetComponentInChildren<PreparationMenuPanel>().gameObject;
        menuPanel = gameObject.GetComponentInChildren<MenuPanel>().gameObject;
        gameOverPanel = gameObject.GetComponentInChildren<GameOverPanel>().gameObject;

    }

    public void Start() {
        GameControl.control.OnGameStateChanged += OnGameStateChanged;
    }

    public void OnDestroy() {
        GameControl.control.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state) {

        towerPanel.SetActive(state == GameState.Preparation || state == GameState.Combat);

        resourcePanel.SetActive(state == GameState.Preparation || state == GameState.Combat);

        // todo: consider removing prep menu since it feels redundant
        preparationMenuPanel.SetActive(state == GameState.Preparation);

        menuPanel.SetActive(state == GameState.Menu);

        gameOverPanel.SetActive(state == GameState.Victory || state == GameState.Defeat);

        Debug.Log("imagine cool animations");

        //preparationMenuPanel.SetActive(state == GameState.Preparation);
        
    }

    public void OnPlay() {
        // highlight play button
        // todo
        

    }

    public void OnPause() {
        // highlight pause button
        // todo
    }

}
