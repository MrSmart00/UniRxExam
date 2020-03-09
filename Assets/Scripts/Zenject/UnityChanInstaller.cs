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
        }
    }
}
