using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject BlastEffect;

    void OnCollisionEnter(Collision other){
        Instantiate(BlastEffect,transform.position, Quaternion.identity);
        Destroy(this.gameObject);
	}
}
