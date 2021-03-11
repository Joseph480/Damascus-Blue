using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Control : MonoBehaviour
{
    public Image HealthBar;

    private Player_Stats PlayerStats;

    void Start(){
        PlayerStats = FindObjectOfType<Player_Stats>();
    }
    void FixedUpdate(){
        UpdateHealthBar();
    }

    void UpdateHealthBar(){
        HealthBar.rectTransform.localScale = new Vector3(.01f*(float)PlayerStats.Health,1,1);
    }
}
