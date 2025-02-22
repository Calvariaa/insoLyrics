﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using insoLyrics.Interop;
using insoLyrics.Forms;
using insoLyrics.Lyrics;

namespace insoLyrics
{
    internal partial class CanvasForm : GhostLayeredForm
    {
        #region Lyrics()

        public static CanvasForm Constructor;
        public LyricManager lyricManager = new LyricManager();

        public CanvasForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (Constructor == null)
            {
                Constructor = this;
            }
            InitializeComponent();

            //Osu.MessageReceived += Osu_MessageReceived;
            // Invoke these
            lyricManager.LyricChanged += (s, e) => Refresh();
            lyricManager.PlaySpeedChanged += (s, e) => Refresh();
            lyricManager.PlayTimeChanged += (s, e) => Refresh();
            lyricManager.AudioChanged += (s, e) => Refresh();
            Osu.KeyDown += Osu_KeyDown;
        }

        ~CanvasForm()
        {
            Osu.KeyDown -= Osu_KeyDown;
        }

        public override void Render(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawLyric(g);
        }

        #endregion

        private void Lyrics_Load(object sender, EventArgs e)
        {
            Notice(Constants._MutexName);
        }

        private async void Lyrics_Shown(object sender, EventArgs e)
        {
            // 打开对话框进行初始设置
            if (!File.Exists(Settings._Path))
            {
                BeginInvoke(new MethodInvoker(menuSetting.PerformClick));
            }
            while (!Osu.Process.HasExited)
            {
                if (Osu.IsForeground)
                {
                    if (!Location.Equals(Osu.ClientLocation))
                    {
                        Location = Osu.ClientLocation;
                    }
                    if (!Size.Equals(Osu.ClientSize))
                    {
                        Size = Osu.ClientSize;
                        Settings.DrawingOrigin = Point.Empty;
                    }
                    if (!(Settings?.Visible ?? false))
                    {
                        TopMost = true;
                    }
                    Visible = true;
                }
                else if (Settings?.Visible ?? false)
                {
                    Visible = true;
                }
                else if (Settings.ShowWhileOsuTop)
                {
                    Visible = false;
                }

                Refresh();

                await Task.Delay(Settings.RefreshRate);
            }
            Close();
        }







        #region Notice(...)

        private string _notice;

        private void Notice(string value)
        {
            timer1.Stop();

            _notice = value;
            Invoke(new MethodInvoker(Refresh));

            timer1.Start();
        }

        private void Notice(string format, params object[] args)
        {
            Notice(string.Format(format, args));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _notice = null;
            Invoke(new MethodInvoker(Invalidate));
        }

        #endregion

        private bool showLyric = true;

        private void DrawLyric(Graphics g)
        {
            if (_notice != null)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddString(
                        _notice, Settings.FontFamily, Settings.FontStyle, g.DpiY * 14 / 72, Point.Empty,
                        StringFormat.GenericDefault);
                    if (Settings.BorderWidth > 0)
                    {
                        g.DrawPath(Settings.Border, path);
                    }
                    g.FillPath(Settings.Brush, path);
                }
            }

            if (!showLyric)
            {
                return;
            }

            var lyricBuilder = new StringBuilder();
            var lyric = lyricManager.GetLyricAtNow();
            foreach (var l in lyricManager.TruncateLyric(lyric))
            {
                lyricBuilder.AppendLine(l.Text);
            }

            using (var path = new GraphicsPath())
            {
                path.AddString(
                    lyricBuilder.ToString(), Settings.FontFamily, Settings.FontStyle, g.DpiY * Settings.FontSize / 72,
                    Settings.DrawingOrigin, Settings.StringFormat);
                if (Settings.BorderWidth > 0)
                {
                    g.DrawPath(Settings.Border, path);
                }
                g.FillPath(Settings.Brush, path);
            }
        }





        private void Osu_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果正在设置，则不挂起键盘！
            if (Settings?.Visible ?? false)
            {
                return;
            }

            // 如果有匹配的热键，osu！防止摇晃传播
            e.SuppressKeyPress = Settings.SuppressKey;

            if (e.KeyData == Settings.KeyToggle)
            {
                showLyric = !showLyric;
                Notice("歌词 {0}", showLyric ? "显示" : "隐藏");
                return;
            }

            // 只在看到歌词的情况下处理的热键。
            if (!Settings.BlockSyncOnHide || (Settings.BlockSyncOnHide && showLyric))
            {
                if (e.KeyData == Settings.KeyBackward)
                {
                    lyricManager.LyricSync += 0.5;
                    Notice("缓慢下沉({0}秒)", lyricManager.LyricSync.ToString("F1"));
                    return;
                }
                if (e.KeyData == Settings.KeyForward)
                {
                    lyricManager.LyricSync -= 0.5;
                    Notice("快速下沉({0}秒)", lyricManager.LyricSync.ToString("F1"));
                    return;
                }
            }

            // 没有匹配的热键，所以将热键发送给 osu！
            e.SuppressKeyPress = false;
        }








        private void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Osu.Show();
            }
        }

        public static Settings Settings;

        private void menuSetting_Click(object sender, EventArgs e)
        {
            if (Settings == null)
            {
                Settings = new Settings
                {
                    TopMost = true
                };
                Settings.ShowDialog();
                Settings.Dispose();
                Settings = null;
            }
            else
            {
                Settings.TopMost = true;
                Settings.Focus();
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}