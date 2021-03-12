using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    public int Health;
    public static int Ferocity; 
    public float FDrain, HurtRate;

    private IEnumerator Coroutine, Coroutine2;
    private bool Hurting;

    void Start(){
        Coroutine = DrainFerocity();
        Coroutine2 = Hurt();
        Health = 100;
        Ferocity = 25;
        StartCoroutine(Coroutine);
    }
    void OnTriggerEnter(Collider other){
        switch(other.tag){
            case "MedKit": Health += 10; Health = Mathf.Clamp(Health,0,100); Destroy(other.transform.parent.gameObject); break;
            case "FSphere": Ferocity += 25; Ferocity = Mathf.Clamp(Ferocity,1,200);Destroy(other.transform.parent.gameObject); break;
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
        if (Hurting){Health--; Ferocity--; 
        Health = Mathf.Clamp(Health,0,100);Ferocity = Mathf.Clamp(Ferocity, 1, 200);}
        else StopCoroutine(Coroutine2);
        yield return new WaitForSeconds(HurtRate);
        Coroutine2 = Hurt(); StartCoroutine(Coroutine2);
    }
    private IEnumerator DrainFerocity(){
        yield return new WaitForSeconds(FDrain - FDrain/Ferocity);
        Ferocity--;
        Ferocity = Mathf.Clamp(Ferocity, 1, 200);
        Coroutine = DrainFerocity(); StartCoroutine(Coroutine);
    }
}
