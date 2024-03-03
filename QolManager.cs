using Il2CppAssets.Scripts.Database;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppAssets.Scripts.PeroTools.GeneralLocalization;
using Il2CppAssets.Scripts.PeroTools.Nice.Events;
using Il2CppAssets.Scripts.PeroTools.Nice.Variables;
using UnityEngine.Events;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

namespace HiddenQol;

internal static class QoLManager
{
    internal static PnlStage Stage { get; set; }
    internal static GameObject QolToggle { get; set; }

    internal static void ActivateAllHidden()
    {
        var instance = Singleton<SpecialSongManager>.instance;

        YumeOuManager.ActivateYume();

        instance.m_HideBmsInfos["4-5"] = new SpecialSongManager.HideBmsInfo("4-5", 2, 4, "goodtek_map4");
        instance.m_HideBmsInfos["8-3"] = new SpecialSongManager.HideBmsInfo("8-3", -1, 4, "sweet_witch_girl_map4");
        instance.SetBarrageMode(true);

        foreach (var (_, value) in instance.m_HideBmsInfos)
        {
            if (!instance.IsInvokeHideBms(value.uid))
            {
                ActivateHidden(value);
            }
        }

        Stage.OnTriggerHideBmsEvent();
        Stage.musicFancyScrollView.onItemIndexChange.Invoke(GlobalDataBase.dbMusicTag.curSelectedMusicIdx);
    }

    private static void ActivateHidden(SpecialSongManager.HideBmsInfo hideBmsInfo)
    {
        if (hideBmsInfo == null)
        {
            MelonLogger.Error("ActivateHidden: HideBmsInfo cannot be null!");
            return;
        }

        var instance = Singleton<SpecialSongManager>.instance;
        var musicInfo = GlobalDataBase.dbMusicTag.GetMusicInfoFromAll(hideBmsInfo.uid);
        var triggerDiff = hideBmsInfo.triggerDiff;

        var maskDifficultyName = $"difficulty{triggerDiff}";
        var maskLevelDesignerName = $"levelDesigner{triggerDiff}";

        if (triggerDiff == -1)
        {
            instance.InvokeHideBms(musicInfo, true);
        }

        var (difficultyText, levelDesignerText) = GetDifficultyAndLevelDesigner(musicInfo, hideBmsInfo.m_HideDiff);

        musicInfo.m_MaskValue[maskDifficultyName] = difficultyText;
        musicInfo.m_MaskValue[maskLevelDesignerName] = levelDesignerText;

        musicInfo.SetDifficulty(triggerDiff, hideBmsInfo.m_HideDiff);
        instance.m_IsInvokeHideDic[hideBmsInfo.uid] = true;
    }

    private static (string, string) GetDifficultyAndLevelDesigner(MusicInfo musicInfo, int hideDiff)
    {
        var difficultyText = "?";
        var levelDesignerText = musicInfo.levelDesigner;
        switch (hideDiff)
        {
            case 1:
                difficultyText = musicInfo.difficulty1;
                levelDesignerText = musicInfo.levelDesigner1 ?? musicInfo.levelDesigner;
                break;
            case 2:
                difficultyText = musicInfo.difficulty2;
                levelDesignerText = musicInfo.levelDesigner2 ?? musicInfo.levelDesigner;
                break;
            case 3:
                difficultyText = musicInfo.difficulty3;
                levelDesignerText = musicInfo.levelDesigner3 ?? musicInfo.levelDesigner;
                break;
            case 4:
                difficultyText = musicInfo.difficulty4;
                levelDesignerText = musicInfo.levelDesigner4 ?? musicInfo.levelDesigner;
                break;
        }

        return (difficultyText, levelDesignerText);
    }

    internal static void DeactivateAllHidden()
    {
        var instance = Singleton<SpecialSongManager>.instance;

        YumeOuManager.DisableYume();

        instance.m_HideBmsInfos["4-5"] = new SpecialSongManager.HideBmsInfo("4-5", 2, 4, "goodtek_map4",
            new Func<bool>(() => instance.m_GameInTime is { Month: 4, Day: 1 }));
        instance.m_HideBmsInfos["8-3"] = new SpecialSongManager.HideBmsInfo("8-3", -1, 4, "sweet_witch_girl_map4",
            new Func<bool>(() => instance.m_GameInTime is { Month: 11, Day: 1 }));
        instance.SetBarrageMode(false);

        foreach (var (_, value) in instance.m_HideBmsInfos)
        {
            if (instance.IsInvokeHideBms(value.uid))
            {
                DeactivateHidden(value);
            }
        }

        Stage.OnTriggerHideBmsEvent();
        Stage.musicFancyScrollView.onItemIndexChange.Invoke(GlobalDataBase.dbMusicTag.curSelectedMusicIdx);
    }

    private static void DeactivateHidden(SpecialSongManager.HideBmsInfo hideBms)
    {
        GlobalDataBase.dbMusicTag.GetMusicInfoFromAll(hideBms.uid).ClearMaskValue();
        Singleton<SpecialSongManager>.instance.m_IsInvokeHideDic[hideBms.uid] = false;
    }

    internal static void SetupToggle()
    {
        QolToggle.name = "TglHiddenQoL";
        var text = QolToggle.transform.Find("Txt").GetComponent<Text>();
        var foreground = QolToggle.transform.Find("Background").GetComponent<Image>();
        var background = QolToggle.transform.Find("Background").GetChild(0).GetComponent<Image>();
        var toggle = QolToggle.GetComponent<Toggle>();
        QolToggle.transform.position = new Vector3(-7f, -5f, 100f);

        QolToggle.GetComponent<OnToggle>().Destroy();
        QolToggle.GetComponent<OnToggleOn>().Destroy();
        QolToggle.GetComponent<OnActivate>().Destroy();
        QolToggle.GetComponent<VariableBehaviour>().Destroy();

        text.GetComponent<Localization>().Destroy();
        toggle.group = null;
        toggle.SetIsOnWithoutNotify(Setting.QolEnabled);
        toggle.onValueChanged.AddListener((UnityAction<bool>)
            ((bool val) =>
            {
                Setting.QolEnabled = val;
                if (val)
                {
                    ActivateAllHidden();
                }
                else
                {
                    DeactivateAllHidden();
                }
            }));
        text.text = "HIDDEN MODE";
        text.color = new Color(1f, 1f, 1f, 0.298f);
        var rectTransform = text.transform.Cast<RectTransform>();
        var offsetMax = rectTransform.offsetMax;
        rectTransform.offsetMax = new Vector2(text.preferredWidth + 10f, offsetMax.y);
        foreground.color = new Color(0.23529412f, 0.15686275f, 0.43529412f);
        background.color = new Color(0.40392157f, 0.3647059f, 0.50980395f);
    }
}