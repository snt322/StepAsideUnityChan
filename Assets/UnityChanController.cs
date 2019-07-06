using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using System.Runtime.CompilerServices;


public class UnityChanController : MonoBehaviour {
    enum enumMotion
    {
        None = 0,
        WaitFirstMotionAfterEnd,
        FirstMotionAfterEnd,
        MotionLoopAfterEnd,
        WaitFirstMotionAfterGoal,
        FirstMotionAfterGoal,
        MotionLoopAfterGoal,
    }



    //デバッグ用Unityちゃんがスタート地点から移動しなくするフラグ
    [SerializeField]
    private bool canMove = true;



    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネント入れる(追加)
    private Rigidbody myRigidbody;
    //前進するための力 (追加)
    private float forwardForce = 800.0f;
    //ジャンプするための力 (追加)
    private float upForce = 500.0f;
    //左右に移動する為の力 (追加)
    private float turnForce = 500.0f;
    //左右の移動できる範囲 (追加)
    private float movableRange = 3.4f;
    //動きを減衰させる係数
    private float coefficient = 0.95f;

    //ゲーム終了の判定
    private bool isEnd = false;
    
    //ゲーム終了時に表示するテキスト
    private GameObject stateText;
    //スコアを表示するテキスト
    private GameObject scoreText;
    //得点
    private int score = 0;

    //左ボタン押下の判定
    private bool isLButtonDown = false;
    //右ボタン押下の判定
    private bool isRButtonDown = false;

    //アイテムを生成するスクリプト
    ItemGenerator mItemGenScript = null;


    //ゲーム終了判定後のアイドリング
    private enumMotion eMotion = enumMotion.None;

    //スクリプト自身が不要になったら自分自身を削除する
    void DestroyMySelf()
    {
        Destroy(this);
    }


	// Use this for initialization
	void Start () {

        //発展課題
        mItemGenScript = GameObject.Find("ItemGenerator").GetComponent<ItemGenerator>() as ItemGenerator;

        //Animatorコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを開始
        myAnimator.SetFloat("Speed", 1.0f);


        //Rigidbodyコンポーネントを取得(追加)
        this.myRigidbody = GetComponent<Rigidbody>();

        //シーン中のstateTextオブジェクトを取得
        this.stateText = GameObject.Find("GameResultText");

        //シーン中のscoreTextオブジェクトを取得
        this.scoreText = GameObject.Find("ScoreText");
	}
	
