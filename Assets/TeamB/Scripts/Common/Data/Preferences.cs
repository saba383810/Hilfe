using Fresh23_N.Scripts.Extensions;
using TeamB.Scripts.InGame.Matching;
using UnityEngine;

public static class Preferences
{
    private const string PlayerNameKey = "PLAYER_NAME_KEY";
    private const string PlayerIdKey = "PLAYER_ID_KEY";
    private const string HighScoreKey = "HighScoreKey";
    private const string BGMBaseVolume = "BGMBaseVolume";
    private const string SEBaseVolume = "SEBaseVolume";
    private const string TutorialKey = "TutorialKey";
    private const string RankingKey = "RankingKey";
    private const string MelogardiaKey = "MelogardiaKey";
    private const string GrimyKey = "GrimyKey";
    private const string FangKey = "FangcuriteKey";

    /// <summary>
    ///   playerの名前を取得
    /// </summary>
    /// <returns>データがなかったらnullを返す</returns>
    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, null);
    }

    public static string GetPlayerId()
    {
        return PlayerPrefs.GetString(PlayerIdKey, null);
    }

    /// <summary>
    ///   Playerの名前を保存
    /// </summary>
    /// <param name="val"></param>
    public static void SetPlayerName(string val)
    {
        PlayerPrefs.SetString(PlayerNameKey, val);
    }
    
    public static void SetPlayerId(string val)
    {
        PlayerPrefs.SetString(PlayerIdKey, val);
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static void SetHighScore(int val)
    {
        PlayerPrefs.SetInt(HighScoreKey,val);
    }

    public static float GetBgmBaseVolume()
    {
        return PlayerPrefs.GetFloat(BGMBaseVolume, 0.4f);
    }
    
    public static void SetBgmBaseVolume(float volume)
    {
        PlayerPrefs.SetFloat(BGMBaseVolume, volume);
    }
    
    public static float GetSeBaseVolume()
    {
        return PlayerPrefs.GetFloat(SEBaseVolume, 0.8f);
    }
    
    public static void SetSeBaseVolume(float volume)
    {
        PlayerPrefs.SetFloat(SEBaseVolume, volume);
    }

    public static bool GetTutorial()
    {
        return PlayerPrefsExtensions.GetBool(TutorialKey, false);
    }

    public static void SetTutorial(bool isTutorial)
    {
        PlayerPrefsExtensions.SetBool(TutorialKey, isTutorial);
    }

    public static bool GetRanking()
    {
        return PlayerPrefsExtensions.GetBool(RankingKey, false);
    }
    public static void SetRanking(bool isActive)
    {
        PlayerPrefsExtensions.SetBool(RankingKey, isActive);
    }

    public static VotePercentData GetVotePercentData()
    {
        var grimy = PlayerPrefs.GetFloat(GrimyKey, 33.3f);
        var melogardia = PlayerPrefs.GetFloat(MelogardiaKey, 33.3f);
        var fangqulite = PlayerPrefs.GetFloat(FangKey, 33.3f);
        return new VotePercentData(grimy, melogardia, fangqulite);
    }

    public static void SetVotePercentData(VotePercentData data)
    {
        PlayerPrefs.SetFloat(GrimyKey, data.Grimy);
        PlayerPrefs.SetFloat(MelogardiaKey, data.Melogardia);
        PlayerPrefs.SetFloat(FangKey, data.Fangqulite);
    }
}
