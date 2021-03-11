using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damascus_Control : MonoBehaviour
{
    public KeyCode Fire, Clutch;
    public GameObject Munition;
    public float CoolDown, ShotForce, Spread, Kick;
    public int ClusterSize;

    List<Quaternion> Scatter;

    private bool Cooling, Shot, Clutching;
    private float SpreadDefault;
    private Transform Origin;
    private Rigidbody ParentBody;
    private Animator PA;
    private Camera Cam;
    private Player_Control Player;

    void Awake(){
        Shot = false;
        PreLoad();
    }
    void PreLoad(){
        SpreadDefault = Spread;
        Scatter = new List<Quaternion>(ClusterSize);
        for (int i = 0; i < ClusterSize; i++){
            Scatter.Add(Quaternion.Euler(Vector3.zero));
        }
    }
    void Start(){
        Cooling = false;
        Origin = GameObject.Find("Origin").transform;
        ParentBody = GameObject.Find("Avatar").GetComponent<Rigidbody>();
        PA = transform.GetComponent<Animator>();
        Cam = Camera.main;
        Player = ParentBody.GetComponent<Player_Control>();
    }
    void Update(){
        if (Player.Paused) return;
        ReadMouse();
    }
    void FixedUpdate(){
       if(Shot)FireGun();
       FieldFocus();
    }
    void ReadMouse(){
        if (Input.GetKeyDown(Fire) && !Cooling)Shot = true;
        if (Input.GetKeyDown(Clutch)){
            Clutching = true;
            PA.SetInteger("Clutch",1);
            Spread = SpreadDefault;
        }
        else if (Input.GetKeyUp(Clutch)){
            Clutching = false;
            PA.SetInteger("Clutch",0);
            Spread = Spread * 2;
        }
    }
    void FieldFocus(){
        if (Clutching) Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, 65, 0.2f);
        if (!Clutching) Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, 75, 0.2f);
    }
    void FireGun(){
        Shot = false;
        Cooling = true;
        for (int i = 0; i < ClusterSize; i++){
            Scatter[i] = Random.rotation;
            GameObject M = Instantiate(Munition, Origin.position, Origin.rotation);
            M.transform.rotation = Quaternion.RotateTowards(M.transform.rotation, Scatter[i], Spread);
            M.transform.GetComponent<Rigidbody>().velocity = M.transform.forward * ShotForce;
        }
        Kickback();
        PA.SetTrigger("Fire");
        PA.SetInteger("Flag",1);
        Invoke ("Reload", CoolDown);
    }
    void Kickback(){
        if (Clutching)
            ParentBody.velocity = Origin.transform.forward * -Kick;
    }
    void Reload(){
        PA.SetInteger("Flag",0);
        Cooling = false;
    }
}
