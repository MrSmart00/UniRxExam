using Zenject;

namespace UnityChan.Rx.Zenject
{
    public class UnityChanInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ICharacterMover>()
                .To<UnityChanMover>()
                .AsTransient();
            Container
                .Bind<IAnimatorStatus<UnityChanAnimatorState>>()
                .To<UnityChanAnimatorStatus>()
                .AsTransient();
            Container
                .Bind<IDeltaTime>()
                .To<DeltaTime>()
                .AsTransient();

            Container
                .Bind<IUnityChanViewModel<UnityChanAnimatorState, UnityChanRxViewModelContext>>()
                .To<UnityChanRxViewModel>()
                .AsTransient();
        }
    }
}
