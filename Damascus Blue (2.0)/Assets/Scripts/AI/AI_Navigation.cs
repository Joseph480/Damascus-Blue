using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    public float MoveSpeed, LookSpeedChase, LookSpeedIdel, IdelPathTime, SightLength, Alertness;
    public bool HasPath;
    //Create points for path. Draw line during editor time from [i] to [i++] foreach();
    public Vector3[] Path;

    private Vector3 Origin, RayOriginOffset, RayAlertOffset, Target;
    private Quaternion IdelDir, TargetDir;
    private Player_Control Player;
    private float PlayerDistance;
    private bool Sighting, EmptyRay, Idel;
    private Rigidbody RB;
    private Ray Vision;
    private RaycastHit Hit; 
    private IEnumerator IdelRoutine, AlertRoutine, RandomRoutine;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        if (!HasPath){IdelRoutine = NewDirection(); StartCoroutine(IdelRoutine);}
        AlertRoutine = Lingering(); StartCoroutine(AlertRoutine);
        RandomRoutine = RandomDetect(); StartCoroutine(RandomRoutine);
        Idel = true;
    }
    void Update(){
        Detect();
    }
    void FixedUpdate(){
        if (HasPath){
            //FollowPath.
            //Never Walk idel so end with return.
            return;
        }
        if (Idel) WalkIdel(); else Follow(); 
    }
    void OnTriggerEnter (Collider other){
        switch (other.tag){
            //add tag name for projectiles, so enemy gets alert upon damage.
            case "": Idel = false; break;
            default: break;
        }
    }
    void Detect(){
        Vision.origin = transform.position + RayOriginOffset;
        if (Idel) Vision.direction = transform.forward; 
        else {
            RayAlertOffset = Player.transform.position - transform.position;
            RayAlertOffset.x = transform.forward.x; RayAlertOffset.z = transform.forward.z;
            Vision.direction = RayAlertOffset;
        }
        if (Physics.Raycast(Vision, out Hit, SightLength))
        {
            if (Hit.collider.tag == "Player"){ 
                Idel = false; Sighting = true;
            }
            if (!Idel && (Hit.collider.tag != "Player" || EmptyRay)){ 
                Sighting = false;
            }
            EmptyRay = false;
        } else EmptyRay = true;
    }
    void Follow(){
        RB.MovePosition(Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed/50));
        //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed/50);
        Target = new Vector3 (Player.transform.position.x, transform.position.y, Player.transform.position.z);
        TargetDir = Quaternion.LookRotation(Target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetDir, LookSpeedChase * Time.deltaTime);
    }
    void WalkIdel(){
        RB.MovePosition(Vector3.Lerp(transform.position, transform.position + transform.forward,(MoveSpeed/100)));
        transform.rotation = Quaternion.Slerp(transform.rotation, IdelDir, LookSpeedIdel * Time.deltaTime);
    }
    IEnumerator NewDirection(){
        IdelDir = Quaternion.Euler(0,Random.Range(-180,180),0);
        yield return new WaitForSeconds(IdelPathTime);
        IdelRoutine = NewDirection(); StartCoroutine(IdelRoutine);
    }
    IEnumerator Lingering(){
        yield return new WaitForSeconds(IdelPathTime * IdelPathTime);
        if (!Sighting && !Idel) Idel = true;
        AlertRoutine = Lingering(); StartCoroutine(AlertRoutine);
    }
    IEnumerator RandomDetect(){
        float i = 0;
        PlayerDistance = Vector3.Distance(Player.transform.position, transform.position);
        i = Random.Range(0f, (float)PlayerDistance/Alertness); if (i < 0.3f) Idel = false;
        yield return new WaitForSeconds((float)IdelPathTime/2);
        RandomRoutine = RandomDetect(); StartCoroutine(RandomRoutine);
    }
}

/*      NOTE SECTION

Okay, use 4 or 8 rays at base of A.I; if forward ray is hitting something that is not the player. Then the 
A.I should start moving in the direction of ray that is both not hitting something and whose length
is closest to player.

Maybe alertness should rely on dedicated ray?

*/