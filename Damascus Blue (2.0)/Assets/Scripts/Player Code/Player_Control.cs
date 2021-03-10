using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    public KeyCode Forward,Back,Left,Right,Sprint,Jump,Pause;
    public float MoveSpeed,LookSpeed,JumpPower,SprintAdditive,Drift,Drag;
    public bool Paused,Moving; 
    public Camera Cam;

    private Vector3 CamF,CamR,Mover,Slider;
    private Vector2 MinMax = new Vector2 (-90f, 90f);
    private float Yaw, Pitch, BaseSpeed;
    [HideInInspector]
    public bool Mf,Mb,Ml,Mr,Sp,Jmp;
    private Rigidbody Rb;

    void Awake(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start(){
        Cam = Camera.main;
        Rb = this.GetComponent<Rigidbody>();
        BaseSpeed = MoveSpeed;
        CamRelative();
    }
    void Update(){
        ReadController();
        if(Paused)return;
        MoveDir();
        CheckMoving();
    }
    void LateUpdate(){
        if(Paused)return;
        CamRotate();
    }
    void FixedUpdate(){
        if(Paused)return;
        ApplyForce();
        Sprinting();
        Jumping();
    }
    void ReadController(){
        if(Input.GetKeyDown(Pause))PausePlayer(!Paused);
        if(Input.GetKey(Forward))Mf=true;else Mf=false;
        if(Input.GetKey(Back))Mb=true;else Mb=false;
        if(Input.GetKey(Left))Ml=true;else Ml=false;
        if(Input.GetKey(Right))Mr=true;else Mr=false;
        if(Input.GetKey(Sprint))Sp=true;else Sp=false;
        if(Input.GetKeyDown(Jump) && Landing_Control.Jumps > 0) Jmp=true;
    }
    void CheckMoving(){
        if (Ml||Mr||Mf||Mb)Moving=true;else Moving=false;
    }
    void MoveDir(){
        CamRelative();
        if (Mf){Mover += Vector3.Lerp(CamF,CamF,MoveSpeed);}
        if (Mb){Mover -= Vector3.Lerp(CamF,CamF,MoveSpeed);}
        if (Ml){Mover -= Vector3.Lerp(CamR,CamR,MoveSpeed);}
        if (Mr){Mover += Vector3.Lerp(CamR,CamR,MoveSpeed);}
    }
    void ApplyForce(){
        Slider = Mover - transform.position; Vector3.Normalize(Slider);
        Rb.AddForce(Slider * (Drift + Rb.velocity.magnitude), ForceMode.Acceleration); if (Rb.velocity.magnitude >= Drag) Rb.AddForce(-Slider * (Drift + Rb.velocity.magnitude), ForceMode.Acceleration);
        Rb.MovePosition(Vector3.Lerp(transform.position, Mover,(MoveSpeed/50)));
    }
    void CamRelative(){
        Mover = transform.position;
        CamF = Cam.transform.forward;
        CamR = Cam.transform.right;
        CamF.y = 0; CamF = CamF.normalized;
        CamR.y = 0; CamR = CamR.normalized;
    }
    void CamRotate(){
        Yaw += Input.GetAxis("Mouse X") * LookSpeed;
        Pitch -= Input.GetAxis("Mouse Y") * LookSpeed;
        Pitch = Mathf.Clamp(Pitch, MinMax.x, MinMax.y);
        transform.rotation = Quaternion.Euler(0,Yaw,0);
        Cam.transform.rotation = Quaternion.Euler(Pitch,transform.rotation.eulerAngles.y,0);
    }
    void Jumping(){
        if (Jmp){
            Vector3 vector;
            vector.x = Rb.velocity.x; 
            vector.y = JumpPower; 
            vector.z = Rb.velocity.z;
            Landing_Control.Jumps--;
            Rb.velocity = vector;
            Jmp=false;
        }
    }
    void Sprinting(){
        if (Sp) MoveSpeed = Mathf.Lerp(MoveSpeed,BaseSpeed + SprintAdditive,MoveSpeed);
        else MoveSpeed = BaseSpeed;
    }
    public void PausePlayer(bool i){
        Paused = i;
        if (i){
			Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0.0001f;
        }
		else if (!i){
			Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
        //Debug.Log(Paused);
    }
}
