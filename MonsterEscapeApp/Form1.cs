using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MonsterEscape;
using System.Threading;

namespace MonsterEscapeApp
{
    public partial class Form1 : Form
    {
        private class AI : IMonsterEscapeAI
        {
            int step = 0;
            double criticalRadius = -1;
            int nState = 0;
            public double NextBearing(IState state)
            {
                step = (++step) % 8;
                if (step == 0)
                    Thread.Sleep(1);

                switch (nState)
                {
                    case 0:
                        criticalRadius = 1 / state.MonsterSpeed;
                        nState++;
                        return state.CurrentBearing;
                    case 1:
                        if (state.PositionRadial - criticalRadius > 1e-5)
                        {
                            nState++;
                            return state.CurrentBearing + Math.PI;
                        }
                        return state.CurrentBearing;
                    case 2:
                        if (state.PositionRadial - criticalRadius > 1e-5)
                        {
                            return state.CurrentBearing;
                        }
                        nState++;
                        return state.CurrentBearing + Math.PI / 2;
                    case 3:
                        if (state.PositionTheta.Diff(state.MonsterTheta + Math.PI) < 1e-5)
                        {
                            nState++;
                            return state.CurrentBearing + Math.PI / 2;
                        }
                        return state.CurrentBearing - 1e-5 / criticalRadius;
                    case 4:
                    default:
                        return state.CurrentBearing;
                }
            }
        }

        private AI _ai;
        private IMonsterEscape _escape;
        private Thread _thread;

        public Form1()
        {
            InitializeComponent();

            Paint += new PaintEventHandler(Form1_Paint);
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            _ai = new AI();
            _escape = MonsterEscape.MonsterEscape.Create(1e-5, 4, 1e-5, _ai);

            DoubleBuffered = true;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _thread.Abort();
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            MonsterEscape.MonsterEscape.DrawSituation(e.Graphics, DisplayRectangle, _escape.GetState());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _thread = new Thread(new ThreadStart(worker));
            _thread.Start();

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000 / 30;
            timer.Tick += (o, ee) => { this.Invalidate(true); };
            timer.Start();
        }

        private void worker()
        {
            if (!_escape.Start())
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Oops!");
                }));
        }
    }
}
