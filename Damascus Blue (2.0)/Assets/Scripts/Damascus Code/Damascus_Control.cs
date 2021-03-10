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
    //private player_animation PA;
    //private Player_audio AD;

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
    }
    void Update(){
        ReadMouse();
    }
    void FixedUpdate(){
       if(Shot)FireGun();
    }
    void ReadMouse(){
        if (Input.GetKeyDown(Fire) && !Cooling)Shot = true;
        if (Input.GetKeyDown(Clutch)){
            Clutching = true;
            Spread = SpreadDefault;
        }
        else if (Input.GetKeyUp(Clutch)){
            Clutching = false;
            Spread = Spread * 3;
        }
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
        //PA.SetFire(1);
        //PA.SetFlag(1);
        //Invoke ("Settle", 0.1f);
        Invoke ("Reload", CoolDown);
    }
    void Kickback(){
        if (Clutching)
            ParentBody.velocity = Origin.transform.forward * -Kick;
    }
    void Settle(){
        //PA.SetFire(0);
    }
    void Reload(){
        //PA.SetFlag(0);
        Cooling = false;
    }
}
