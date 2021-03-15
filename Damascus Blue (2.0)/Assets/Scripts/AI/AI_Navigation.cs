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
    private float PlayerDistance, DefaultAlert;
    private bool Sighting, EmptyRay, Idel;
    private Rigidbody RB;
    private Ray Vision;
    private RaycastHit Hit; 

    public float NavLength;
    public Transform NavOffset;
    private bool n, ne, e, se, s, sw, w, nw;
    private Ray N, NE, E, SE, S, SW, W, NW;
    private RaycastHit HitN, HitNE, HitE, HitSE, HitS, HitSW, HitW, HitNW;
    private Vector3 PosN, PosNE, PosE, PosSE, PosS, PosSW, PosW, PosNW;
    private Vector3 Mover;
    //Net casting off Cardinal rays.
    private float NetReach;
    private Ray[] Net = new Ray[8];
    private Vector3[] NetNodes;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        DefaultAlert = Alertness;
        //Cycles below
        if (!HasPath)InvokeRepeating("NewDirection",IdelPathTime,IdelPathTime);
        InvokeRepeating("Lingering", IdelPathTime * 5,IdelPathTime * 5);
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
            case "Projectile": Idel = false; Alertness = DefaultAlert * 2; break;
            default: break;
        }
    }
    void Detect(){
        Vision.origin = transform.position + RayOriginOffset;
        Vision.direction = transform.forward;
        if (Physics.Raycast(Vision,out Hit, SightLength))
        {
            if (Hit.collider.tag == "Player"){ 
                Idel = false; Sighting = true; Alertness = DefaultAlert * 2;
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

        Debug.DrawRay(Net[0].origin,Net[0].direction * NetReach, Color.green);
        Debug.DrawRay(Net[1].origin,Net[1].direction * NetReach, Color.green);
        Debug.DrawRay(Net[2].origin,Net[2].direction * NetReach, Color.green);
        Debug.DrawRay(Net[3].origin,Net[3].direction * NetReach, Color.green);
        Debug.DrawRay(Net[4].origin,Net[4].direction * NetReach, Color.green);
        Debug.DrawRay(Net[5].origin,Net[5].direction * NetReach, Color.green);
        Debug.DrawRay(Net[6].origin,Net[6].direction * NetReach, Color.green);
        Debug.DrawRay(Net[7].origin,Net[7].direction * NetReach, Color.green);
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
        float x = Vector3.Distance(transform.position,Player.transform.position);

        if (x > Vector3.Distance(PosN,Player.transform.position) && !n){
            x = Vector3.Distance(PosN,Player.transform.position); Mover = PosN;}

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

        NetForPlayer();

    }
    void NetForPlayer(){
        Net[0].origin = PosN;
        Net[1].origin = PosNE;
        Net[2].origin = PosE;
        Net[3].origin = PosSE;
        Net[4].origin = PosS;
        Net[5].origin = PosSW;
        Net[6].origin = PosW;
        Net[7].origin = PosNW;

        Net[0].direction = N.direction;
        Net[1].direction = NE.direction;
        Net[2].direction = E.direction;
        Net[3].direction = SE.direction;
        Net[4].direction = S.direction;
        Net[5].direction = SW.direction;
        Net[6].direction = W.direction;
        Net[7].direction = NW.direction;
    }
    void Navigate(){
        N.origin = NavOffset.position;
        NE.origin = NavOffset.position;
        E.origin = NavOffset.position;
        SE.origin = NavOffset.position;
        S.origin = NavOffset.position;
        SW.origin = NavOffset.position;
        W.origin = NavOffset.position;
        NW.origin = NavOffset.position;

        N.direction = NavOffset.forward;
        NE.direction = NavOffset.forward + NavOffset.right.normalized;
        E.direction = NavOffset.right;
        SE.direction = NavOffset.right + -NavOffset.forward.normalized;
        S.direction = -NavOffset.forward;
        SW.direction = -NavOffset.forward + -NavOffset.right.normalized;
        W.direction = -NavOffset.right;
        NW.direction = -NavOffset.right + NavOffset.forward.normalized;

        PosN = NavOffset.position + N.direction * NavLength;
        PosNE = NavOffset.position + NE.direction * NavLength;
        PosE = NavOffset.position + E.direction * NavLength;
        PosSE = NavOffset.position + SE.direction * NavLength;
        PosS = NavOffset.position + S.direction * NavLength;
        PosSW = NavOffset.position + SW.direction * NavLength;
        PosW = NavOffset.position + W.direction * NavLength;
        PosNW = NavOffset.position + NW.direction * NavLength;

        if (Physics.Raycast(N, out HitN, NavLength)){    
            if(HitN.collider.tag != "Player") n = true;} else n = false;

        if (Physics.Raycast(NE, out HitNE, NavLength)){    
            if(HitNE.collider.tag != "Player") ne = true;} else ne = false;

        if (Physics.Raycast(E, out HitE, NavLength)){    
            if(HitE.collider.tag != "Player") e = true;} else e = false;

        if (Physics.Raycast(SE, out HitSE, NavLength)){    
            if(HitSE.collider.tag != "Player") se = true;} else se = false;

        if (Physics.Raycast(S, out HitS, NavLength)){    
            if(HitS.collider.tag != "Player") s = true;} else s = false;

        if (Physics.Raycast(SW, out HitSW, NavLength)){    
            if(HitSW.collider.tag != "Player") sw = true;} else sw = false;

        if (Physics.Raycast(W, out HitW, NavLength)){    
            if(HitW.collider.tag != "Player") w = true;} else w = false;

        if (Physics.Raycast(NW, out HitNW, NavLength)){    
            if(HitNW.collider.tag != "Player") nw = true;} else nw = false;

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
        if (!Sighting && !Idel) Idel = true; Alertness = DefaultAlert;
    }
    void RandomDetect(){
        float i = 0;
        PlayerDistance = Vector3.Distance(Player.transform.position, transform.position);
        i = Random.Range(0f, (float)PlayerDistance/Alertness); 
        if (i < 0.3f) Idel = false;
    }
}

/*

        Make functions that change nav length and nav height depending on situation.
        OR: cast new rays from tips of original 8.
        Have Pos variables be located also hit points, so that you might be able to send more rays from those locations.

*/
