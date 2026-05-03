using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers major lifecycle events,
/// e.g., starting a run, opening a lootbox
/// 
/// All Managers are registered with this class on their Awake()
/// and receive callbacks throught the game's flow
/// 
/// Manager's are attached to child gameObjects of this object
/// but this isn't required
/// </summary>

public class GameManager : MonoBehaviour
{
    private static HashSet<IManager> registeredManagers = new HashSet<IManager>();

    public static bool CanInteract { get; private set; } //Used to disable/enable interaction during things like menu animations, pauses
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        StartRun();
    }

    private void StartRun()
    {
        foreach (var manager in registeredManagers)
            manager.OnStartRun();

        GameScreenManager.Instance.ShowGameScreen();
    }

    public void StartNewRound()
    {
        foreach (var manager in registeredManagers)
            manager.OnStartRound();
        CanInteract = true;
    }

    public void EndRun(bool success)
    {
        foreach (var manager in registeredManagers)
            manager.OnStartRound();
    }

    public void ProcessOpenLootbox(Lootbox lootbox)
    {
        //Add each item that was in the lootbox to the player's inventory
            //TO DO
        //Register the lootbox as being opened
            //TO DO
        //Progress the timers forward for all lootboxes
            //TO DO
        //Tell the GameplayDisplayManager to show the open sequence


        foreach (var manager in registeredManagers)
            manager.OnLootboxOpened();
    }

    public static void RegisterManager(IManager manager)
    {
        //All Manager objects register themselves on their own Awake()
        registeredManagers.Add(manager);
    }

    public static void UnregisterManager(IManager manager)
    {
        registeredManagers.Remove(manager);
    }

    public static void SetCanInteract(bool newState)
    {
        CanInteract = newState;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            registeredManagers.Clear();
        }
    }

}

public interface IManager
{
    void OnStartRun();
    void OnStartRound();
    void OnLootboxOpened();
    void Setup(); //Called in Awake of each individual Manager
    void OnEndRun(bool success);
}