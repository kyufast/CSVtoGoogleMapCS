using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace GPStoGoogleMap
{
    public partial class Form1 : Form
    {
        Label inputlabel, outputlabel;
        Button inputbutton, outputbutton, okbutton;
        FileOperation fileoperation;
        OpenFileDialog inputFileDialog;
        FolderBrowserDialog outputDirctory;

        public Form1()
        {
            InitializeComponent();

            init();

            this.inputlabel = new Label
            {
                Location = new Point(10, 20),
                AutoSize = true,
                Text = "入力ファイル名",
            };
            this.Controls.Add(inputlabel);

            this.inputbutton = new Button();
            this.inputbutton.Location = new Point(10, 50);
            this.inputbutton.Size = new Size(170, 30);
            this.inputbutton.Text = "入力ファイル";
            this.inputbutton.Click += new EventHandler(this.inputButtonClick);
            this.Controls.Add(this.inputbutton);


            this.outputlabel = new Label
            {
                Location = new Point(10, 100),
                AutoSize = true,
                Text = "出力フォルダは選択しない時入力ファイルのフォルダです",
            };
            this.Controls.Add(outputlabel);

            this.outputbutton = new Button();
            this.outputbutton.Location = new Point(10, 140);
            this.outputbutton.Size = new Size(170, 30);
            this.outputbutton.Text = "出力ファイル";
            this.outputbutton.Click += new EventHandler(this.ouputButtonClick);
            this.Controls.Add(this.outputbutton);

            this.okbutton = new Button();
            this.okbutton.Location = new Point(10, 190);
            this.okbutton.Size = new Size(170, 30);
            this.okbutton.Text = "実行";
            this.okbutton.Click += new EventHandler(this.okButtonClick);
            this.Controls.Add(this.okbutton);

        }

        void init()
        {
            fileoperation = new FileOperation();
        }
        void inputButtonClick(object sender, EventArgs e)
        {
            inputFileDialog = new OpenFileDialog();
            if (inputFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileoperation.InputFilePath=inputFileDialog.FileName;
                Debug.WriteLine("input Path:" + inputFileDialog.FileName);
                inputlabel.Text = inputFileDialog.FileName;
                //System.IO.StreamReader sr = new
                //  System.IO.StreamReader(inputFileDialog.FileName);
                //MessageBox.Show(sr.ReadToEnd());
                //sr.Close();
            }
        }

        void ouputButtonClick(object sender, EventArgs e)
        {
            outputDirctory = new FolderBrowserDialog();
            if (outputDirctory.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                Debug.WriteLine("output Path:" + outputDirctory.SelectedPath);
                fileoperation.OutputDirctoryPath = outputDirctory.SelectedPath;
                outputlabel.Text = outputDirctory.SelectedPath;
            }
        }

        void okButtonClick(object sender, EventArgs e)
        {
            fileoperation.calcLogToGoogleMap();
        }
    }
}
