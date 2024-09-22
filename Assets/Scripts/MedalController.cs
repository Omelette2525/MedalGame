using System.Collections;
using System.Collections.Generic;
using CommonConst;
using UnityEngine;

public class MedalController : MonoBehaviour
{
    /* prefabにシーン内のオブジェクトをアタッチすることはできないので、生成したメダルはアタッチが外れている */
    [SerializeField] PlayerDataManager playerDataScript; // 落下時に所持メダルを増やすためにアタッチ
    [SerializeField] SlotManager slotScript; // スロットストックを増やすためにアタッチ
    [SerializeField] FieldManager fieldScript; // outMedalを増やすためにアタッチ
    [SerializeField] SoundController soundScript; // 音を鳴らすためにアタッチ
    [SerializeField] float boaderZ; // 横穴に落ちたかどうかはz軸で判定
    [SerializeField] float boaderY; // 一定の高さまで落ちたメダルを消去する
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < boaderY) // メダルが落ちた
        {
            if(gameObject.transform.position.z < boaderZ) // 手前側で落ちたらメダルゲット
            {
                playerDataScript.MedalProperty++; // 持ちメダルを増やす
                fieldScript.OutMedalProperty++; // outMedalを増やす
                //Debug.Log("持ちメダルは" + playerDataScript.medal + "枚");
                soundScript.PlaySE(CommonConstManager.MEDALGET); // SEを鳴らす
            }
            Destroy(gameObject); // メダルを消去
        }
    }

    /* 生成する際に外れたアタッチをつける */
    public void MedalSetUp(PlayerDataManager getPlayerScript, SlotManager getSlotScript, FieldManager getFieldScript, SoundController getSoundScript)
    {
        /* 受け取ったスクリプトをセットする */
        playerDataScript = getPlayerScript;
        slotScript = getSlotScript;
        fieldScript = getFieldScript;
        soundScript = getSoundScript;
    }

    /* スロットチェッカーを通過したらスロットストックを増やす */
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SlotChecker"))
        {
            slotScript.SlotStockProperty++;
        }        
    }
}
