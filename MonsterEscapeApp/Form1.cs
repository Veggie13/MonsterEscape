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
        private AI _ai;
        private IAlgorithm _escape;
        private Thread _thread;

        private class BufferedPanel : Panel
        {
            public BufferedPanel() : base()
            {
                DoubleBuffered = true;
            }
        }

        public Form1()
        {
            InitializeComponent();

            panel1.Paint += new PaintEventHandler(Form1_Paint);
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            _ai = new AI();
            _escape = MonsterEscape.Algorithm.Create(1e-5, 4, 1e-3, _ai);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _thread.Abort();
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            MonsterEscape.Algorithm.DrawSituation(e.Graphics, panel1.DisplayRectangle, _escape.GetState());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
                    button1.Enabled = true;
                }));
            else
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Yay!");
                    button1.Enabled = true;
                }));
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _escape.SetSpeed(1 << vScrollBar1.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            _thread = new Thread(new ThreadStart(worker));
            _thread.Start();
        }
    }
}
