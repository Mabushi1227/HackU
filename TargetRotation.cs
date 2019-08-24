using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ゲームオブジェクトを回す */
public class TargetRotation : MonoBehaviour
{
    //回転角度の設定　エディタ上で設定する
    public float xAngle;
    public float yAngle;

    public float zAngle;

    // Update is called once per frame
    void Update()
    {
        // transformを取得
        Transform myTransform = this.transform;
        myTransform.Rotate(xAngle, yAngle, zAngle, Space.World);

    }
}
