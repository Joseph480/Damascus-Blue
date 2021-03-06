using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBack_Ray : MonoBehaviour
{
    public float Length;
    public GameObject Holster, Gun;

    public bool Hitting;

    private Ray Ray;
    private RaycastHit Hit = new RaycastHit();
    private Vector3 Origin, Offset;
    private Player_Control Player;

    void Start(){
        Origin = Holster.transform.localPosition;
        Offset = new Vector3 (Origin.x,Origin.y,Origin.z - 1.5f);
        Player = transform.root.GetComponent<Player_Control>();
    }
    void Update(){
        CastRay();
        if (Player.Paused) return;
        PositionGun();
    }
    void CastRay(){
        Ray = new Ray(transform.position,transform.forward);
        Debug.DrawRay(transform.position,transform.forward * Length, Color.red);
        if (Physics.Raycast(Ray, out Hit, Length)){
            Hitting = true;
        } else Hitting = false;
    }
    void PositionGun(){
        if (Hitting)Holster.transform.localPosition = Vector3.Lerp(Holster.transform.localPosition, Offset, 0.1f);
        else Holster.transform.localPosition = Vector3.Lerp (Holster.transform.localPosition, Origin, 0.1f);
    }
}
