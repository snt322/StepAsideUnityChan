using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 発展課題：ユニティちゃんの位置に応じてアイテムを動的に生成しましょう
 * ItemGeneratorクラスのメソッドpublic void UpdateItem(float unitychanPosZ)
 *
 */

public class ItemGenerator : MonoBehaviour {

    //CarPrefabを入れる
    public GameObject carPrefab;
    //coinPrefabを入れる
    public GameObject coinPrefab;
    //cornPrefabを入れる
    public GameObject conePrefab;
    //スタート地点
    private int startPos = -160;
    //ゴール地点
    private int goalPos = 120;
    //アイテムを出すx方向の範囲
    private float posRange = 3.4f;

    //発展課題
    //Lesson6内では一定の距離ごとにアイテムを生成している。発展課題でも生成する距離は同じく一定間隔(itemGenerateSpan = 15づつ)にする。
    //UnityちゃんがZ方向に+15移動する毎ににアイテムを生成する
    //UnityちゃんのZ座標>itemGeneratePosZとなったらアイテム製造し、Unityちゃんがゴール付近[ (goalPos - itemGeneratorCallSpan) <= UnityちゃんのZ座標 ]となったらアイテムの生成を停止する。

    private int nextItemGeneratePos = -180;
    private int itemGeneratorCallSpan = 60;
    private int itemGenerateSpan = 15;




	// Use this for initialization
	void Start ()
    {
        //5.12 車・カラーコーン・コインをゲーム中に生成するスクリプトを作成
        //Start()関数内で最初にすべてのアイテムを生成する場合
//        CreateItem(startPos, goalPos, 15);

    }


	
	// Update is called once per frame
	void Update () {
		
	}

    //unitychanPosZはUnityちゃんのz座標
    public void UpdateItem(float unitychanPosZ)
    {
        float tmpItemGeneratePosZ = unitychanPosZ + itemGeneratorCallSpan;                          //現在のUnityちゃんのz座標に+itemGeneratorCallSpanを加える //Unityちゃんz座標+itemGeneratorCallSpan位置からアイテム生成を開始する。

        bool isReachGeneratePosZ = (float)nextItemGeneratePos <= tmpItemGeneratePosZ;               //アイテムを生成するz座標にUnityちゃんが到達している?

        if (isReachGeneratePosZ)
        {

            int tmpGenerateStart = nextItemGeneratePos;                                     //アイテムの生成開始位置
            int tmpGenerateEnd   = tmpGenerateStart + itemGeneratorCallSpan;                //アイテムの生成終了位置
            if(tmpGenerateEnd >= goalPos) { tmpGenerateEnd = goalPos - 10; }                //生成位置終了位置がgoalPosを超える場合

            nextItemGeneratePos += itemGeneratorCallSpan;                                   //次にアイテム生成するz座標を更新


            //ここから下は「5.12 車・カラーコーン・コインをゲーム中に生成するスクリプトを作成」と全く同じです
            CreateItem(tmpGenerateStart, tmpGenerateEnd, itemGenerateSpan);

        }//if
    }

    //5.12 車・カラーコーン・コインをゲーム中に生成するスクリプトを作成
    private void CreateItem(int start, int end, int span)
    {

        for (int i = start; i < end; i += span)
        {
            //どのアイテムを出すのかをランダムに設定
            int num = Random.Range(1, 11);
            if (num <= 2)
            {
                //コーンをx軸方向に一直線に生成
                for (float j = -1; j <= 1; j += 0.4f)
                {
                    GameObject cone = Instantiate(conePrefab) as GameObject;
                    cone.transform.position = new Vector3(4 * j, cone.transform.position.y, i);
                    cone.AddComponent<ItemDestroyer>();
                }
            }
            else
            {
                //レーンごとにアイテムを生成
                for (int j = -1; j <= 1; j++)
                {
                    //アイテムの種類を決める
                    int item = Random.Range(1, 11);
                    //アイテムを置くZ座標のオフセットをランダムに設定
                    int offsetZ = Random.Range(-5, 6);
                    //60%コイン配置：30%車配置：10%何もなし
                    if (1 <= item && item <= 6)
                    {
                        //コインを生成
                        GameObject coin = Instantiate(coinPrefab) as GameObject;
                        coin.transform.position = new Vector3(posRange * j, coin.transform.position.y, i + offsetZ);
                        coin.AddComponent<ItemDestroyer>();
                    }
                    else if (7 <= item && item <= 9)
                    {
                        //車を生成
                        GameObject car = Instantiate(carPrefab) as GameObject;
                        car.transform.position = new Vector3(posRange * j, car.transform.position.y, i + offsetZ);
                        car.AddComponent<ItemDestroyer>();
                    }
                }
            }//else
        }//for
    }
}
