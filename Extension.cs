using Il2CppAssets.Scripts.Database;
using Il2CppBmsPair = Il2CppSystem.Collections.Generic.KeyValuePair<string, Il2Cpp.SpecialSongManager.HideBmsInfo>;
using Object = UnityEngine.Object;

namespace HiddenQol;

public static class Extension
{
    public static void Deconstruct(this Il2CppBmsPair keyValuePair, out string key, out SpecialSongManager.HideBmsInfo value)
    {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
    }

    public static void Destroy(this Component component) => Object.Destroy(component);

    public static void SetMusicInfo(this DBMusicTag dbmt, string uid, MusicInfo info) => dbmt.m_AllMusicInfo[uid] = info;
}