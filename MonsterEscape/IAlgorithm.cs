namespace MonsterEscape
{
    public interface IAlgorithm
    {
        bool Start();
        IState GetState();
        void SetSpeed(int speed);
    }
}
