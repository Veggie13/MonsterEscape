using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace MonsterEscape
{
    public struct Angle
    {
        private static double mod(double d1, double d2)
        {
            double v = d1 % d2;
            return v < 0 ? v + d2 : v;
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = mod(value, 2 * Math.PI); }
        }

        public double HalfHalf
        {
            get { return mod(_value + Math.PI, 2 * Math.PI) - Math.PI; }
        }

        public static implicit operator double(Angle a)
        {
            return a.Value;
        }

        public static implicit operator Angle(double v)
        {
            return new Angle() { Value = v };
        }

        public static Angle operator +(Angle b, Angle a)
        {
            return new Angle() { Value = b.Value + a.Value };
        }

        public static Angle operator -(Angle b, Angle a)
        {
            return new Angle() { Value = b.Value - a.Value };
        }

        public double Diff(Angle a)
        {
            return Math.Abs(HalfHalf - a.HalfHalf);
        }
    }

    public interface IState
    {
        Angle MonsterTheta { get; }
        double MonsterSpeed { get; }
        Angle CurrentBearing { get; }
        Angle PositionTheta { get; }
        double PositionRadial { get; }
    }

    class State : IState
    {
        public Angle MonsterTheta { get; set; }
        public double MonsterSpeed { get; set; }
        public Angle CurrentBearing { get; set; }
        public Angle PositionTheta { get; set; }
        public double PositionRadial { get; set; }
    }

    public interface IMonsterEscapeAI
    {
        double NextBearing(IState state);
    }

    public interface IMonsterEscape
    {
        bool Start();
        IState GetState();
    }

    public static class MonsterEscape
    {
        public static IMonsterEscape Create(double timeStep, double monsterSpeed, double epsilon, IMonsterEscapeAI ai)
        {
            return new MonsterEscapeImpl(timeStep, monsterSpeed, epsilon, ai);
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

    class MonsterEscapeImpl : IMonsterEscape, IState
    {
        private IMonsterEscapeAI _ai;
        private double _dtO2;
        private double _epsilon;

        public MonsterEscapeImpl(double timeStep, double monsterSpeed, double epsilon, IMonsterEscapeAI ai)
        {
            _ai = ai;
            _dtO2 = timeStep / 2;
            _epsilon = epsilon;
            MonsterSpeed = monsterSpeed;
            MonsterTheta = Math.PI;
            CurrentBearing = 0;
            PositionTheta = 0;
            PositionRadial = 0;
        }

        public bool Start()
        {
            while (true)
            {
                double nextBearing = _ai.NextBearing(this);

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
                    if (PositionTheta.Diff(MonsterTheta) < _epsilon)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public IState GetState()
        {
            return new State()
            {
                MonsterTheta = MonsterTheta,
                MonsterSpeed = MonsterSpeed,
                CurrentBearing = CurrentBearing,
                PositionTheta = PositionTheta,
                PositionRadial = PositionRadial
            };
        }

        public Angle MonsterTheta { get; private set; }

        public double MonsterSpeed { get; private set; }

        public Angle CurrentBearing { get; private set; }

        public Angle PositionTheta { get; private set; }

        public double PositionRadial { get; private set; }
    }
}
