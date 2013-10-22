using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterEscape;

namespace MonsterEscapeApp
{
    class AI : IMonsterEscapeAI
    {
        int nState = 0;
        double omega = -1;
        public Angle NextBearing(IState state)
        {
            if (state.PositionRadial == 0)
                nState = 0;

            switch (nState)
            {
                case 0:
                    if (state.PositionTheta.Diff(state.MonsterTheta) <= 3 * Math.PI / 4 - 0.1)
                    {
                        nState = 1;
                        if (state.MonsterTheta < Math.PI)
                        {
                            omega = -1 / state.PositionRadial;
                            return state.CurrentBearing - Math.PI / 2;
                        }
                        else
                        {
                            omega = 1 / state.PositionRadial;
                            return state.CurrentBearing + Math.PI / 2;
                        }
                    }
                    return state.CurrentBearing;
                case 1:
                    if (Math.PI - state.PositionTheta.Diff(state.MonsterTheta) < state.Epsilon)
                    {
                        nState = 2;
                        return state.PositionTheta;
                    }
                    return state.CurrentBearing + omega * state.TimeStep;
                case 2:
                default:
                    return state.CurrentBearing;
            }
        }
    }
}
