using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWClientLib;
using System.Xml.Linq;

namespace ClientWithDll
{
    public partial class Form1 : Form
    {
        int maxlist = 100;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var data= Client.GetAnalyzersStatus();
            SetStatus(data);
            //var xml = XDocument.Parse(data);
            //foreach (var ele in xml.Root.Descendants("analyzer"))
            //{
            //    //listBox1.Items.Add(ele.Element("name").Value + ":" + ele.Element("sttsq").Value + "," + ele.Element("sttsd").Value + "," + ele.Element("errq").Value + "," + ele.Element("errd").Value);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.OnGotInfo += Client_OnGotInfo;
            Client.OnError += Client_OnError;
            Client.MaxErrorCount = 1;
            string succstr;
            var start= Client.Start(out succstr);
            button1.Enabled = !start;
            button2.Enabled = start;
            if (!start) return;
            SetStatus(succstr);
            //var xml = XDocument.Parse(succstr);
            //foreach (var ele in xml.Root.Descendants("analyzer")) {
            //    //listBox1.Items.Add(ele.Element("name").Value + ":" + ele.Element("sttsq").Value + "," + ele.Element("sttsd").Value + "," + ele.Element("errq").Value + "," + ele.Element("errd").Value);
            //}
        }

        void Client_OnError(int count)
        {
            stop();
        }

        void Client_OnGotInfo(string info)
        {
            AddStatus(info);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop();
        }
        delegate void addItemHandler(string item);
        void addItem(string item) {
            if (this.InvokeRequired)
            {
                this.Invoke(new addItemHandler(addItem), item);
            }
            else {
                //listBox1.Items.Add(item);
                //listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }
        delegate void stopHandler();
        void stop()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new stopHandler(stop));
            }
            else
            {
                Client.Stop();
                button1.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(GroupBox))
                {
                    var gb = (GroupBox)control;
                    foreach (Control control2 in gb.Controls)
                    {
                        if (control2.GetType() == typeof(ListBox) && ((ListBox)control2).Tag.ToString() == "q")
                        {
                            var lbq = (ListBox)control2;
                            lbq.Items.Clear();
                        }
                        if (control2.GetType() == typeof(ListBox) && ((ListBox)control2).Tag.ToString() == "d")
                        {
                            var lbd = (ListBox)control2;
                            lbd.Items.Clear();
                        }
                    }
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.Stop();
        }
        delegate void StatusHandler(string s);
        void SetStatus(string s) {
            if (this.InvokeRequired)
            {
                this.Invoke(new StatusHandler(SetStatus),s);
            }
            else
            {
                if (s == "") {
                    MessageBox.Show(this,"连接服务器失败","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                var xml = XDocument.Parse(s);
                foreach (var ele in xml.Root.Descendants("analyzer"))
                {
                    TextBox tbq = null;
                    TextBox tbd = null;
                    ListBox lbq = null;
                    ListBox lbd = null;
                    var has = getControls(ele.Element("name").Value,out tbq,out tbd,out lbq,out lbd);
                    if (!has) continue;
                    tbq.Text = ele.Element("errq").Value;
                    tbd.Text = ele.Element("errd").Value;
                    if (lbq.Items.Count >= maxlist)
                    {
                        lbq.Items.RemoveAt(0);
                    }
                    lbq.Items.Add(ele.Element("sttsq").Value);
                    lbq.SelectedIndex = lbq.Items.Count - 1;
                    if (lbd.Items.Count >= maxlist)
                    {
                        lbd.Items.RemoveAt(0);
                    }
                    lbd.Items.Add(ele.Element("sttsd").Value);
                    lbd.SelectedIndex = lbd.Items.Count - 1;
                }
            }
        }
        void AddStatus(string s) {
            if (this.InvokeRequired)
            {
                this.Invoke(new StatusHandler(AddStatus), s);
            }
            else
            {
                var slist = s.Split(',');
                var name=slist[0];
                var ltype = slist[1];
                var status = slist[3];
                var err = "";
                if (slist.Length > 4) {
                    err = string.Join(",", slist, 4, slist.Length - 4);
                }
                TextBox tbq = null;
                TextBox tbd = null;
                ListBox lbq = null;
                ListBox lbd = null;
                var has = getControls(name, out tbq, out tbd, out lbq, out lbd);
                if (!has) return;
                if (ltype == "quarter")
                {
                    if (err != "") {
                        tbq.Text = err;
                    }
                    if (lbq.Items.Count >= maxlist)
                    {
                        lbq.Items.RemoveAt(0);
                    }
                    lbq.Items.Add(DateTime.Now.ToString("HH:mm:ss")+":"+status);
                    lbq.SelectedIndex = lbq.Items.Count - 1;
                }
                else if (ltype == "day")
                {
                    if (err != "")
                    {
                        tbd.Text = err;
                    }
                    if (lbd.Items.Count >= maxlist)
                    {
                        lbd.Items.RemoveAt(0);
                    }
                    lbd.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ":" + status);
                    lbd.SelectedIndex = lbd.Items.Count - 1;
                }
            }
        }
        bool getControls(string text,out TextBox tbq, out TextBox tbd, out ListBox lbq, out ListBox lbd) {
            tbq = null;
            tbd = null;
            lbq = null;
            lbd = null;
            var res=false;
            GroupBox gb = null;
            foreach (Control control in this.Controls) {
                if (control.GetType() == typeof(GroupBox)&&((GroupBox)control).Text==text) {
                    gb = (GroupBox)control;
                    break;
                }
            }
            if (gb == null) return false;
            foreach (Control control in gb.Controls) {
                if (control.GetType() == typeof(TextBox) && ((TextBox)control).Tag.ToString() == "q")
                {
                    tbq = (TextBox)control;
                }
                if (control.GetType() == typeof(TextBox) && ((TextBox)control).Tag.ToString() == "d")
                {
                    tbd = (TextBox)control;
                }
                if (control.GetType() == typeof(ListBox) && ((ListBox)control).Tag.ToString() == "q")
                {
                    lbq = (ListBox)control;
                }
                if (control.GetType() == typeof(ListBox) && ((ListBox)control).Tag.ToString() == "d")
                {
                    lbd = (ListBox)control;
                }
            }
            res = true;
            if (tbq == null || tbd == null || lbq == null || lbd == null) {
                res = false;
            }
            return res;
        }
    }
}
