using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Check : MonoBehaviour
{
    public bool GivesFerocity, Breaks;
    public int Health;

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Projectile"){
            if (GivesFerocity){ Player_Stats.Ferocity++; 
            Player_Stats.Ferocity = Mathf.Clamp(Player_Stats.Ferocity, 0, 200);}
            if (Breaks) Health = Health - Player_Stats.Ferocity/4;
        }
    }
}
