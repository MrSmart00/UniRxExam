public interface IViewModel<Context, Input, Output>
{
    void inject(Context context);
    Output transform(Input input);
}