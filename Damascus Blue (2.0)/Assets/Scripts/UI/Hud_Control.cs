using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Control : MonoBehaviour
{
    public Image HealthBar;
    public Text FCounter;

    private Player_Stats PlayerStats;
    private Player_Control Player;

    void Start(){
        PlayerStats = FindObjectOfType<Player_Stats>();
        Player = PlayerStats.GetComponent<Player_Control>();
    }
    void FixedUpdate(){
        if (Player.Paused) return;
        UpdateHealthBar();
        UpdateFCounter();
    }

    void UpdateHealthBar(){
        HealthBar.fillAmount = 0.01f*(float)PlayerStats.Health;
    }
    void UpdateFCounter(){
        FCounter.text = Player_Stats.Ferocity.ToString();
    }
}
