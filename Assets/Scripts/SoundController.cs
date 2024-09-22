using System.Collections;
using System.Collections.Generic;
using CommonConst;
using Unity.Burst;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    /* audioclipをインスペクタでアタッチする */
    [SerializeField] AudioClip medalGetSE;
    [SerializeField] AudioClip medalThrowSE;
    [SerializeField] AudioClip slotStartSE;
    [SerializeField] AudioClip reelStopSE;
    [SerializeField] AudioClip slotSuccessSE;
    [SerializeField] AudioClip ballFallSE;
    [SerializeField] AudioClip pocketInSE;
    [SerializeField] AudioClip eventBallGenSE;

    [SerializeField] AudioSource audioSourceSE;
    Dictionary<int, AudioClip> soundDicSE = new Dictionary<int, AudioClip>();
    
    // Start is called before the first frame update
    void Start()
    {
        /* se辞書にkeyとvalueを登録 */
        soundDicSE.Add(CommonConstManager.MEDALGET, medalGetSE);
        soundDicSE.Add(CommonConstManager.MEDALTHROW, medalThrowSE);
        soundDicSE.Add(CommonConstManager.SLOTSTART, slotStartSE);
        soundDicSE.Add(CommonConstManager.REELSTOP, reelStopSE);
        soundDicSE.Add(CommonConstManager.SLOTSUCCESS, slotSuccessSE);
        soundDicSE.Add(CommonConstManager.BALLFALL, ballFallSE);
        soundDicSE.Add(CommonConstManager.POCKETIN, pocketInSE);
        soundDicSE.Add(CommonConstManager.EVENTBALLGEN, eventBallGenSE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /* 指定したSEを流す */
    public void PlaySE(int key)
    {
        if(soundDicSE.TryGetValue(key, out AudioClip value)) // keyに対応する値を取得できれば実行 失敗したら実行しない
        {
            audioSourceSE.PlayOneShot(value);
        }
        else
        {
            Debug.Log("se辞書に指定のサウンドが登録されていません[SoundController]"); // debug用
        }
    }
}
