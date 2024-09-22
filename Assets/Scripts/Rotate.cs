using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ただ回転させるだけのスクリプト 内部角度などは記録しない */
public class Rotate : MonoBehaviour
{
    [SerializeField] Transform rotateObj; // 回転させるオブジェクトをアタッチ
    [SerializeField] Vector3 rotateSpeed; // どの軸でどのくらい回転させるか
    [SerializeField] bool rotateFlag = true; // 回転させるかどうか falseでストップ
    private Vector3 initSpeed; // 速度の初期値 startで取得する
    // Start is called before the first frame update
    void Start()
    {
        initSpeed = rotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateFlag == true)
        {
            Vector3 angle = rotateSpeed * Time.deltaTime; // 回転させる角度を決める ゲーム内時間と同期する
            rotateObj.Rotate(angle);  // angle分回転させる
        }
    }

    /* 外部から回転を制御するためのプロパティ */
    public bool RotateFlagProperty
    {
        set
        {
            rotateFlag = value;
        }
    }
    public Vector3 RotateSpeedProperty
    {
        set
        {
            rotateSpeed = value;
        }
    }
    /* 回転速度の初期値を取得するためのプロパティ */
    public Vector3 InitSpeedProperty
    {
        get
        {
            return initSpeed;
        }
    }
}
