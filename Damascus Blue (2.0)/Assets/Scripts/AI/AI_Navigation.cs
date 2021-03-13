using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    public float MoveSpeed, LookSpeedChase, LookSpeedIdel, IdelPathTime, SightLength, Alertness;
    public bool HasPath;

    //Create points for path. Draw line during editor time from [i] to [i++] foreach();
    //public Vector3[] Path;
    //public Vector3 NextNode;

    private Vector3 Origin, RayOriginOffset, RayAlertOffset, Target;
    private Quaternion IdelDir, CurrentDir, TargetDir;
    private Player_Control Player;
    private float PlayerDistance;
    private bool Sighting, EmptyRay, Idel;
    private Rigidbody RB;
    private Ray Vision;
    private RaycastHit Hit; 

    //navigation variables
    public float NavLength;
    private Ray N, NE, E, SE, S, SW, W, NW;
    private RaycastHit HitN, HitNE, HitE, HitSE, HitS, HitSW, HitW, HitNW;
    private Vector3 PosN, PosNE, PosE, PosSE, PosS, PosSW, PosW, PosNW;
    private Vector3 DiagonalOffset;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        DiagonalOffset = new Vector3(0,45,0);
        //Cycles below
        if (!HasPath)InvokeRepeating("NewDirection",IdelPathTime,IdelPathTime);
        InvokeRepeating("Lingering", IdelPathTime*IdelPathTime,IdelPathTime*IdelPathTime);
        InvokeRepeating("RandomDetect",(float)IdelPathTime/2,(float)IdelPathTime/2);
        InvokeRepeating("Navigate",0.1f,0.1f);
        Idel = true;
    }
    void Update(){
        Detect();
    }
    void FixedUpdate(){
        if (HasPath){
            if (!Idel) Follow();
            // else FollowPath();
            //Never Walk idel so end with return.
            return;
        }
        if (Idel) WalkIdel(); else Follow(); 
    }
    void OnTriggerEnter (Collider other){
        switch (other.tag){
            case "Projectile": Idel = false; break;
            default: break;
        }
    }
    void Detect(){
        Vision.origin = transform.position + RayOriginOffset;
        if (Idel) Vision.direction = transform.forward; 
        else {
            RayAlertOffset = Player.transform.position - transform.position;
            RayAlertOffset.x = transform.forward.x; RayAlertOffset.z = transform.forward.z;
            Vision.direction = transform.forward + RayAlertOffset;
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
    void Navigate(){
        N.direction = transform.forward;
        NE.direction = transform.forward + DiagonalOffset;
        E.direction = transform.right;
        SE.direction = transform.right + DiagonalOffset;
        S.direction = -transform.forward;
        SW.direction = -transform.forward + DiagonalOffset;
        W.direction = -transform.right;
        NW.direction = -transform.right + DiagonalOffset;

        Debug.DrawRay(Origin,N.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,NE.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,E.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,SE.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,S.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,SW.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,W.direction * NavLength,Color.red);
        Debug.DrawRay(Origin,NW.direction * NavLength,Color.red);

        /*
        if (Physics.Raycast(N, out HitN, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(NE, out HitNE, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(E, out HitE, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(SE, out HitSE, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(S, out HitS, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(SW, out HitSW, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(W, out HitW, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        if (Physics.Raycast(NW, out HitNW, NavLength)){    
            if(Hit.collider.tag != "Player" && Hit.collider.tag != "Enemy"){    }
        }
        */
    }
    void Follow(){
        //Change pos to cardinal points
        RB.MovePosition(Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed/50));
        Target = new Vector3 (Player.transform.position.x, transform.position.y, Player.transform.position.z);
        TargetDir = Quaternion.LookRotation(Target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetDir, LookSpeedChase * Time.deltaTime);
    }
    void WalkIdel(){
        RB.MovePosition(Vector3.Lerp(transform.position, transform.position + transform.forward,(MoveSpeed/100)));
        transform.rotation = Quaternion.Slerp(transform.rotation, IdelDir, LookSpeedIdel * Time.deltaTime);
    }
    int TurnBias() {
        float rand = Random.value;
        if (rand <= .5f)
            return Random.Range(-90, 90);
        if (rand <= .8f)
            return Random.Range(-130, 130);
        return Random.Range(-180, 180);
    }
    void NewDirection(){
        CurrentDir = transform.rotation;
        IdelDir = CurrentDir * Quaternion.Euler(0,TurnBias(),0);
    }
    void Lingering(){
        if (!Sighting && !Idel) Idel = true;
    }
    void RandomDetect(){
        float i = 0;
        PlayerDistance = Vector3.Distance(Player.transform.position, transform.position);
        i = Random.Range(0f, (float)PlayerDistance/Alertness); 
        if (i < 0.3f) Idel = false;
    }
}
