using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace MonsterEscape
{
    public static class Algorithm
    {
        public static IAlgorithm Create(double timeStep, double monsterSpeed, double epsilon, IMonsterEscapeAI ai)
        {
            return new AlgorithmImpl(timeStep, monsterSpeed, epsilon, ai);
        }

        public static void DrawSituation(Graphics g, Rectangle rect, IState state)
        {
            g.FillRectangle(Brushes.Black, rect);
            g.FillEllipse(Brushes.Blue, rect);
            int x0 = rect.Width / 2, y0 = rect.Height / 2;
            int xO2 = x0 - 2, yO2 = y0 - 2;
            g.FillEllipse(Brushes.Red,
                x0 + xO2 * (float)(state.PositionRadial * Math.Cos(state.PositionTheta)),
                y0 + yO2 * (float)(state.PositionRadial * Math.Sin(state.PositionTheta)),
                2, 2);
            g.FillEllipse(Brushes.Magenta,
                x0 + xO2 * (float)(Math.Cos(state.MonsterTheta)),
                y0 + yO2 * (float)(Math.Sin(state.MonsterTheta)),
                3, 3);
        }
    }

    class State : IState
    {
        public double TimeStep { get; set; }
        public double Epsilon { get; set; }
        public Angle MonsterTheta { get; set; }
        public double MonsterSpeed { get; set; }
        public Angle CurrentBearing { get; set; }
        public Angle PositionTheta { get; set; }
        public double PositionRadial { get; set; }
    }

    class AlgorithmImpl : IAlgorithm, IState
    {
        private IMonsterEscapeAI _ai;
        private double _dtO2;
        private int _speed = 1;
        private object _locker = new object();

        public AlgorithmImpl(double timeStep, double monsterSpeed, double epsilon, IMonsterEscapeAI ai)
        {
            _ai = ai;
            _dtO2 = timeStep / 2;
            Epsilon = epsilon;
            MonsterSpeed = monsterSpeed;
            MonsterTheta = Math.PI;
            CurrentBearing = 0;
            PositionTheta = 0;
            PositionRadial = 0;
        }

        public bool Start()
        {
            PositionRadial = 0;
            PositionTheta = 0;
            MonsterTheta = Math.PI;
            CurrentBearing = 0;

            int step = 0;
            while (true)
            {
                lock (_locker)
                {
                    Angle nextBearing = _ai.NextBearing(this);

                    double x = PositionRadial * Math.Cos(PositionTheta) + _dtO2 * (Math.Cos(nextBearing) + Math.Cos(CurrentBearing));
                    double y = PositionRadial * Math.Sin(PositionTheta) + _dtO2 * (Math.Sin(nextBearing) + Math.Sin(CurrentBearing));

                    CurrentBearing = nextBearing;
                    PositionTheta = Math.Atan2(y, x);
                    PositionRadial = Math.Sqrt(x * x + y * y);

                    //Angle destTheta = CurrentBearing - Math.Asin(PositionRadial * Math.Sin(CurrentBearing - PositionTheta));
                    var diff = MonsterTheta - PositionTheta;
                    if (diff > Math.PI)
                    {
                        MonsterTheta += 2 * MonsterSpeed * _dtO2;
                    }
                    else
                    {
                        MonsterTheta -= 2 * MonsterSpeed * _dtO2;
                    }

                    if (PositionRadial >= 1)
                    {
                        if (PositionTheta.Diff(MonsterTheta) < Epsilon)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                step = (++step) % _speed;
                if (step == 0)
                    Thread.Sleep(1);
            }
        }

        public IState GetState()
        {
            lock (_locker)
                return new State()
                {
                    TimeStep = TimeStep,
                    Epsilon = Epsilon,
                    MonsterTheta = MonsterTheta,
                    MonsterSpeed = MonsterSpeed,
                    CurrentBearing = CurrentBearing,
                    PositionTheta = PositionTheta,
                    PositionRadial = PositionRadial
                };
        }

        public void SetSpeed(int speed)
        {
            if (speed < 1)
                speed = 1;

            lock (_locker)
                _speed = speed;
        }

        public double TimeStep { get { return 2 * _dtO2; } }

        public double Epsilon { get; private set; }
        
        public Angle MonsterTheta { get; private set; }

        public double MonsterSpeed { get; private set; }

        public Angle CurrentBearing { get; private set; }

        public Angle PositionTheta { get; private set; }

        public double PositionRadial { get; private set; }
    }
}
