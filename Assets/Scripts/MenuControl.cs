using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MenuControl : MonoBehaviour
{
    public GameObject gameMenuPanel;

    public void Host()
    {
        NetworkingManager.Singleton.StartHost(); // can set spawn location if needed
        gameMenuPanel.SetActive(false);
    }

    public void Join()
    {
        NetworkingManager.Singleton.StartClient();
        gameMenuPanel.SetActive(false);
    }
}
