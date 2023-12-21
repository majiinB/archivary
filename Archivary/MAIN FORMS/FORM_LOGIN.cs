﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivary.PARENT_FORMS
{
    public partial class FORM_LOGIN : Form
    {
        private FORM_ROOT FormsRoot;

        private readonly Size minimumSize = new Size(960, 650);

        public FORM_LOGIN(FORM_ROOT formsRoot)
        {
            InitializeComponent();
            FormsRoot = formsRoot;

            this.Size = new Size(960, 650);
            this.MinimumSize = minimumSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormsRoot.loadParentForm(new FORM_SIDEBAR(FormsRoot));
            FormsRoot.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Close();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            FormsRoot.loadParentForm(new FORM_TITLE(FormsRoot));
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            FormsRoot.loadParentForm(new FORM_SIDEBAR(FormsRoot));
            FormsRoot.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void PANEL_TOPBAR_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.FormsRoot.Handle, 0x112, 0xf012, 0);
        }

        private void logoTitlePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.FormsRoot.Handle, 0x112, 0xf012, 0);
        }

        //
        // USERNAME HOVERS
        //
        private string usernameInput;
        private void usernameTextbox_Enter(object sender, EventArgs e)
        {
            usernameInput = usernameTextbox.Text;
            if (usernameInput == "Enter Username") usernameTextbox.Text = "";
            else usernameTextbox.Text = usernameInput;
        }

        private void usernameTextbox_Leave(object sender, EventArgs e)
        {
            usernameInput = usernameTextbox.Text;
            if (string.IsNullOrWhiteSpace(usernameInput)) usernameTextbox.Text = "Enter Username";
        }

        //
        // PASSWORD HOVERS
        //
        private string passwordInput;
        private void passwordTextbox_Enter(object sender, EventArgs e)
        {
            passwordInput = passwordTextbox.Text;
            if (passwordInput == "Enter Password") passwordTextbox.Text = "";
            else passwordTextbox.Text = passwordInput;

        }

        private void passwordTextbox_Leave(object sender, EventArgs e)
        {
            passwordInput = passwordTextbox.Text;
            if (string.IsNullOrWhiteSpace(passwordInput)) passwordTextbox.Text = "Enter Password";

        }

        //
        // TOGGLE EYE BUTTON
        //
        private bool isToggled = false;
        private void eyeButton_Click(object sender, EventArgs e)
        {
            isToggled = !isToggled;
            togglePassword();
        }

        private void togglePassword()
        {
            if (isToggled)
            {
                this.eyeButton.ButtonImage = global::Archivary.Properties.Resources.ICON_EYE;
                passwordTextbox.UseSystemPasswordChar = false;
            }
            else
            {
                this.eyeButton.ButtonImage = global::Archivary.Properties.Resources.ICON_EYE_SLASH;
                passwordTextbox.UseSystemPasswordChar = true;
            }
        }

        private void passwordPicturebox_Click(object sender, EventArgs e)
        {

        }

        private void LAYOUT_EYE_Paint(object sender, PaintEventArgs e)
        {

        }

        private void usernameTextbox_Click(object sender, EventArgs e)
        {

        }

        private void passwordTextbox_Click(object sender, EventArgs e)
        {

        }
    }
}
