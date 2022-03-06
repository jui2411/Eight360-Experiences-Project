using UnityEngine;

public class novaCFG : MonoBehaviour
{
    [SerializeField] NovaController controller;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("ipcfg")) { PlayerPrefs.SetString("ipcfg", "localhost"); }
        updateIP(PlayerPrefs.GetString("ipcfg","localhost"));

        if (!PlayerPrefs.HasKey("portcfg")) { PlayerPrefs.SetInt("portcfg", 28360); }
        updatePort(PlayerPrefs.GetInt("portcfg", 28360));
    }


    public void updateIP(string ip)
    {
        controller.novaIpAddress = ip;
    }

    public void updatePort(int port)
    {
        controller.novaPort = port;
    }
}
