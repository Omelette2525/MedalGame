using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonConst
{
    /* 定数管理用のクラス */
    public static class CommonConstManager
    {
        /* イベントの順番管理用 入力時と、判定時に使う*/
        public const int SC = 1;
        public const int BRIGHTJPC = 2;
        public const int SHADOWJPC = 3;

        /* jpcのポケット識別用 */
        public const int POCKET50 = 50;
        public const int POCKET100 = 100;
        public const int POCKET200 = 200;
        public const int POCKET300 = 300;
        public const int POCKETJP = 1;
        public const int POCKETOUT = 2;

        /* メダル補給にかかる時間 UIManagerとPlayerDataManagerで使う */
        public const int SUPPLYTIME = 6000;

        /* どんな音をならすか識別するのに使う定数 */
        public const int MEDALGET = 1;
        public const int MEDALTHROW = 2;
        public const int SLOTSTART = 3;
        public const int REELSTOP = 4;
        public const int SLOTSUCCESS = 5;
        public const int BALLFALL = 6;
        public const int POCKETIN = 7;
        public const int EVENTBALLGEN = 8;
    }
}
