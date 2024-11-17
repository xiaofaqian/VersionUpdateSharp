using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace update
{
    public partial class FormUpdateMain : UIForm
    {
        private Icon currentIcon = null;
        public FormUpdateMain()
        {
            LoadIcon("update.ico");
            Config.Load();
            InitializeComponent();
        }
        private void LoadIcon(string iconPath)
        {
            try
            {
                if (currentIcon != null)
                {
                    currentIcon.Dispose();
                }
                currentIcon = new Icon(iconPath);
                this.Icon = currentIcon;
            }
            catch
            {
                this.ShowWarningTip("应用图标加载失败");
            }
        }

        private async Task checkUpdate()
        {
            try
            {
                List<string> updateFiles = await CheckUpdate.GetUpdateFiles();
                if (updateFiles.Count > 0)
                {
                    Text = "有更新";
                    if (this.ShowAskDialog("是否更新?"))
                    {
                        uiProcessBar1.Maximum = updateFiles.Count;
                        foreach (string file in updateFiles)
                        {
                            Text = $"更新{file}";
                            uiProcessBar1.Value += 1;
                            byte[] fileBytes = await CheckUpdate.GetUpdateFile(file);
                            try
                            {
                                string[] files = file.Split(new char[] { '\\', });
                                File.WriteAllBytes(file, fileBytes);
                            }
                            catch (Exception ex)
                            {
                                Text = ex.Message;
                            }
                        }
                    }
                }
                string CurrentDirectory = Directory.GetCurrentDirectory();
                Process.Start($"{CurrentDirectory}/maoniu/maoniu.exe", "start");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                this.ShowErrorDialog($"更新失败:{ex.Message}\r\n请重试!");
                Environment.Exit(0);
            }
        }

        private void FormUpdateMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.ShowAskDialog("程序正在更新,确定退出?"))
            {
                e.Cancel = true;
                return;
            }
            Environment.Exit(0);
        }

        private async void FormUpdateMain_Load(object sender, EventArgs e)
        {
            await checkUpdate();
        }
    }
}
