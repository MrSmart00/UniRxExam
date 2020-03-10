using System;

public interface IAnimatorStatus<State> where State: Enum
{
    State currentState { get; }
    void update(int stateHash);
}
