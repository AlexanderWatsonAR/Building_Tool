
// Should IBuildable be an abstract class inheriting from monobehaviour?
// My main reasoning is that all the buildable inherited classes we have created so far
// store an data object inherited from IData. I have been considering implementing this change because
// it would simplify the data overlay classes.

public interface IBuildable
{
    IBuildable Initialize(IData data);
    void Build();
    void Demolish();

}
