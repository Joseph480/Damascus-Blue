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
        //PA = FindObjectOfType<player_animation>().GetComponent<player_animation>();
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
            //PA.SetClutch(1);
            Spread = Spread * 3;
        }
        else if (Input.GetKeyUp(Clutch)){
            Clutching = false;
            //PA.SetClutch(0);
            Spread = SpreadDefault;
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
        //Invoke ("settle", 0.1f);
        //Invoke ("reload", CoolDown);
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
