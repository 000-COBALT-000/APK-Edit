﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frmMain.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the frmMain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APK_Edit
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// </summary>
    public partial class frmMain : Form
    {
        private ApkFile apkFile;
        private bool closeAfterCompiling;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.apkFile = new ApkFile();
                this.apkFile.EnableBackup = true;
                this.apkFile.OverWriteBackup = true;
                this.apkFile.EnableSigning = true;
                this.apkFile.CreateSeperateSignedApk = false;
                
                this.apkFile.Decompiling += this.Decompiling;
                this.apkFile.Decompiled += this.Decompiled;
                this.apkFile.FrameworkMissing += this.FrameworkMissing;
                this.apkFile.Compiling += this.Compiling;
                this.apkFile.Compiled += this.Compiled;
                this.apkFile.Decompile();

                textBoxName.Text = this.apkFile.ApplicationName;
                labelApkToolVersion.Text = Wrappers.Apktool.Version;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void Decompiling()
        {
            this.DisableUI();
            labelStatus.Text = "Loading.";
            pictureBoxStatus.Image = Properties.Resources.Loading_64;
        }

        private void Decompiled()
        {
            labelStatus.Text = string.Empty;
            pictureBoxStatus.Image = null;
            pictureBoxHDPI.Image = this.apkFile.GetIcon();
            pictureBoxHDPI.Cursor = Cursors.Hand;
            this.EnableUI();
        }

        private void FrameworkMissing()
        {
            labelStatus.Text = "Framework file missing.";
            pictureBoxStatus.Image = Properties.Resources.FileWarning_128;
            if (openFileDialogFramework.ShowDialog() == DialogResult.OK)
            {
                this.apkFile.FrameworkInstalling += this.FrameworkInstalling;
                this.apkFile.FrameworkInstalled += this.FrameworkInstalled;
                this.apkFile.InstallFramework(this.openFileDialogFramework.FileName);
            }
            else
            {
                this.Close();
            }
        }

        private void FrameworkInstalling()
        {
            labelStatus.Text = "Installing framework.";
            pictureBoxStatus.Image = Properties.Resources.Loading_64;
        }

        private void FrameworkInstalled()
        {
            this.apkFile.Decompile();
        }

        private void Compiling()
        {
            this.DisableUI();
            labelStatus.Text = "Saving changes.";
            pictureBoxStatus.Image = Properties.Resources.Loading_64;
        }

        private void Compiled()
        {
            if (this.closeAfterCompiling)
            {
                this.Close();
            }
            else
            {
                labelStatus.Text = string.Empty;
                pictureBoxStatus.Image = null;
                pictureBoxHDPI.Image = this.apkFile.GetIcon();
                pictureBoxHDPI.Cursor = Cursors.Hand;
                this.EnableUI();
            }
        }

        private void EnableUI()
        {
            btnOK.Enabled = true;
            btnCancel.Enabled = true;
            btnApply.Enabled = true;
            textBoxName.Enabled = true;
            pictureBoxHDPI.Enabled = true;
            labelStatus.Enabled = true;
            linkLabelBrowse.Enabled = true;
            linkLabelBrowse.Visible = true;
        }

        private void DisableUI()
        {
            btnOK.Enabled = false;
            btnCancel.Enabled = false;
            btnApply.Enabled = false;
            textBoxName.Enabled = false;
            pictureBoxHDPI.Enabled = false;
            labelStatus.Enabled = false;
            linkLabelBrowse.Enabled = false;
            linkLabelBrowse.Visible = false;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.apkFile.ApplicationName = this.textBoxName.Text;
            this.apkFile.Compile();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.closeAfterCompiling = true;
            this.apkFile.ApplicationName = this.textBoxName.Text;
            this.apkFile.Compile();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxHDPI_Click(object sender, EventArgs e)
        {
            if (openFileDialogIcon.ShowDialog() == DialogResult.OK)
            {
                this.pictureBoxHDPI.Image = this.apkFile.SetIcon(this.openFileDialogIcon.FileName);
            }
        }

        private void linkLabelBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", ApkFile.TempPath);
        }
    }
}
