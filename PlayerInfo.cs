using UnityEngine;
using System;
using VRM;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

/* プレイヤーのパラメータ */
[System.Serializable]
public class PlayerData
{
    public float fat;       //カロリー多い
    public float slim;      //カロリー少ない
    public float sleepy;    //睡眠少ない
    public float good;      //栄養バランス良い
    public float bad;       //バランス悪い

}

/*  */
public class PlayerInfo : MonoBehaviour
{

    /* このクラスで使用する変数 */

    //パラメータを反映させるVRMモデル
    GameObject model;

    //PlayerInfoクラスをアタッチするゲームオブジェクト
    public GameObject ivent;

    //モデルのBlendShape(表情)
    public VRMBlendShapeProxy Proxy;

    //モデルをキラキラさせるゲームオブジェクト
    public GameObject star1;
    public GameObject star2;

    //WebRequestから受け取ったデータ
    public string inputData = "";
    
    //ログに表示する文字列
    public string output = "";

    //PlayerDataクラス変数
    PlayerData  data;

    /* パラメータのゲッタ */
    public float GetFat(){ return data.fat;}
    public float GetSlim(){ return data.slim;}
    public float GetSleepy(){ return data.sleepy;}
    public float GetGood(){ return data.good;}
    public float GetBad(){ return data.bad;}

    //WebRequet先のURL
    public string url = "http://localhost:8080/test.json";

    /* UIを描画する関数 */
    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 100, 40, 80, 20), "Log Open"))
        {
            //Log Openを押すとdataPath関数を実行する
            ivent.SendMessage("dataPath", url);
        }
        if (GUI.Button(new Rect(Screen.width - 100, 60, 80, 20), "Log Clean"))
        {
            output = "";
        }
        GUILayout.Space(32);
        GUILayout.Box(output, GUILayout.Height(128));
    }

    public void dataPath(string reqUrl)
    {
        url = reqUrl;
        if (Proxy == null)
        {
            Proxy = model.GetComponent<VRMBlendShapeProxy>();
            return;
        }
        else
        {
            //データの受信
            StartCoroutine(Receive(reqUrl));
            output += "Receive url \n";
            output += reqUrl + "\n";
        }
    }

    private IEnumerator Receive(string reqUrl)
    {

        //1.UnityWebRequestを生成
        UnityWebRequest request = UnityWebRequest.Get(reqUrl);

        //2.SendWebRequestを実行し、送受信開始
        yield return request.SendWebRequest();

        //3.isNetworkErrorとisHttpErrorでエラー判定
        if (request.isHttpError || request.isNetworkError)
        {
            //4.エラー確認
            //Debug.Log(request.error);
            output += request.error + "\n";
        }
        else
        {
            //4.結果確認
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                // UTF8文字列として取得する
                inputData = request.downloadHandler.text;
                output += "data = " + inputData + "\n";

                //受信したjsonデータをパラメータにPlayerData変数に変換する
                data = JsonUtility.FromJson<PlayerData>(inputData);

                //パラメータをモデルに反映させる
                Proxy.ImmediatelySetValue("FAT", data.fat);
                Proxy.ImmediatelySetValue("SLIM", data.slim);
                Proxy.ImmediatelySetValue("SLEEPY", data.sleepy);
                Proxy.ImmediatelySetValue("BAD", data.bad);

                /* goodが0.5以上ならキラキラを表示、未満ならキラキラを消す */
                if (data.good >= 0.5)
                {
                    star1.SetActive(true);
                    star2.SetActive(true);

                    float s1 = (data.good - data.bad) * 4.0f;
                    float s2 = (data.good - data.bad) * 2.0f;

                    star1.transform.localScale = new Vector3(s1, s1, s1);
                    star2.transform.localScale = new Vector3(s2, s2, s2);
                }
                else
                {
                    star1.SetActive(false);
                    star2.SetActive(false);
                }
                
            }
        }
    }
}