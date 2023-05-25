using System;
using UniRx;

namespace sabanogames.Common.UI
{
    public interface IButtonInput
    {
        IObservable<Unit> OnClick { get; }
        IObservable<Unit> OnClickDefendChattering { get; }
        IObservable<Unit> OnClickOnce { get; }
    }

    public class ButtonInput
    {
        public static readonly EmptyInput Empty = new EmptyInput();

        public class EmptyInput : IButtonInput
        {
            public IObservable<Unit> OnClick => Observable.Empty<Unit>();
            public IObservable<Unit> OnClickDefendChattering => Observable.Empty<Unit>();
            public IObservable<Unit> OnClickOnce => Observable.Empty<Unit>();
        }
    }
}