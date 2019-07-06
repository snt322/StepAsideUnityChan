using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTestController : MonoBehaviour {

    Animator ani = null;
    readonly string animeName = "WAIT04";
    float initDeg = 0.0f;
    float targetDeg = 180.0f;

    // Use this for initialization

    void Start () {
        ani = this.gameObject.GetComponent<Animator>() as Animator;
        if(ani != null)
        {
            ani.Play(animeName);
        }
        else
        {
            string message = string.Format("オブジェクト名：{0} , Animatorがアタッチされていません。", this.name);
            Debug.Log(message);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if(ani != null)
        {
            AnimatorStateInfo inf = ani.GetCurrentAnimatorStateInfo(0);
            if(inf.IsName(animeName))
            {
                float deg = initDeg;
                deg = this.transform.eulerAngles.y >= targetDeg ? deg = 0 : deg += (180 * Time.deltaTime);
                this.transform.Rotate(Vector3.up, deg);
            }
            else
            {
                //アニメーションが終了した場合、スクリプト自身をリムーブする
                Destroy(this);
            }
        }

        

	}


    //ani.Play("WAIT04")を再生中にUnityちゃんの向きをY軸周りに回転させる
    void RotateDuringAnime(float deg)
    {
        
    }


}