	// Update is called once per frame
	void Update () {

        //発展課題：ユニティちゃんの位置に応じてアイテムを動的に生成しましょう
        //アイテムを生成する関数を呼び出し
        if(mItemGenScript != null)
        {
            mItemGenScript.UpdateItem(this.gameObject.transform.position.z);
        }


        //ゲーム終了ならUnityちゃんの動きを減衰する
        if (this.isEnd && eMotion != enumMotion.MotionLoopAfterEnd)
        {
            this.forwardForce *= this.coefficient;
            this.turnForce *= this.coefficient;
            this.upForce *= this.coefficient;
            this.myAnimator.speed *= this.coefficient;


            //Unityちゃんの動きが十分に減衰したら、次のモーションに映る
            if(this.myAnimator.speed <= 0.01f)
            {
                switch(eMotion)
                {
                    case enumMotion.WaitFirstMotionAfterEnd:                //ゲームオーバの場合
                        eMotion = enumMotion.FirstMotionAfterEnd;
                        break;
                    case enumMotion.WaitFirstMotionAfterGoal:               //ゲームクリアの場合
                        eMotion = enumMotion.FirstMotionAfterGoal;
                        break;
                    default:
                        break;
                }
            }
        }

        //ゲームが終了したら息切れしたポーズを行う
        if(eMotion == enumMotion.FirstMotionAfterEnd)
        {
            Debug.Log("eMotion 1stEnd");
            this.myAnimator.speed = 1.0f;                                   //アニメーションのspeedを1に戻す
//            this.myAnimator.Play("REFLESH00");                            //ゲームオーバのモーション
            eMotion = enumMotion.MotionLoopAfterEnd;

            this.gameObject.AddComponent<GameOverMotion>();
            DestroyMySelf();
        }

        //ゴールしてゲームが終了しポーズをループする
        if(eMotion == enumMotion.FirstMotionAfterGoal)
        {
            Debug.Log("eMotion 1st Goal");
            this.myAnimator.speed = 1.0f;                                   //アニメーションのspeedを1に戻す
//            this.myAnimator.Play("WAIT04");                               //ゲームクリアのモーション
            eMotion = enumMotion.MotionLoopAfterEnd;

            this.gameObject.AddComponent<GameClearMotion>();
            DestroyMySelf();
        }


        //unityちゃんに前方向の力を加える(追加)
        if (canMove)
        {
            this.myRigidbody.AddForce(this.transform.forward * this.forwardForce);
        }


        //Unityちゃんを矢印キーまたはボタンに応じて左右に移動させる
        if( (Input.GetKey(KeyCode.LeftArrow) || this.isLButtonDown) && (- this.movableRange < this.transform.position.x ))
        {
            this.myRigidbody.AddForce(- this.transform.right * this.turnForce);
        }

        if( (Input.GetKey(KeyCode.RightArrow) || this.isRButtonDown) && (this.movableRange > this.transform.position.x))
        {
            this.myRigidbody.AddForce( this.transform.right * this.turnForce);
        }

        //Jumpステートの場合はJumpにfalseをセットする (追加)
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }
        if (Input.GetKey(KeyCode.Space) && this.transform.position.y < 0.5f)
        {
            this.myAnimator.SetBool("Jump", true);
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }


    }

    private void FixedUpdate()
    {
/*
        //Jumpステートの場合はJumpにfalseをセットする (追加)
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }
        if (Input.GetKey(KeyCode.Space) && this.transform.position.y < 0.5f)
        {
            this.myAnimator.SetBool("Jump", true);
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }
*/
    }

    //トリガーモードでほかのオブジェクトと接触した場合の処理
    private void OnTriggerEnter(Collider other)
    {
        //障害物に衝突した場合
        if (other.gameObject.tag == "CarTag" || other.gameObject.tag == "TrafficConeTag")
        {
            Debug.Log("d");
            this.isEnd = true;
            this.eMotion = enumMotion.WaitFirstMotionAfterEnd;

            WriteUITextResult("Game Over.");
        }

        //ゴール地点に到達した場合
        if(other.gameObject.tag == "GoalTag")
        {
            Debug.Log("g");
            this.isEnd = true;
            this.eMotion = enumMotion.WaitFirstMotionAfterGoal;

            WriteUITextResult("Clear!!");
        }

        //コインに衝突した場合
        if(other.gameObject.tag == "CoinTag")
        {
            Debug.Log("c");

            //スコアを加算
            this.score += 10;

            //ScoreText獲得した点数を表示
            if (this.scoreText)
            {
                this.scoreText.GetComponent<Text>().text = "Score " + this.score + "pt";
            }

            //パーティクルを再生
            ParticleSystem psystem = this.gameObject.GetComponent<ParticleSystem>() as ParticleSystem;
            if(psystem != null){ psystem.Play(); }

            //接触したコインのオブジェクトを破棄
            Destroy(other.gameObject);




            //            (this.gameObject.GetComponent<ParticleSystem>() as ParticleSystem).Stop();
        }
    }


    //--------------------------------------------------------------------------------
    //ジャンプボタンを押した場合の処理
    public void GetMyJumpButtonDown()
    {
        if(this.transform.position.y < 0.5f)
        {
            this.myAnimator.SetBool("Jump", true);
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }
    }

    //左ボタンを押し続けた場合の処理
    public void GetMyLeftButtonDown()
    {
        this.isLButtonDown = true;
    }
    //左ボタンを離した場合の処理
    public void GetMyLeftButtonUp()
    {
        this.isLButtonDown = false;
    }

    //右ボタンを押し続けた場合の処理
    public void GetMyRightButtonDown()
    {
        this.isRButtonDown = true;
    }
    //右ボタンを離した場合の処理
    public void GetMyRightButtonUp()
    {
        this.isRButtonDown = false;
    }



    private void WriteUITextResult(string str)
    {
        if (stateText)
        {
            UnityEngine.UI.Text txt = stateText.GetComponent<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
            if (txt != null)
            {
                txt.text = str;
            }
            else
            {
//                System.Runtime.CompilerServices.caller
                Debug.Log("collision CarTag or TrafficConeTage / GetComponent(Text) is failed.");
            }
        }
    }
}





















