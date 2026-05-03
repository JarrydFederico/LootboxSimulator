using UnityEngine;

/// <summary>
/// A Manager is used for each area of functionality
/// e.g., menus, lootbox processing, HUD
/// 
/// They are registered with the GameManager on Awake()
/// and are usually on a gameObject that is a child of the GameManager
/// but it can be placed anywhere in the scene
/// </summary>

public abstract class Manager<T> : MonoBehaviour, IManager where T : MonoBehaviour
{
    //Managers with frequent calls are in this class, they are subordinate to the GameManager
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        //Register this with the GameManager. Destroy self if there is a duplicate
        if (Instance == null)
            Instance = this as T;
        else
        {
            Destroy(gameObject);
            return;
        }

        GameManager.RegisterManager(this);

        Setup();
    }

    public virtual void OnStartRun() { }
    public virtual void OnStartRound() { }
    public virtual void OnLootboxOpened(Lootbox lootbox) { }
    public virtual void OnEndRun(bool success) { }
    public virtual void Setup() { }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            GameManager.UnregisterManager(this);
        }
    }
}
