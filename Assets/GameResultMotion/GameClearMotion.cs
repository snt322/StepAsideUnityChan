using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearMotion : MonoBehaviour {

    Animator ani = null;
    readonly string animeName = "WAIT04";                               //ゲームクリアモーションの開始ステート名

    Transform unityTForm;                                               //スクリプト開始時のUnityちゃんのTransform
    Transform camTForm;                                                 //メインカメラのTransform

    Camera cam;                                                         //メインカメラコンポーネント
    float originalFOV = 0.0f;                                           //初期のメインカメラFOV
    float targetFOV = 15.0f;                                            //アニメーション"WAIT04"終了時点でのメインカメラFOV

    Quaternion targetQuat = new Quaternion();

    float blendRate = 0.0f;                                             //UnityちゃんがY軸周りに回転する回転度合い[Quaternion.Lerp(,blendRate:0～1)に使用]

    // Use this for initialization
    void Start ()
    {
        ani = this.gameObject.GetComponent<Animator>() as Animator;
        if (ani != null)
        {
            ani.Play(animeName);                                                                     //Unityちゃんが回転するアニメーション["WAIT04"]をスタート

            unityTForm = this.gameObject.transform;                                                  //スクリプト開始時のUnityちゃんのTransformを取得
            camTForm   = GameObject.Find("Main Camera").GetComponent<Transform>() as Transform;      //メインカメラのTransformを取得

            if (camTForm != null)
            {
                Vector3 dir = camTForm.position - unityTForm.position;                                   //UnityちゃんからMainCameraへの方向ベクトル
                dir.y = 0.0f;                                                                            //方向ベクトルのうちy成分を0にする
                dir.Normalize();                                                                         //正規化

                targetQuat.SetLookRotation(dir, unityTForm.up);                                         //Quaternion targetQuatに[Unityちゃんのup方向をupとして、dir方向へ向く]quaternionをセットする
            }

            cam = GameObject.Find("Main Camera").GetComponent<Camera>() as Camera;
            if(cam != null)
            {
                originalFOV = cam.fieldOfView;
            }

        }
        else
        {
            string message = string.Format("オブジェクト名：{0} , Animatorがアタッチされていません。", this.name);
            Debug.Log(message);
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ani != null)
        {
            AnimatorStateInfo inf = ani.GetCurrentAnimatorStateInfo(0);     //アニメーション"WAIT04"中にUnityちゃんがカメラ方向へ向く
            if (inf.IsName(animeName))
            {
                if (blendRate >= 1.0f) { blendRate = 1.0f; }
                else
                {
                    blendRate += 0.01f;                                                                                 //増分+0.0001fは適当に決定
                    this.transform.rotation = Quaternion.Lerp(unityTForm.rotation, targetQuat, blendRate);
                }


                if(cam != null)                                                                                          //メインカメラを取得出来ていたら、Unityちゃんにズームする
                {
                    cam.fieldOfView = originalFOV - blendRate * (originalFOV - targetFOV);
                    cam.transform.position = new Vector3(unityTForm.position.x * blendRate, cam.transform.position.y, cam.transform.position.z);
                }
            }
            else
            {
                //アニメーション"WAIT04"が終了したらこのスクリプトは不要になるのでデタッチする。
                Destroy(this);
            }
        }
    }
}
