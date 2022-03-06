using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Header("Player Settings")]
    // Reset Sensitivity
    public Text resetSensitivityText_def;
    public Text resetSensitivityText_cur;
    public InputField resetSensitivityIF;
    // Stabilise Sensitivity
    public Text stabliseSensitivityText_def;
    public Text stabliseSensitivityText_cur;
    public InputField stabliseSensitivityIF;
    // Rotation Sensitivity
    public Text rotSensitivityText_def;
    public Text rotSensitivityText_cur;
    public InputField rotSensitivityIF;
    //Damage
    public Text dmgText_def;
    public Text dmgText_cur;
    public InputField dmgIF;

    [Header("Targets Settings")]
    public Text spawnTimeText_def;
    public Text spawnTimeText_cur;
    public InputField spawnTimeIF;

    [Header("Game Settings")]
    public Text timerText_def;
    public Text timerText_cur;
    public InputField timerIF;

    [Header("XR")]
    public Text camHeightText_def;
    public Text camHeightText_cur;
    public InputField camHeightIF;

    // Start is called before the first frame update
    void Start()
    {
        Player.TurretController player = Player.TurretController.instance;
        DestructionManager targetManager = DestructionManager.instance;
        ScoreManager gameManager = ScoreManager.instance;
        resetSensitivityText_def.text = player.m_resetSensitivity.ToString();
        resetSensitivityText_cur.text = player.m_resetSensitivity.ToString();
        stabliseSensitivityText_def.text = player.m_stabiliseSensitivity.ToString();
        stabliseSensitivityText_cur.text = player.m_stabiliseSensitivity.ToString();
        rotSensitivityText_def.text = player.sensitivity.ToString();
        rotSensitivityText_cur.text = player.sensitivity.ToString();
        spawnTimeText_def.text = targetManager.deadTime.ToString();
        spawnTimeText_cur.text = targetManager.deadTime.ToString();
        timerText_def.text = gameManager.countdownTimer.ToString();
        timerText_cur.text = gameManager.countdownTimer.ToString();
        dmgText_def.text = player.damage.ToString();
        dmgText_cur.text = player.damage.ToString();
        camHeightText_def.text = gameManager.m_camHeight.ToString();
        camHeightText_cur.text = gameManager.m_camHeight.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplySettings(string _variableName)
    {
        switch (_variableName)
        {
            case "ResetSensitivity":
                resetSensitivityText_cur.text = resetSensitivityIF.text;
                Player.TurretController.instance.m_resetSensitivity = float.Parse(resetSensitivityIF.text);
                break;
            case "StabiliseSensitivity":
                stabliseSensitivityText_cur.text = stabliseSensitivityIF.text;
                Player.TurretController.instance.m_stabiliseSensitivity = float.Parse(stabliseSensitivityIF.text);
                break;
            case "RotSensitivity":
                rotSensitivityText_cur.text = rotSensitivityIF.text;
                Player.TurretController.instance.sensitivity = float.Parse(rotSensitivityIF.text);
                break;
            case "SpawnTime":
                spawnTimeText_cur.text = spawnTimeIF.text;
                DestructionManager.instance.deadTime = float.Parse(spawnTimeIF.text);
                break;
            case "GameTimer":
                timerText_cur.text = timerIF.text;
                ScoreManager.instance.countdownTimer = float.Parse(timerIF.text);
                break;
            case "WeaponDamage":
                dmgText_cur.text = dmgIF.text;
                Player.TurretController.instance.damage = float.Parse(dmgIF.text);
                break;
            case "CamHeight":
                camHeightText_cur.text = camHeightIF.text;
                ScoreManager.instance.m_camHeight = float.Parse(camHeightIF.text);
                ScoreManager.instance.recenterPlease();
                break;
        }
    }

    public void ChangeMusic(int index)
    {

    }
}
