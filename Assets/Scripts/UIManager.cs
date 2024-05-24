using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    private PlayerDataManager playerDataScript;
    private int currentMedal = 0; // 持ちメダルに変化があったか検出するための変数 とりあえず0を入れておく
    [SerializeField] private TMP_Text medalText;
    private string medalFormat; // format形式で最初のテキストを読み込み、それに従って描画する
    // Start is called before the first frame update
    void Start()
    {
        playerDataScript = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
        medalFormat = medalText.text;
    }
    // Update is called once per frame
    void Update()
    {
        if(currentMedal != playerDataScript.medal) // 持ちメダルが変化したら
        {
            medalText.text = string.Format(medalFormat, playerDataScript.medal); // テキストを更新 フォーマットを使うことで普通の文字部分には影響しない
        }
        currentMedal = playerDataScript.medal; // メダル枚数を更新
    }
}
