using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruct : MonoBehaviour
{
    public float LifeSpan;

    void Start(){
        Invoke("SelfDestruct", LifeSpan);
    }

    void SelfDestruct(){
        Destroy(this.gameObject);
    }
}
