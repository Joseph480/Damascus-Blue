using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Script : MonoBehaviour
{
    
    public GameObject Target;
    public bool Movement, Activation, Destruction;
    public Vector3 Direction;
    public float Distance,MoveSpeed;

    private Vector3 Origin, Destination;
    private bool Acting, Finished;

    void Start(){
        if (Movement){
            Origin = Target.transform.position;
            Direction = Direction.normalized;
            Destination = Origin + Direction * Distance;
        }
    }
    void Update(){
        if (Target == null) return;
        if (!Acting)return;
        if (Movement)Move();
    }
    void Move(){
        if (Target.transform.position == Destination){Movement = false; return;}
        Target.transform.position = Vector3.Lerp(Target.transform.position, Destination, MoveSpeed);
    }
    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Player"){
            Acting = true;
            transform.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}
