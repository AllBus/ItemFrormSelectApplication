using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ItemFrormSelectApplication
{
    
    public partial class Form1 : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private int duration=500*10000;

        List<AnimateItem> animates;
        Color hoverBack = Color.MediumBlue;
        Color leaveBack = Color.DarkGreen;

        public Form1()
        {
            InitializeComponent();
            animates = new List<AnimateItem>();
            label1.Tag = new AnimateItem(label1);
            label2.Tag = new AnimateItem(label2);
            label3.Tag = new AnimateItem(label3);
            label4.Tag = new AnimateItem(label4);

            readProperties();

            label1.ForeColor = leaveBack;
            label2.ForeColor = leaveBack;
            label3.ForeColor = leaveBack;
            label4.ForeColor = leaveBack;
        }

        private void readProperties()
        {
            XmlTextReader reader = new XmlTextReader("./appprops/data.xml");
            String tek = "";
            while (reader.Read())
            {
               
                // Обработка данных.
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // Узел является элементом.
                        Console.Write("<" + reader.Name);
                        Console.WriteLine(">");
                        if (reader.Name == "item")
                            readPropertiesItem(reader);
                        else if (reader.Name == "color")
                            readPropertiesColor(reader);
                        else if (reader.Name == "background")
                            readPropertiesBackground(reader);
                        tek = reader.Name;
                        break;
                    case XmlNodeType.Text: // Вывести текст в каждом элементе.
                        Console.WriteLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement: // Вывести конец элемента.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            Console.ReadLine();
        }

        private void readPropertiesBackground(XmlTextReader reader)
        {
                       
            String value = reader.GetAttribute("color");
            if (value!=null && value.Length>0)
                this.BackColor= ColorTranslator.FromHtml(value);

            String image = "./appprops/"+ reader.GetAttribute("image");
            this.BackgroundImage = Image.FromFile(image);
        }

        private void readPropertiesColor(XmlTextReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // Узел является элементом.
                        if (reader.Name == "hover")
                            readPropertyColorHover(reader);
                        else if (reader.Name == "leave")
                            readPropertyColorLeave(reader);

                        break;

                    case XmlNodeType.EndElement: // Вывести конец элемента.
                        return;
                }
            }
        }

        private void readPropertyColorLeave(XmlTextReader reader)
        {
            String value = reader.GetAttribute("value");
            leaveBack= ColorTranslator.FromHtml(value);
        }

        private void readPropertyColorHover(XmlTextReader reader)
        {
            String value = reader.GetAttribute("value");
            hoverBack = ColorTranslator.FromHtml(value);
        }

        private void readPropertiesItem(XmlTextReader reader)
        {
            String title = reader.GetAttribute("title");
            String path = reader.GetAttribute("path");
            switch (reader.GetAttribute("name"))
            {
                case "One":
                    label1.Text = title;
                    break;
                case "Two":
                    label2.Text = title;
                    break;
                case "Three":
                    label3.Text = title;
                    break;
                case "Four":
                    label4.Text = title;
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_MouseHover(object sender, EventArgs e)
        {

            AnimateItem a = (AnimateItem)((Label)sender).Tag;
            if (a != null)
            {
                a.state = 1;
                a.startTime = DateTime.Now.Ticks - (long)((a.value) * duration);
                animates.Add(a);

                animateTimer.Enabled = true;
            }
            
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {

            AnimateItem a = (AnimateItem)((Label)sender).Tag;
            if (a != null)
            {
                a.state = -1;
                a.startTime = DateTime.Now.Ticks - (long)((1-a.value) * duration);
                animates.Add(a);

                animateTimer.Enabled = true;
            }
        }

        private void animateTimer_Tick(object sender, EventArgs e)
        {
            long t = DateTime.Now.Ticks;

            foreach(var a in animates)
            {
                long p = t - a.startTime;
                if (p >= duration)
                {
                    if (a.state == 1)
                    {
                        a.element.ForeColor = hoverBack;
                        a.value = 1;
                    }
                    else
                    {
                        a.element.ForeColor = leaveBack;
                        a.value = 0;
                    }
                    a.state = 0;

                }
                else
                {
                    double h = p*1.0 / duration;
                    if (a.state == -1)
                        h = 1.0 - h;
                    a.value = h;
                    a.element.ForeColor = Utils.Blend(hoverBack, leaveBack, h);
                }
            }

            animates = animates.FindAll(x => x.state != 0);

            if (animates.Count == 0)
            {
                animateTimer.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}

