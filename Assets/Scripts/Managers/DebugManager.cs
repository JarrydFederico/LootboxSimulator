using UnityEngine;

/// <summary>
/// All debug controls are centralised here.
/// Will only do debug things if debugMode is set to on
/// </summary>

public class DebugManager : Manager<DebugManager>
{
    [SerializeField] private bool debugMode;
    public bool DebugMode => debugMode;

    public void Update()
    {
        if (!debugMode)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ItemDataManager.Instance.ProcessItemInfos();
        }
    }

}
