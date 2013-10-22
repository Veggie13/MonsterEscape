using System;
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
            get { return doHalfHalf(_value); }
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
            return Math.Abs(doHalfHalf(this - a));
        }

        private double doHalfHalf(double v)
        {
            return mod(v + Math.PI, 2 * Math.PI) - Math.PI;
        }

        public override string ToString()
        {
            return "(rad) " + _value.ToString();
        }
    }
}