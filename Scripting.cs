﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using CSScripting;
using csscript;
using CSScriptLib;

namespace BioImage
{
    public partial class Scripting : Form
    {
        public static Scripting runner;

        public static string log;
        
        public static void LogLine(string s)
        {
            log += s + Environment.NewLine;
        }

        public static Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public class Script
        {
            public string name;
            public string file;
            public string scriptString;
            public dynamic script;
            public object obj;
            public string output = "";
            public bool done = false;
            public Exception ex = null;
            public Thread thread;
            public Script(string file, string scriptStr)
            {
                name = Path.GetFileName(file);
                scriptString = scriptStr;
            }
            public Script(string file)
            {
                name = Path.GetFileName(file);
                scriptString = File.ReadAllText(file);
                this.file = file;
            }
            public Script()
            { }
            public static void Run(Script rn)
            {
                scriptName = rn.name;
                Thread t = new Thread(new ThreadStart(RunScript));
                t.Start();
            }

            private static string scriptName = "";
            private static void RunScript()
            {
                Script rn = Scripts[scriptName];
                rn.ex = null;
                try
                {
                    rn.done = false;
                    rn.script = CSScript.Evaluator.LoadCode(rn.scriptString);
                    rn.obj = rn.script.Load();
                    rn.output = rn.obj.ToString();
                    rn.done = true;
                }
                catch (Exception e)
                {
                    rn.ex = e;
                }
            }
            public void Run()
            {
                if (!Scripts.ContainsKey(name))
                    Scripts.Add(name, this);
                scriptName = this.name;
                thread = new Thread(new ThreadStart(RunScript));
                thread.Start();
            }
            public void Stop()
            {
                thread.Abort();
            }
            public override string ToString()
            {
                if (thread != null)
                {
                    if (thread.ThreadState == ThreadState.Running)
                        return name.ToString() + ", Running";
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex == null && output != "")
                        return name.ToString() + ", Output: " + output;
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex != null)
                        return name.ToString() + ", Exception: " + ex.ToString();
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex == null)
                        return name.ToString() + ", Exception: " + ex.ToString();
                }
                return name.ToString();
            }
        }
        public class State
        {
            public Event type;
            public static State GetUp(PointF pf,MouseButtons mb)
            {
                State st = new State();
                st.type = Event.Up;
                st.p = pf;
                st.buts = mb;
                return st;
            }
            public static State GetDown(PointF pf, MouseButtons mb)
            {
                State st = new State();
                st.type = Event.Down;
                st.p = pf;
                st.buts = mb;
                return st;
            }
            public static State GetMove(PointF pf, MouseButtons mb)
            {
                State st = new State();
                st.type = Event.Move;
                st.p = pf;
                st.buts = mb;
                return st;
            }
            public PointF p;
            public MouseButtons buts;
            //public bool processed = false;
            public override string ToString()
            {
                return type.ToString() + " ,(" + p.X.ToString() + ", " + p.Y.ToString() + "), " + buts.ToString();
            }

        }
        public enum Event
        {
            Down,
            Up,
            Move,
            None
        }
        private static State state;
        public static State GetState()
        {
            return state;
        }
        public static void UpdateState(State s)
        {
            if(s!=null)
                state = s;
        }
        public void RefreshItems()
        {
            string dir = Application.StartupPath + "//" + "Scripts";
            foreach (string file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(".cs"))
                {
                    if (!Scripts.ContainsKey(Path.GetFileName(file)))
                    {
                        //This is a script file.
                        Script sc = new Script(file, File.ReadAllText(file));
                        ListViewItem lv = new ListViewItem();
                        lv.Tag = sc;
                        lv.Text = sc.ToString();
                        scriptView.Items.Add(lv);
                        Scripts.Add(lv.Text, sc);
                    }
                }
            }
        }
        public void RefreshStatus()
        {
            foreach (ListViewItem item in scriptView.SelectedItems)
            {
                Script s = (Script)item.Tag;
                //We update item text to show Script status.
                item.Text = s.ToString();
                outputBox.Text = s.output;
                if(s.ex!=null)
                errorBox.Text = s.ex.Message.ToString();
            }
            logBox.Text = log;
            //We scroll to end of text so we see latest log output.
            if (logBox.SelectionStart != logBox.Text.Length)
            {
                logBox.SelectionStart = logBox.Text.Length;
                logBox.ScrollToCaret();
            }
        }
        public Scripting()
        {
            InitializeComponent();
            scriptView.MultiSelect = false;
            runner = this;
            RefreshItems();
            timer.Start();
        }

        public void RunScriptFile(string file)
        {
            Script sc = new Script(file);
            Scripts.Add(sc.name,sc);
            RefreshItems();
            RunByName(sc.name);
        }
        public void Run()
        {
            outputBox.Text = "";
            errorBox.Text = "";
            if (scriptView.SelectedItems.Count == 0)
                    return;
            foreach (ListViewItem item in scriptView.SelectedItems)
            {
                //We run this script
                Script sc = (Script)item.Tag;
                sc.scriptString = textBox.Text;
                sc.output = "";
                sc.Run();
                outputBox.Text = sc.output;
                logBox.Text = log;
            }
        }
        public void Stop()
        {
            if (scriptView.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem item in scriptView.SelectedItems)
            {
                //We run this script
                Script sc = (Script)item.Tag;
                sc.Stop();
            }
        }
        public static void RunByName(string name)
        {
            Scripts[name].Run();
        }

        private void scriptView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Run();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void openScriptFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\Scripts";
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshItems();
            RefreshStatus();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                RefreshStatus();
            }
        }

        private void ScriptRunner_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void scriptView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scriptView.SelectedItems.Count == 0)
                return;
            ListViewItem item = scriptView.SelectedItems[0];
            Script s = (Script)item.Tag;
            textBox.Text = s.scriptString;
            scriptLabel.Text = s.name;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void scriptLoadBut_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = Application.StartupPath + "\\Scripts";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Script script = new Script(openFileDialog.FileName, File.ReadAllText(openFileDialog.FileName));
            script.name = Path.GetFileName(openFileDialog.FileName);
            textBox.Text = script.scriptString;
            scriptLabel.Text = Path.GetFileName(openFileDialog.FileName);
            ListViewItem item = new ListViewItem();
            item.Tag = script;
            item.Text = script.name;
            scriptView.Items.Add(item);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.InitialDirectory = Application.StartupPath + "\\Scripts";
            saveFileDialog.FileName = scriptLabel.Text;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            scriptLabel.Text = Path.GetFileName(saveFileDialog.FileName);
            File.WriteAllText(saveFileDialog.FileName, textBox.Text);
        }

        private void stopBut_Click(object sender, EventArgs e)
        {
            Stop();
        }
    }
}
