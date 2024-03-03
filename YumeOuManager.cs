using Il2CppAssets.Scripts.Database;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPeroPeroGames.GlobalDefines;

namespace HiddenQol
{
    internal static class YumeOuManager
    {
        private static MusicInfo HiddenOuSong { get; set; }
        private static MusicInfo BaseOuSong { get; set; }

        private static void GetInfo()
        {
            HiddenOuSong ??= GlobalDataBase.dbMusicTag.GetMusicInfoFromAll("0-53");
            BaseOuSong ??= GlobalDataBase.dbMusicTag.GetMusicInfoFromAll("0-54");
        }

        private static void SetBaseAsHidden(bool useHidden)
        {
            if (BaseOuSong == null || HiddenOuSong == null) return;

            var tempInfo = BaseOuSong;
            if (useHidden) tempInfo = HiddenOuSong;

            GlobalDataBase.dbMusicTag.SetMusicInfo(BaseOuSong.uid, tempInfo);
            GlobalDataBase.dbMusicTag.SetMusicInfo(HiddenOuSong.uid, tempInfo);
        }

        private static void RemoveFromCulling()
        {
            var cullingArr = DBMusicTagDefine.s_CullingMusicUids;
            DBMusicTagDefine.s_CullingMusicUids = cullingArr.Where(x => x != "0-53").ToArray();
        }

        private static void AddToCulling()
        {
            if (DBMusicTagDefine.s_CullingMusicUids.Contains("0-53")) return;

            var cullingArr = DBMusicTagDefine.s_CullingMusicUids;
            var newCullingArray = new Il2CppStringArray(cullingArr.Length + 1);

            for (var i = 0; i < cullingArr.Length; i++)
            {
                newCullingArray[i] = cullingArr[i];
            }
            newCullingArray[^1] = "0-53";
            DBMusicTagDefine.s_CullingMusicUids = newCullingArray;
        }

        internal static void ActivateYume()
        {
            GetInfo();
            RemoveFromCulling();
            SetBaseAsHidden(true);
        }

        internal static void DisableYume()
        {
            AddToCulling();
            SetBaseAsHidden(false);
        }
    }
}
