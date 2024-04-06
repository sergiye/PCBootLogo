﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PCBootLogo {
  
  public partial class MainForm : Form {
    
    private readonly LogoModel model = new LogoModel();

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

      Load += (s, e) => {
        model.CreateViewData();
        if (!model.UiIsEnable) {
          MessageBox.Show("Unsupported BIOS!\nThis application only supports computers with newer BIOS versions.", 
            LogoModel.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
          Application.Exit();
          return;
        }

        if (model.VisibleLoadingIco) {
          cbxShowLoadingIcon.Checked = model.DisplayLoadingIco;
          cbxShowLoadingIcon.Enabled = true;
        }

        lblFormat.Text = $"Format: {model.Filter} / Max: {model.DefaultWidth}x{model.DefaultHeight}";
      };

      FormClosed += (s, e) => {
        try {
          ApiMethods.ReleaseUnmanagedLibrary();
          if (!string.IsNullOrEmpty(model.ImagePath))
            File.Delete(model.ImagePath);
        }
        catch {
          //ignore
        }
      };
    }

    private void btnSelectImage_Click(object sender, EventArgs e) {
      model.SelectedImageClick();
      showTip();
    }

    private void btnApply_Click(object sender, EventArgs e) {
      model.SaveLogoClick();
      showTip();
    }

    private void showTip() {
      if (model.ShowWarning) {
        MessageBox.Show(model.ShowWarnInfo, LogoModel.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        model.ShowWarning = false;
      }

      if (model.ShowSuccessTip) {
        MessageBox.Show(model.ShowSuccessText, LogoModel.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        model.ShowSuccessTip = false;
      }

      btnApply.Enabled = model.FunEnable;
      btnRecovery.Enabled = model.CanRecovery;
    }

    private void btnRecovery_Click(object sender, EventArgs e) {
      model.ToRecovery();
      showTip();
    }

    private void cbxShowLoadingIcon_CheckedChanged(object sender, EventArgs e) {
      if (!cbxShowLoadingIcon.Enabled) return;
      var result = model.ChangeLodingIco(cbxShowLoadingIcon.Checked);
      MessageBox.Show(result ? "Done!" : "Failed!", LogoModel.AppTitle, MessageBoxButtons.OK, 
        result ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }
  }
}