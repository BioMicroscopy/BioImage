﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bio
{
    public partial class ColorTool : Form
    {
        private ColorS color = new ColorS(65535, 65535, 65535);
        public ColorS Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public void UpdateGUI()
        {
            color = new ColorS((ushort)redBox.Value, (ushort)greenBox.Value, (ushort)blueBox.Value);
            colorPanel.BackColor = ColorS.ToColor(color);
            if (rBar.Value != redBox.Value)
                redBox.Value = rBar.Value;
            if (gBar.Value != greenBox.Value)
                greenBox.Value = gBar.Value;
            if (bBar.Value != blueBox.Value)
                blueBox.Value = bBar.Value;
        }
        public ColorTool()
        {
            InitializeComponent();
            UpdateGUI();
        }
        public ColorTool(ColorS col)
        {
            InitializeComponent();
            color = col;
            rBar.Value = color.R;
            gBar.Value = color.G;
            bBar.Value = color.B;
            UpdateGUI();
        }

        private void redBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        private void greenBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        private void blueBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        private void rEnbaled_CheckedChanged(object sender, EventArgs e)
        {
            Tools.rEnabled = rEnbaled.Checked;
        }

        private void gEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Tools.gEnabled = gEnabled.Checked;
        }

        private void bEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Tools.bEnabled = bEnabled.Checked;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelBut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void rBar_Scroll(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        private void gBar_Scroll(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        private void bBar_Scroll(object sender, EventArgs e)
        {
            UpdateGUI();
        }
    }
}
