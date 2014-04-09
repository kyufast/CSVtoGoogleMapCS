using Direction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectionFoem
{
    public partial class Form1 : Form
    {
        Button okbutton;
        public Form1()
        {
            InitializeComponent();

            init();

            this.okbutton = new Button();
            this.okbutton.Location = new Point(10, 190);
            this.okbutton.Size = new Size(170, 30);
            this.okbutton.Text = "実行";
            this.okbutton.Click += new EventHandler(this.okButtonClick);
            this.Controls.Add(this.okbutton);
        }

        void init()
        {
        }

        public void okButtonClick(object sender, EventArgs e)
        {
            StationInfoOperation stationinfooperation = new StationInfoOperation();
            String result = stationinfooperation.getUPorDown("東京", "横浜", "JR東海道本線");
        }
    }
}
