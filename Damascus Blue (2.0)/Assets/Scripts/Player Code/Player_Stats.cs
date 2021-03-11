using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    public int Health, Energy, Ferocity, FDrain; 
    public float HurtRate;

    private IEnumerator Coroutine, Coroutine2;
    private bool Hurting;

    void Start(){
        Coroutine = DrainFerocity();
        Coroutine2 = Hurt();
        Health = 100;
        Energy = 100;
        Ferocity = 200;
        StartCoroutine(Coroutine);
    }

    void OnTriggerEnter(Collider other){
        switch(other.tag){
            case "Medkit": Health += 10; Health = Mathf.Clamp(Health,0,100); break;
            case "FSphere": Ferocity += 5; break;
            case "Hazzard": Hurting = true; Coroutine2 = Hurt(); StartCoroutine(Coroutine2); break;
            default: break;
        }
    }
    void OnTriggerExit(Collider other){
        switch(other.tag){
            case "Hazzard": Hurting = false; break;
            default: break;
        }
    }
    private IEnumerator Hurt(){
        if (Hurting){Health--; Health = Mathf.Clamp(Health,0,100);}
        else StopCoroutine(Coroutine2);
        yield return new WaitForSeconds(HurtRate);
        Coroutine2 = Hurt(); StartCoroutine(Coroutine2);
    }
    private IEnumerator DrainFerocity(){
        yield return new WaitForSeconds(FDrain/(Ferocity/2));
        Ferocity--;
        Ferocity = Mathf.Clamp(Ferocity, 0, 999);
        Debug.Log(Ferocity);
        Coroutine = DrainFerocity(); StartCoroutine(Coroutine);
    }
}
