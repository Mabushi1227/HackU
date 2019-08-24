using UnityEngine;
using System.Collections;
using VRM;


// Require these components when using this script
[RequireComponent(typeof(Animator))]

/* VRMモデルのモーションを制御するクラス */
public class MotionController : MonoBehaviour
{
	/* 各種関数で使用する変数  publicのものはunityエディタ上でアタッチする　*/
	private Animator anim;			// Animatorへの参照
	public float _interval = 10f;		// ランダム判定のインターバル
	GameObject model;			//パラメータを反映させるモデル
    	public GameObject ivent;		//PlayerInfoをアタッチしたゲームオブジェクト			
    	public VRMBlendShapeProxy Proxy;	//モデルのBlendShape(表情)変化に使用
	int actionLength = 7;   		//アクション数
	string comment;				//リアクション再生中に表示するコメント
	private bool actionset;			//リアクション再生中
	string process = "";			//リアクション再生ボタンに表示するメッセージ
	PlayerInfo info;			//プレイヤーのパラメーターを取得するPlayerInfoクラス
	public GameObject ui;			//コメント表示の時に表示する吹き出し
	GUIStyle style = new GUIStyle();        //UIの表示設定
	public Font font;			//UIのフォント

		// Use this for initialization
	void Start ()
	{
		/* 各参照の初期化 */

		//animatorへの参照取得
		anim = GetComponent<Animator> ();
		//モーションを再生中:false 
		actionset = true;
		//processは実行ボタンに表示する文字列
		process = "Play!!";
    	info = ivent.GetComponent<PlayerInfo>();
		// Position the Texture in the center of the Box
    	style.alignment = TextAnchor.MiddleCenter;
		//コメントのフォント設定-unityエディタ上で設定(日本語対応フォントを設定するため)
		style.font = font;
		//コメントのフォント
		style.fontSize = 20;
		//コメント用のUIはコメントを表示させている時のみ表示
		ui.SetActive(false);
	}

	/* 各UIを表示する関数 */
	void OnGUI ()
	{
		GUI.Box (new Rect (Screen.width - 110, 20, 100, 90), "Commands");
		//リアクション再生ボタン
		if (GUI.Button (new Rect (Screen.width - 100, 80, 80, 20), process))
        {
			Transform model_t = this.transform;
			model_t.eulerAngles = new Vector3(0,0,0);
			if (Proxy == null)
        	{
        		Proxy = model.GetComponent<VRMBlendShapeProxy>();
        		return;
    		}else{
				//リアクションを再生する
				StartCoroutine("Reaction");
			}

        }

		//リアクション再生中に吹き出しuiとコメントを表示
		if(!actionset){
			ui.SetActive(true);
			GUI.Box(new Rect(Screen.width / 4, Screen.height - 120, Screen.width / 2, 50), comment, style);
		}else{
			ui.SetActive(false);
		}

    }

