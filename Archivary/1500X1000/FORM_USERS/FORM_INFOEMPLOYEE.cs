﻿using Archivary._1200X800.FORM_USERS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Archivary._1200X800.FORM_USERS;

namespace Archivary._1500X1000.FORM_USERS
{
    public partial class FORM_INFOEMPLOYEE : Form
    {
        private FORM_EDITEMPLYEE editInfo = new FORM_EDITEMPLYEE();

        private Color archivaryGreen()
        {
            return Color.FromArgb(37, 211, 102);
        }

        private Color archivaryRed()
        {
            return Color.FromArgb(227, 25, 55);
        }
        private Color archivaryWhite()
        {
            return Color.FromArgb(244, 244, 244);
        }
        private Color archivaryBlack()
        {
            return Color.FromArgb(20, 18, 18);
        }

        public FORM_INFOEMPLOYEE()
        {
            InitializeComponent();
        }

        private void FORM_INFOEMPLOYEE_Load(object sender, EventArgs e)
        {
            String status = "Active";
            statusColor(status);
            //sample data, pwede toh burahin mga pre

            emailLabel.Text = "sampleemail@email.com";
            contactNumLabel.Text = "00000000000";
            addressLabel.Text = "Bldg No, Street, Brgy., City";
            Random random = new Random();
            string[] returnStatus = { "Overdue", "Not Overdue" };
            String randomStatus;
            int n = 10;
            for (int i = 0; i <= n; i++)
            {
                randomStatus = returnStatus[random.Next(returnStatus.Length)];
                bookListDataGridView.Rows.Add("abcdefghijklmnopqrstuvwxyz", "MM/DD/YY", "MM/DD/YY", "MM/DD/YY", randomStatus);
            }
        }
        private void backButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void editInfoButton_Click(object sender, EventArgs e)
        {
            editInfo.ShowDialog();
        }
        
        private void bookListDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i <= e.RowIndex + e.RowCount - 1; i++)
            {
                var row = bookListDataGridView.Rows[i];
                string returnStatus = Convert.ToString(row.Cells[4].Value).ToLower();
                if (returnStatus == null) continue;

                if (returnStatus == "overdue") row.Cells[4].Style.ForeColor = archivaryRed();
                else if (returnStatus == "not overdue") row.Cells[4].Style.ForeColor = archivaryGreen();
            }
        }

        private void statusColor(String status)
        {
            if (status == "Active")
            {
                editInfoButton.BackColor = archivaryGreen();
                editInfoButton.ForeColor = archivaryBlack();
                changeStatusButton.BackColor = archivaryRed();
                changeStatusButton.Text = "DEACTIVATE";
                PANEL_role.BorderColor = archivaryGreen();
                PANEL_role.BackgroundColor = archivaryGreen();
                BackColor = archivaryGreen();
                userIDLabel.ForeColor = archivaryGreen();
            }
            else if (status == "Deactivated")
            {
                editInfoButton.BackColor = archivaryRed();
                editInfoButton.ForeColor = archivaryWhite();
                changeStatusButton.BackColor = archivaryGreen();
                changeStatusButton.Text = "REACTIVATE";
                PANEL_role.BorderColor = archivaryRed();
                PANEL_role.BackgroundColor = archivaryRed();
                BackColor = archivaryRed();
                userIDLabel.ForeColor = archivaryRed();
            }
        }

    }
}
