using System;
using UnityEngine;
using UniRx;
using static UniRx.Triggers.ObservableStateMachineTrigger;

public struct UnityChanViewModelInput
{
    public IObservable<Unit> update;
    public IObservable<OnStateInfo> stateInfo;

    public UnityChanViewModelInput(IObservable<Unit> update, IObservable<OnStateInfo> stateInfo)
    {
        this.update = update;
        this.stateInfo = stateInfo;
    }
}

public struct UnityChanViweModelOutput<State> where State : Enum
{
    public IObservable<State> state;
    public IObservable<(Vector3 position, Vector3 rotate)> move;

    public UnityChanViweModelOutput(IObservable<State> state, IObservable<(Vector3 position, Vector3 rotate)> move)
    {
        this.state = state;
        this.move = move;
    }
}

public interface IUnityChanViewModel<State, Context> where State : Enum
{
    void inject(Context context);
    UnityChanViweModelOutput<State> transform(UnityChanViewModelInput input);
}