	/* リアクションを再生する関数 */
	IEnumerator Reaction(){
		//リアクション再生をしていない時のみ再生
		if(actionset){
			//リアクションを再生中に変更
			actionset = false;	
			process = "playng";

			//ランダムで再生するリアクションを選択	
			int _seed = Random.Range (1,actionLength+1);

			//リアクションの再生
			anim.SetInteger("action",_seed);

			//パラメータ評価のための変数
			float tmpData;

			//リアクションの表情と競合しないために表情を設定
			MorphClean();

			//リアクションごとの設定
			switch (_seed)
			{
				case 1:
					tmpData = info.GetSlim();
					Proxy.ImmediatelySetValue(BlendShapePreset.Angry,1.0f);
					comment = (tmpData > 0.5 ? "食事をしっかり取って!!\n身体は資本だよ!!": "私も連れてって!!");
					break;
				case 2:
					tmpData = info.GetBad();
					Proxy.ImmediatelySetValue("BAD",1.0f);
					comment = (tmpData > 0.5 ? "栄養が偏ってる...\nバランスの良い食事を心がけて!!" : "うぅ どうして名古屋はこんなに暑いの...");
					break;
				case 3:
					tmpData = info.GetGood();
					Proxy.ImmediatelySetValue(BlendShapePreset.Fun,1.0f);
					comment = (tmpData > 0.5 ? "いい感じだよ!!\n絶好調だね!!": "明日、晴れるかな??");
					break;
				case 4:
					tmpData = info.GetSleepy();
					Proxy.ImmediatelySetValue(BlendShapePreset.A,1.0f);
					Proxy.ImmediatelySetValue(BlendShapePreset.Blink,1.0f);
					comment = (tmpData > 0.5 ? "ふぁ〜、最近あんまり寝られてないね...\n今日は早く寝ない??": "ふぁ〜、眠くなってくる時間帯だね");
					break;
				case 5:	
					tmpData = info.GetFat() - info.GetSlim();
					Proxy.ImmediatelySetValue(BlendShapePreset.A,1.0f);
					Proxy.ImmediatelySetValue("SLEEPY",0.5f);
					Proxy.ImmediatelySetValue("ZITO",0.5f);
					comment = (tmpData > 0.5 ? "食べ過ぎ!!\nこのままだと危ないよ!!": "(...ここは、どこ??)");
					break;
				case 6:
					tmpData = info.GetFat();
					Proxy.ImmediatelySetValue(BlendShapePreset.Joy,1.0f);
					comment = (tmpData > 0.5 ? "食べ過ぎじゃない？\n体重計乗ってみよ!!": "こんな日には外に出て見よう!!\nいいことあるよ!!");
					break;
				case 7:
					tmpData = info.GetGood() - info.GetBad() - info.GetSleepy();
					if(tmpData > 0.5){
						Proxy.ImmediatelySetValue("DOYA", 1.0f);
						comment = "言うことなし!!君は完璧だよ";
					 }else{
						Proxy.ImmediatelySetValue(BlendShapePreset.O, 0.7f);
						comment = "おーい、見えてますかー？";
					 }
					break;
				default:		//エラー
					tmpData = info.GetFat();
					Proxy.ImmediatelySetValue(BlendShapePreset.Angry,1.0f);
					comment = (tmpData > 0.5 ? "エラーかな??": "エラーっぽいね");
				break;
			}
		}else{
			//すでにリアクション再生中の時の処理
			process = "Please Wait";
		}
		// _interval秒まつ
		yield return new WaitForSeconds (_interval);

		StartCoroutine("forIdle");
		MorphClean();
	}

	//Idleへ戻る関数
	IEnumerator forIdle(){
		//idleモーションの再生
		anim.SetInteger("action",0);
			
		//1.5秒待つ
		yield return new WaitForSeconds (1.5f);

		//待機状態への処理
		actionset = true;
		process = "Play!!";

		//正面を向くための処理
		Transform model_t = this.transform;
		model_t.eulerAngles = new Vector3(0,0,0);
	}

		//表情が競合しないように各使用BlendShapeの値を設定、値の再取得
	void MorphClean(){
		Proxy.ImmediatelySetValue(BlendShapePreset.Angry,0.0f);
		Proxy.ImmediatelySetValue("BAD",info.GetBad());
		Proxy.ImmediatelySetValue(BlendShapePreset.Fun,0.0f);
		Proxy.ImmediatelySetValue(BlendShapePreset.O,0.0f);
		Proxy.ImmediatelySetValue(BlendShapePreset.A,0.0f);
		Proxy.ImmediatelySetValue(BlendShapePreset.Joy,0.0f);
		Proxy.ImmediatelySetValue("SLEEPY",info.GetSleepy());
		Proxy.ImmediatelySetValue("DOYA",0.0f);
		Proxy.ImmediatelySetValue("ZITO",0.0f);
	}
}
