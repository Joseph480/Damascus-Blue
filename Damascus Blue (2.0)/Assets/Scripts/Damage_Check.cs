using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Check : MonoBehaviour
{
    public bool GivesFerocity, Breaks, DropLoot, DropRandomLoot;
    public int Health, LootDropIndex;
    public GameObject [] Loot;

    private Vector3 OffSet =  new Vector3 (0,0,0.2f);

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Projectile"){
            if (GivesFerocity){Player_Stats.Ferocity++; 
            Player_Stats.Ferocity = Mathf.Clamp(Player_Stats.Ferocity, 1, 200);}
            if (Breaks) {Health = Health - Mathf.CeilToInt((float)Player_Stats.Ferocity/8f); if (Health <= 0) Destruct();}
        }
    }
    void Destruct(){
        if (DropLoot){
            Instantiate(Loot[LootDropIndex], transform.position, Quaternion.identity);
        }
        if (DropRandomLoot){
            int x = Random.Range(0,Loot.Length);
            Instantiate(Loot[x], transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}
