using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRotate : MonoBehaviour
{
    [SerializeField] Transform rotateObj; // 回転させるオブジェクトをアタッチ
    [SerializeField] Vector3 rotateInit = new Vector3(0, 450, 90); // 初期化時の値
    [SerializeField] float rotateSpeed; // 回転速度
    [SerializeField] bool rotateFlag = true; // 回転させるかどうか falseでストップ
    private Vector3 currentAngle; // 内部角度
    // Start is called before the first frame update
    void Start()
    {
        // currentAngle = rotateObj.localEulerAngles; // start時の角度を取得
        currentAngle = new Vector3(0, 0, 0); // ゲームスタート時に真上を向いているようにして、0度スタートとする
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateFlag == true)
        {
            float angle = rotateSpeed * Time.deltaTime; // 回転させる角度を決める ゲーム内時間と同期する
            rotateObj.Rotate(0, angle, 0);  // x軸でangle度回転させる
            currentAngle.x += angle; // 回転させた分内部角度を増やす
            if(currentAngle.x >= 360) // 360度を超えたら内部角度を0度に戻す
            {
                currentAngle.x -= 360;
            }
        }
    }

    /* 外部のためのプロパティ 用途によってgetのみ、setのみのプロパティがある */
    public bool RotateFlagProperty // 外部から回っているかの確認はしない
    {
        set
        {
            rotateFlag = value;
        }
    }

    public Vector3 CurrentAngleProperty // 外部から内部角度を変更しない
    {
        get
        {
            return currentAngle;
        }
    }

    /* 角度を初期化するメソッド */
    public void RotateAngleInit() 
    {
        rotateObj.localEulerAngles = rotateInit;
        currentAngle = rotateInit;
    }
}
