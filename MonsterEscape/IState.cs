namespace MonsterEscape
{
    public interface IState
    {
        double TimeStep { get; }
        double Epsilon { get; }
        Angle MonsterTheta { get; }
        double MonsterSpeed { get; }
        Angle CurrentBearing { get; }
        Angle PositionTheta { get; }
        double PositionRadial { get; }
    }
}
