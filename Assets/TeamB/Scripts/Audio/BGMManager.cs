namespace Sabanogames.AudioManager {

using UnityEngine;

/// <summary>
/// BGM関連の管理をするクラス
/// </summary>
public class BGMManager : AudioManager<BGMManager> {

  //AudioPlayerの数(同時再生可能数)
  protected override int _audioPlayerNum => 3;

  //再生に使ってるプレイヤークラス
  private AudioPlayer _audioPlayer => _audioPlayerList[0];

  //オーディオファイルが入ってるディレクトリへのパス
  public static readonly string AUDIO_DIRECTORY_PATH = "BGM";
  private string _currentAudio;

  //=================================================================================
  //初期化
  //=================================================================================

  //起動時に実行される
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void Initialize()
  {
      new GameObject("BGMManager", typeof(BGMManager));
  }
  
  protected override void Init() 
  {
    base.Init();

    LoadAudioClip(AUDIO_DIRECTORY_PATH, AudioCacheType.None, false);
    
    ChangeBaseVolume(Preferences.GetBgmBaseVolume());
    DontDestroyOnLoad(gameObject);
    
  }

  //=================================================================================
  //再生
  //=================================================================================

  /// <summary>
  /// 再生
  /// </summary>
  public void Play(AudioClip audioClip, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, bool allowsDuplicate = false) {
    //重複が許可されてない場合は、既に再生しているものを止める
    if (!allowsDuplicate) {
      Stop();
    }
    RunPlayer(audioClip, volumeRate, delay, pitch, isLoop);
  }
  
  /// <summary>
  /// 再生
  /// </summary>
  public void Play(string audioPath, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, bool allowsDuplicate = false)
  {
    if (_currentAudio == audioPath) return;
    _currentAudio = audioPath;
    if (!allowsDuplicate) Stop();
    
    RunPlayer(audioPath, volumeRate, delay, pitch, isLoop);
  }

}
}