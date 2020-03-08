using System;

public interface IAnimatorStatus<State> where State: Enum
{
    State CurrentState();
    void Update(int stateHash);
}
