using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Navigation : MonoBehaviour
{
    public float MoveSpeed, LookSpeedChase, LookSpeedIdel, IdelPathTime, HuntTime, SightLength;
    public float Alertness;
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

    public float NavLength, NavRadius;
    public Transform NavOffset;
    private bool n, ne, e, se, s, sw, w, nw;
    private Ray N, NE, E, SE, S, SW, W, NW;
    private RaycastHit HitN, HitNE, HitE, HitSE, HitS, HitSW, HitW, HitNW;
    private Vector3 PosN, PosNE, PosE, PosSE, PosS, PosSW, PosW, PosNW;
    private Vector3 Mover, PlayerGhost;
    private Ray ImpliedVision;
    private RaycastHit ImpliedHit;

    void Start(){
        Player = FindObjectOfType<Player_Control>();
        RB = GetComponent<Rigidbody>();
        Origin = transform.position;
        RayOriginOffset = new Vector3(0,0.5f,0);
        DefaultAlert = Alertness;
        //Cycles below
        if (!HasPath)InvokeRepeating("NewDirection",IdelPathTime,IdelPathTime);
        InvokeRepeating("NearDetect",(float)IdelPathTime/2,(float)IdelPathTime/2);
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
        if (!Sighting) Vision.direction = transform.forward;
        if (Sighting) Vision.direction = PlayerGhost - transform.position;
        if (Physics.Raycast(Vision,out Hit, SightLength))
        {
            if (Hit.collider.tag == "Player"){ 
                Idel = false; Sighting = true; Alertness = DefaultAlert * 2;
                Invoke("Lingering", HuntTime);
                PlayerGhost = Player.transform.position;
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
        Debug.DrawRay(Vision.origin, Vision.direction * SightLength, Color.yellow);
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
        Gizmos.DrawSphere(PlayerGhost, 0.75f);
    }
    void ClosestNodeToPlayer(){
        float x = Vector3.Distance(transform.position,PlayerGhost) + 1;

        if (x > Vector3.Distance(PosN,PlayerGhost) && !n){
            x = Vector3.Distance(PosN,PlayerGhost); Mover = PosN;}

        if (x > Vector3.Distance(PosNE,PlayerGhost) && !ne){ 
            x = Vector3.Distance(PosNE,PlayerGhost); Mover = PosNE;}

        if (x > Vector3.Distance(PosE,PlayerGhost) && !e){ 
            x = Vector3.Distance(PosE,PlayerGhost); Mover = PosE;}

        if (x > Vector3.Distance(PosSE,PlayerGhost) && !se){ 
            x = Vector3.Distance(PosSE,PlayerGhost); Mover = PosSE;}

        if (x > Vector3.Distance(PosS,PlayerGhost) && !s){ 
            x = Vector3.Distance(PosS,PlayerGhost); Mover = PosS;}

        if (x > Vector3.Distance(PosSW,PlayerGhost) && !sw){ 
            x = Vector3.Distance(PosSW,PlayerGhost); Mover = PosSW;}

        if (x > Vector3.Distance(PosW,PlayerGhost) && !w){ 
            x = Vector3.Distance(PosW,PlayerGhost); Mover = PosW;}

        if (x > Vector3.Distance(PosNW,PlayerGhost) && !nw){ 
            x = Vector3.Distance(PosNW,PlayerGhost); Mover = PosNW;}
    }
    void CheckIfGhostReached(){
        var x = Vector3.Distance(transform.position, PlayerGhost);
        ImpliedVision.origin = PlayerGhost;
        ImpliedVision.direction = Player.transform.position - PlayerGhost;
        if (Physics.Raycast(ImpliedVision, out ImpliedHit, SightLength) && x <= 0.5f){
            if (ImpliedHit.transform.tag == "Player") PlayerGhost = ImpliedHit.point;
        }
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

        //I think it works better using vecto3 then transform. But the problem is the rays no longer
        //rotate with a.i, so it's easy for the a.i to have non-dynamic casting, leading to getting stuck on edges,
        //and will probably get stuck looking through peepholes. Therefore, rays need to be dynamic.
        N.direction = Vector3.forward;
        NE.direction = Vector3.forward + Vector3.right.normalized;
        E.direction = Vector3.right;
        SE.direction = Vector3.right + -Vector3.forward.normalized;
        S.direction = -Vector3.forward;
        SW.direction = -Vector3.forward + -Vector3.right.normalized;
        W.direction = -Vector3.right;
        NW.direction = -Vector3.right + Vector3.forward.normalized;

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
        CheckIfGhostReached();
        if (!Sighting)RB.MovePosition(Vector3.MoveTowards(transform.position, Mover, MoveSpeed/50));
        else RB.MovePosition(Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed/50));
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
    //This one actually does need to be an ienumerator so that it can be started and cancled accordingly.
    //We want to use this for handling object permanence when finding the player. Like, if the player
    //goes around a corner, then if the timer is still high enough, we can send the player's position
    //to the PlayerGhost variable. But if not, then we use implied vision.
    void Lingering(){
        if (!Sighting && !Idel){ 
            Idel = true; Alertness = DefaultAlert;
        } else Invoke("Lingering", HuntTime);
    }
    void NearDetect(){
        if (!Idel) return;
        PlayerDistance = Vector3.Distance(Player.transform.position, transform.position);
        float i = PlayerDistance/Alertness; float x = Random.Range(0, Alertness * 0.01f);
        if (i < x){Idel = false; PlayerGhost = Player.transform.position;}
    }
}

/*
            THINGS LEFT TO DO:
            Fix problem where A.I doesn't know what to do after getting to playerghost.
            Fix going back and forth when two nodes are closest to player.
            Fix A.I walking off edges that are too deep. (probably have rays shoot down from nodes)
            Fix walking into edges of objects.
            Add HasPath functionality.
            Add jumping over small obstacles functionality.

            After that, start giving A.I guns, and then control their distance to player.
            Enemies should only get so close before firing. If they can't see the player, they don't shoot.
            If they can see the player, and they are close enough, only shoot so often.
            If the player is above them (at a certain height), don't bother getting close, just shoot.

            Maybe only run Near Detect when player is visible to a.i
*/