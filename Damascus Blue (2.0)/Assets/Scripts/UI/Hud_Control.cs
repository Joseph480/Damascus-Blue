using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Control : MonoBehaviour
{
    public Image HealthBar, CrossHair;
    public Text FCounter;
    public GameObject PauseText;

    private Player_Stats PlayerStats;
    private Player_Control Player;
    private Damascus_Control Gun;

    void Start(){
        PlayerStats = FindObjectOfType<Player_Stats>();
        Player = PlayerStats.GetComponent<Player_Control>();
        Gun = FindObjectOfType<Damascus_Control>();
    }
    void LateUpdate(){
        ActivatePauseText();
    }
    void FixedUpdate(){
        if (Player.Paused) return;
        UpdateHealthBar();
        UpdateFCounter();
        AltCrossHair();
    }
    void AltCrossHair(){
        if (Gun.Clutching){
            CrossHair.color = new Vector4(0,255,255,0.5f);
            CrossHair.rectTransform.localScale = Vector3.Lerp(CrossHair.rectTransform.localScale, new Vector3(0.8f,0.8f,1f),0.08f);
        } else {
            CrossHair.color = Vector4.Lerp(CrossHair.color,new Vector4(0,255,255,0),0.12f);
            CrossHair.rectTransform.localScale = new Vector3(1,1,1);
        }
    }
    void ActivatePauseText(){
        if (Player.Paused) PauseText.SetActive(true);
        else PauseText.SetActive(false);
    }
    void UpdateHealthBar(){
        HealthBar.fillAmount = 0.01f*(float)PlayerStats.Health;
    }
    void UpdateFCounter(){
        FCounter.text = Player_Stats.Ferocity.ToString();
    }
}
