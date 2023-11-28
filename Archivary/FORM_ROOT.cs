﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivary
{
    public partial class FORM_ROOT : Form
    {
        public FORM_ROOT()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        public void loadParentForm(Form loadForm)
        {
            loadForm.TopLevel = false;
            loadForm.FormBorderStyle = FormBorderStyle.None;
            loadForm.Dock = DockStyle.Fill;

            PANEL_HOLDER.Controls.Clear();
            PANEL_HOLDER.Controls.Add(loadForm);

            loadForm.BringToFront();
            loadForm.Show();
        }

        private void FORM_ROOT_Load(object sender, EventArgs e)
        {
            loadParentForm(new FORM_TITLE(this));
        }
    }
}