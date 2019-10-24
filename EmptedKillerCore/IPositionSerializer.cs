namespace EmptedKillerCore
{
    public interface IPositionSerializer
    {
        IPosition Read(string text);

        string Write(IPosition position);
    }
}
