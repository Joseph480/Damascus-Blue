using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    public int Health, Energy, Ferocity, FDrain;

    private IEnumerator Coroutine;

    void Start(){
        Coroutine = DrainFerocity();
        Health = 100;
        Energy = 100;
        Ferocity = 200;
        StartCoroutine(Coroutine);
    }

    void OnTriggerEnter(Collider other){
        switch(other.tag){
            case "Medkit": Health += 10; Health = Mathf.Clamp(Health,0,100); break;
            case "FSphere": Ferocity += 5; break;
            default: break;
        }
    }

    private IEnumerator DrainFerocity(){
        yield return new WaitForSeconds(FDrain/(Ferocity/2));
        Ferocity--;
        Ferocity = Mathf.Clamp(Ferocity, 0, 999);
        Debug.Log(Ferocity);
        Coroutine = DrainFerocity(); StartCoroutine(Coroutine);
    }
}
