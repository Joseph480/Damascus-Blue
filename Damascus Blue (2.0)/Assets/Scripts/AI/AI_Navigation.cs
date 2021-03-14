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
    public bool n, ne, e, se, s, sw, w, nw;
    private Ray N, NE, E, SE, S, SW, W, NW;
    private RaycastHit HitN, HitNE, HitE, HitSE, HitS, HitSW, HitW, HitNW;
    private Vector3 PosN, PosNE, PosE, PosSE, PosS, PosSW, PosW, PosNW;
    private Vector3 Mover;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        //Cycles below
        if (!HasPath)InvokeRepeating("NewDirection",IdelPathTime,IdelPathTime);
        InvokeRepeating("Lingering", IdelPathTime*IdelPathTime,IdelPathTime*IdelPathTime);
        InvokeRepeating("RandomDetect",(float)IdelPathTime/2,(float)IdelPathTime/2);
        InvokeRepeating("Navigate",0.2f,0.2f);
        Idel = true;
    }
    void Update(){
        Detect();
        DrawNavLines();
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
    void DrawNavLines(){
        Debug.DrawRay(N.origin,N.direction * NavLength,Color.yellow);
        Debug.DrawRay(NE.origin,NE.direction * NavLength,Color.blue);
        Debug.DrawRay(E.origin,E.direction * NavLength,Color.red);
        Debug.DrawRay(SE.origin,SE.direction * NavLength,Color.blue);
        Debug.DrawRay(S.origin,S.direction * NavLength,Color.red);
        Debug.DrawRay(SW.origin,SW.direction * NavLength,Color.blue);
        Debug.DrawRay(W.origin,W.direction * NavLength,Color.red);
        Debug.DrawRay(NW.origin,NW.direction * NavLength,Color.blue);
    }
    void OnDrawGizmos(){
        Gizmos.DrawSphere(PosN, 0.2f);
        Gizmos.DrawSphere(PosNE, 0.2f);
        Gizmos.DrawSphere(PosE, 0.2f);
        Gizmos.DrawSphere(PosSE, 0.2f);
        Gizmos.DrawSphere(PosS, 0.2f);
        Gizmos.DrawSphere(PosSW, 0.2f);
        Gizmos.DrawSphere(PosW, 0.2f);
        Gizmos.DrawSphere(PosNW, 0.2f);
        Gizmos.DrawSphere(Mover, 0.5f);
    }
    void ClosestNodeToPlayer(){
        float x = Vector3.Distance(PosN,Player.transform.position);

        if (!n){ x = Vector3.Distance(PosN,Player.transform.position); Mover = PosN;}

        if (x > Vector3.Distance(PosNE,Player.transform.position) && !ne){ 
            x = Vector3.Distance(PosNE,Player.transform.position); Mover = PosNE;}

        if (x > Vector3.Distance(PosE,Player.transform.position) && !e){ 
            x = Vector3.Distance(PosE,Player.transform.position); Mover = PosE;}

        if (x > Vector3.Distance(PosSE,Player.transform.position) && !se){ 
            x = Vector3.Distance(PosSE,Player.transform.position); Mover = PosSE;}

        if (x > Vector3.Distance(PosS,Player.transform.position) && !s){ 
            x = Vector3.Distance(PosS,Player.transform.position); Mover = PosS;}

        if (x > Vector3.Distance(PosSW,Player.transform.position) && !sw){ 
            x = Vector3.Distance(PosSW,Player.transform.position); Mover = PosSW;}

        if (x > Vector3.Distance(PosW,Player.transform.position) && !w){ 
            x = Vector3.Distance(PosW,Player.transform.position); Mover = PosW;}

        if (x > Vector3.Distance(PosNW,Player.transform.position) && !nw){ 
            x = Vector3.Distance(PosNW,Player.transform.position); Mover = PosNW;}

    }
    void Navigate(){
        N.origin = transform.position;
        NE.origin = transform.position;
        E.origin = transform.position;
        SE.origin = transform.position;
        S.origin = transform.position;
        SW.origin = transform.position;
        W.origin = transform.position;
        NW.origin = transform.position;

        N.direction = transform.forward;
        NE.direction = transform.forward + transform.right.normalized;
        E.direction = transform.right;
        SE.direction = transform.right + -transform.forward.normalized;
        S.direction = -transform.forward;
        SW.direction = -transform.forward + -transform.right.normalized;
        W.direction = -transform.right;
        NW.direction = -transform.right + transform.forward.normalized;

        PosN = transform.position + N.direction * NavLength;
        PosNE = transform.position + NE.direction * NavLength;
        PosE = transform.position + E.direction * NavLength;
        PosSE = transform.position + SE.direction * NavLength;
        PosS = transform.position + S.direction * NavLength;
        PosSW = transform.position + SW.direction * NavLength;
        PosW = transform.position + W.direction * NavLength;
        PosNW = transform.position + NW.direction * NavLength;

        if (Physics.Raycast(N, out HitN, NavLength)){    
            if(HitN.collider.tag != "Player" && HitN.collider.tag != "Enemy") n = true;
        } else n = false;

        if (Physics.Raycast(NE, out HitNE, NavLength)){    
            if(HitNE.collider.tag != "Player" && HitNE.collider.tag != "Enemy") ne = true;
        } else ne = false;

        if (Physics.Raycast(E, out HitE, NavLength)){    
            if(HitE.collider.tag != "Player" && HitE.collider.tag != "Enemy") e = true;
        } else e = false;

        if (Physics.Raycast(SE, out HitSE, NavLength)){    
            if(HitSE.collider.tag != "Player" && HitSE.collider.tag != "Enemy") se = true;
        } else se = false;

        if (Physics.Raycast(S, out HitS, NavLength)){    
            if(HitS.collider.tag != "Player" && HitS.collider.tag != "Enemy") s = true;
        } else s = false;

        if (Physics.Raycast(SW, out HitSW, NavLength)){    
            if(HitSW.collider.tag != "Player" && HitSW.collider.tag != "Enemy") sw = true;
        } else sw = false;

        if (Physics.Raycast(W, out HitW, NavLength)){    
            if(HitW.collider.tag != "Player" && HitW.collider.tag != "Enemy") w = true;
        } else w = false;

        if (Physics.Raycast(NW, out HitNW, NavLength)){    
            if(HitNW.collider.tag != "Player" && HitNW.collider.tag != "Enemy") nw = true;
        } else nw = false;

        ClosestNodeToPlayer();
    }
    void Follow(){
        //Change pos to cardinal points
        RB.MovePosition(Vector3.MoveTowards(transform.position, Mover, MoveSpeed/50));
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
