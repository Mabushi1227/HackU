## HackU Nagoya 2019 にて開発した VHealth(unityサイド)のスクリプトです。

### PlayerInfo.cs
    ・プレイヤー情報からサーバー側が計算したパラメータの取得、モデルへの反映
    ・"UnityEngine.Networking"を使用してHTTP通信を行う

### MotionController.cs
    ・モデルのモーションの制御
    ・PlayerInfoのパラメータによって表示コメントを変更する
    
### TargetRotaion.cs
    ・オブジェクトを回転させる
