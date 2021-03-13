using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    public float MoveSpeed, LookSpeedChase, LookSpeedIdel, IdelPathTime, SightLength, Alertness;
    public bool Idel, HasPath; // Send idel to private after tests.
    //Create points for path. Draw line during editor time from [i] to [i++] foreach();
    public Vector3[] Path;

    private Vector3 Origin, RayOriginOffset, RayAlertOffset, Target;
    private Quaternion IdelDir, TargetDir;
    private Player_Control Player;
    private float PlayerDistance;
    private Rigidbody RB;
    private Ray Vision;
    private RaycastHit Hit; 
    private IEnumerator IdelRoutine, AlertRoutine;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        AlertRoutine = Lingering();
        IdelRoutine = NewDirection(); 
        StartCoroutine(IdelRoutine);
        //Reset as coroutine.
        RandomDetect();
        Idel = true;
    }
    void FixedUpdate()
    {
        Detect();
        if (HasPath){
            //FollowPath.

            //Never Walk idel so end with return.
            return;
        }
        if (Idel) WalkIdel(); else Follow(); 
    }
    void OnTriggerEnter (Collider other)
    {
        switch (other.tag)
        {
            //add tag name for projectiles, so enemy gets alert upon damage.
            case "": Idel = false; break;
            default: break;
        }
    }
    void Detect()
    {
        Vision.origin = transform.position + RayOriginOffset;
        if (Idel) Vision.direction = transform.forward; 
        else {
            RayAlertOffset = Player.transform.position - transform.position;
            RayAlertOffset.x = transform.forward.x; RayAlertOffset.z = transform.forward.z;
            Vision.direction = RayAlertOffset;
        }
        if (Physics.Raycast(Vision, out Hit, SightLength))
        {
            Debug.DrawRay(Vision.origin, Vision.direction * SightLength, Color.red);
            if (Hit.collider.tag == "Player") Idel = false; StopCoroutine(IdelRoutine);
            if (!Idel && Hit.collider.tag != "Player") Idel = true; StartCoroutine(AlertRoutine);
        }
    }
    void Follow()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed/50);
        Target = new Vector3 (Player.transform.position.x, transform.position.y, Player.transform.position.z);
        TargetDir = Quaternion.LookRotation(Target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetDir, LookSpeedChase * Time.deltaTime);
    }
    void WalkIdel()
    {
        transform.position += transform.forward * (MoveSpeed/100);
        transform.rotation = Quaternion.Slerp(transform.rotation, IdelDir, LookSpeedIdel * Time.deltaTime);
    }
    IEnumerator NewDirection(){
        IdelDir = Quaternion.Euler(0,Random.Range(-180,180),0);
        yield return new WaitForSeconds(IdelPathTime);
        IdelRoutine = NewDirection(); StartCoroutine(IdelRoutine);
    }
    IEnumerator Lingering(){
        yield return new WaitForSeconds(IdelPathTime * IdelPathTime);
        if (Idel){
            if (HasPath){/* Send monster back to following path*/} 
            else{IdelRoutine = NewDirection(); StartCoroutine(IdelRoutine);}
            StopCoroutine(AlertRoutine);
        } 
        else {AlertRoutine = Lingering(); StartCoroutine(AlertRoutine);}
    }
    //Change to IEnumerator and use IDelPathTime (as a float) divided by 2
    void RandomDetect()
    {
        float i = 0;
        PlayerDistance = Vector3.Distance(Player.transform.position, transform.position);
        i = Random.Range(0f, (float)PlayerDistance/Alertness);
        if (i < 0.3f) {Idel = false;}
        Invoke("RandomDetect", (float)IdelPathTime/2);
    }
}
