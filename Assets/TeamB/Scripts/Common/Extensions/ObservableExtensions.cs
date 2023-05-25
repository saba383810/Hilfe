using System;
using Sabanogames.AudioManager;
using UniRx;

public static class ObservableExtension
{
    public static IObservable<T> DoSePlayShot<T>(this IObservable<T> source, string audioName)
    {
        //音を鳴らす
        return audioName != null ? source.Do(_ =>
        {
            SEManager.Instance.Play(audioName);
        }) : source.Do(_ => { });
    }
}