using PlayerAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using Babalouba.Properties;

namespace Babalouba
{
    public class Babalouba : Form
    {
        private const string PlaylistFileName = "playlist.bbl";
        private const string DefaultThemeFileName = "DefaultTheme.bth";
        private const string ConfigFileName = "babalouba.json";
        private int currentVolume;
        private ListBox lstCurrentPlaylistBox;
        public string currentSong;
        public Dictionary<string, string> mapPathFile;
        public Dictionary<string, string> mapFolder;
        private string previousSong;
        public Player player;
        private bool toTray;
        private bool isFullScreen;
        private Point location;
        private PlayListLoader playListLoader;
        private ThemeLoader themeLoader;
        private PlaylistCollection BblPlayList;
        private IContainer components;
        private GroupBox grpPlaylist;
        private TextBox edtFilter;
        private Label lblFilter;
        public ListBox lstPlayList;
        public Button btnVolume;
        public Button btnNext;
        public Button btnStop;
        public Button btnPrevious;
        public Button btnPlay;
        private TrackBar trcProgress;
        private Label label7;
        private Label label8;
        private Label label9;
        private TrackBar trcTreble;
        private Label label4;
        private Label label5;
        private Label label6;
        private TrackBar trcBass;
        private Label label3;
        private Label label2;
        private Label label1;
        private TrackBar trcBalance;
        private CheckBox chkMute;
        public TrackBar trcVolume;
        public Timer timer;
        private FolderBrowserDialog dlgAddFolder;
        private Label label14;
        private Label label13;
        private Label label12;
        private Label label11;
        private Button btnRemove;
        private Button btnAddFolder;
        private ListBox lstFolder;
        private PictureBox clrSettingFont;
        private PictureBox clrSettingBckg;
        private PictureBox clrMainFont;
        private PictureBox clrMainBckg;
        private ColorDialog dlgColor;
        private ToolTip tipError;
        private NotifyIcon icoNotify;
        private PictureBox btnSlowDown;
        private PictureBox btnSpeedUp;
        private Panel pnlName;
        private Panel pnlSpeed;
        private Label lblSpeed;
        private Panel pnlTime;
        private Label lblTime;
        private Label lblCurrentlyPlaying;
        private CheckBox chkFade;
        private Button btnFull;
        private TabControl tabSettings;
        private TabPage tabFolder;
        private TabPage tabTheme;
        private TabPage tabMixer;
        private GroupBox grpSettings;
        private GroupBox grpPlaylists;
        private Button btnRemovePlaylist;
        private Button btnAddPlaylist;
        private TabControl tabPlaylist1;
        private TabPage tabPage1;
        private ListBox lstPlaylist1;
        private ContextMenuStrip ctxPlaylist;
        private ToolStripMenuItem itmPlay;
        private ToolStripMenuItem itmStop;
        private ToolStripMenuItem itmSend;
        private ContextMenuStrip ctxPlaylists;
        private ToolStripMenuItem itmNew;
        private ToolStripMenuItem itmRename;
        private ToolStripMenuItem itmAdd;
        private ToolStripMenuItem itmDelSong;
        private ToolStripMenuItem itmDelPlaylist;
        private Button btnImportTheme;
        private Button btnExportTheme;
        private SaveFileDialog dlgExportTheme;
        private OpenFileDialog dlgImportTheme;
        private Configuration configuration;

        public Babalouba()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.InitializeComponent();
            this.lstCurrentPlaylistBox = this.lstPlayList;
            this.BblPlayList = new PlaylistCollection();
            this.playListLoader = new PlayListLoader();
            this.themeLoader = new ThemeLoader();
            this.currentSong = string.Empty;
            this.previousSong = string.Empty;
            this.mapPathFile = new Dictionary<string, string>();
            this.mapFolder = new Dictionary<string, string>();
            this.configuration = new Configuration();
            this.configuration.Load(ConfigFileName);
            this.configuration.LibraryFolders?.ForEach(folder => this.lstFolder.Items.Add((object)folder));
            this.currentVolume = 500;
            this.player = new Player("Media");
            this.player.OpenFile += new Player.OpenFileEventHandler(this.OnPlayerOpenFile);
            this.player.PlayFile += new Player.PlayFileEventHandler(this.OnplayerPlayFile);
            this.player.StopFile += new Player.StopFileEventHandler(this.OnPlayerStopFile);
            this.player.PauseFile += new Player.PauseFileEventHandler(this.OnPlayerPauseFile);
            this.player.SongEnd += new Player.SongEndEventHandler(this.OnPlayerSongEnd);
            this.player.Error += new Player.ErrorEventHandler(this.OnPlayerError);
            this.player.Fade += new Player.SongMixEventHandler(this.OnFade);
            this.trcVolume.Value = this.currentVolume / 50;
            this.toTray = false;
            this.isFullScreen = false;
            this.location = this.Location;
            this.dlgExportTheme.Filter = "BBL theme files (*.bth)|*.bth|All files (*.*)|*.*";
            this.dlgImportTheme.Filter = "BBL theme files (*.bth)|*.bth|All files (*.*)|*.*";
        }

        private void OnImportTheme(object sender, EventArgs e)
        {
            if (this.dlgImportTheme.ShowDialog() != DialogResult.OK)
                return;
            this.LoadTheme(this.dlgImportTheme.FileName);
        }

        private void OnExportTheme(object sender, EventArgs e)
        {
            if (this.dlgExportTheme.ShowDialog() != DialogResult.OK)
                return;
            this.SaveCurrentTheme(this.dlgExportTheme.FileName);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && this.isFullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Size = this.MinimumSize;
                this.isFullScreen = false;
                this.CenterToScreen();
            }
            base.OnKeyDown(e);
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            await SaveBeforeClosing();
            base.OnClosing(e);
        }

        private void OnDeleteSong(object sender, EventArgs e)
        {
            TabPage selectedTab = this.tabPlaylist1.SelectedTab;
            ListBox control = (ListBox)selectedTab.Controls[selectedTab.Name];
            foreach (PlayList playList in this.BblPlayList.PlayLists)
            {
                if (playList.Name == selectedTab.Name)
                    playList.Songs.RemoveAt(control.SelectedIndex);
            }
            control.Items.Remove(control.SelectedItem);
        }

        private void OnCtxPlaylistsOpening(object sender, CancelEventArgs e)
        {
            TabPage selectedTab = this.tabPlaylist1.SelectedTab;
            if (this.tabPlaylist1.Controls.Count > 0)
                this.itmDelSong.Enabled = ((ListBox)selectedTab.Controls[selectedTab.Name]).SelectedItems.Count > 0;
            else
                this.itmDelSong.Enabled = false;
        }

        private void OnAddPlaylist(object sender, EventArgs e)
        {
            CreateAndRename createAndRename = new CreateAndRename("");
            if (createAndRename.ShowDialog() != DialogResult.OK)
                return;
            TabPage playListTab = this.CreatePlayListTab(createAndRename.edtPlaylistName.Text);
            this.BblPlayList.PlayLists.Add(new PlayList()
            {
                Name = createAndRename.edtPlaylistName.Text
            });
            this.tabPlaylist1.Controls.Add((Control)playListTab);
        }

        private void OnRemovePlaylist(object sender, EventArgs e)
        {
            if (this.tabPlaylist1.SelectedIndex < 0)
                return;
            TabPage selectedTab = this.tabPlaylist1.SelectedTab;
            PlaylistCollection bbl = new PlaylistCollection();
            this.BblPlayList.PlayLists.RemoveAt(this.tabPlaylist1.SelectedIndex);
            this.tabPlaylist1.Controls.Remove((Control)selectedTab);
        }

        private void OnRenamePlaylist(object sender, EventArgs e)
        {
            CreateAndRename createAndRename = new CreateAndRename(this.tabPlaylist1.SelectedTab.Name);
            if (createAndRename.ShowDialog() != DialogResult.OK)
                return;
            this.BblPlayList.PlayLists[this.tabPlaylist1.SelectedIndex].Name = createAndRename.edtPlaylistName.Text;
            this.tabPlaylist1.SelectedTab.Text = createAndRename.edtPlaylistName.Text;
            this.tabPlaylist1.SelectedTab.Name = createAndRename.edtPlaylistName.Text;
            this.tabPlaylist1.SelectedTab.Controls[0].Name = createAndRename.edtPlaylistName.Text;
        }

        private void OnFullScreen(object sender, EventArgs e)
        {
            if (!this.isFullScreen)
            {
                this.Location = new Point(0, 0);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = Screen.PrimaryScreen.Bounds.Size;
                this.isFullScreen = true;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Size = this.MinimumSize;
                this.isFullScreen = false;
                this.CenterToScreen();
            }
        }

        private void OncmbMediaTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.FillListByFiltered();
        }

        private void OnCtxPlaylistOpening(object sender, CancelEventArgs e)
        {
            this.lstPlayList.Focus();
            this.itmSend.Enabled = this.lstPlayList.SelectedItems.Count > 0 && this.BblPlayList.PlayLists.Count > 0;
            if (this.itmSend.Enabled)
            {
                this.itmSend.DropDownItems.Clear();
                foreach (PlayList playList in this.BblPlayList.PlayLists)
                {
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(playList.Name);
                    toolStripMenuItem.Name = playList.Name;
                    toolStripMenuItem.Click += new EventHandler(this.OnSendTo);
                    this.itmSend.DropDownItems.Add((ToolStripItem)toolStripMenuItem);
                }
            }
            else
                this.itmSend.Enabled = false;
        }

        private void OnSendTo(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            foreach (PlayList playList in this.BblPlayList.PlayLists)
            {
                if (toolStripMenuItem.Name == playList.Name)
                {
                    ListBox control = (ListBox)this.tabPlaylist1.Controls[playList.Name].Controls[playList.Name];
                    Song song = new Song();
                    song.Name = this.lstPlayList.SelectedItem.ToString();
                    if (!control.Items.Contains((object)song.Name))
                    {
                        control.Items.Add(this.lstPlayList.SelectedItem);
                        playList.Songs.Add(song);
                    }
                }
            }
        }

        private void OnFade(object sender, EventArgs e)
        {
        }

        private void OnSpeedUp(object sender, EventArgs e)
        {
            this.player.Speed += 100;
            this.lblSpeed.Text = ((double)((float)this.player.Speed / 1000f)).ToString() + "x";
        }

        private void OnSlowDown(object sender, EventArgs e)
        {
            this.player.Speed -= 100;
            this.lblSpeed.Text = ((double)((float)this.player.Speed / 1000f)).ToString() + "x";
        }

        private void OnicoNotifyRClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            this.GotoTray();
        }

        private void OnResize(object sender, EventArgs e)
        {
        }

        private void OnPlayerError(object sender, Player.ErrorEventArgs e)
        {
            if (e.ErrorCode != 277)
                return;
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
            this.icoNotify.ShowBalloonTip(3000, "BABALOUBA Cannot read", "Cannot read " + this.currentSong + ". " + e.ErrorString, ToolTipIcon.Error);
            if (this.lstCurrentPlaylistBox.Items.Count > 1)
                this.PlayNext();
            else
                this.SetStopState();
        }

        private void OnPlayerOpenFile(object sender, Player.OpenFileEventArgs e)
        {
            this.trcProgress.Maximum = (int)(this.player.AudioLength / 1000UL);
            this.Text = e.FileName;
            this.trcProgress.Value = 0;
            this.timer.Enabled = false;
        }

        private void OnPlayerStopFile(object sender, Player.StopFileEventArgs e)
        {
            this.SetStopState();
        }

        private void OnPlayerSongEnd(object sender, Player.SongEndEventArgs e)
        {
            this.PlayNext();
        }

        private void OnPlayerPauseFile(object sender, Player.PauseFileEventArgs e)
        {
            this.Text = this.icoNotify.Text = "BABALOUBA: Paused";
            this.timer.Enabled = false;
        }

        private void OnProgressScroll(object sender, EventArgs e)
        {
            this.player.Seek((ulong)(this.trcProgress.Value * 1000));
        }

        private void OnProgressMouseUp(object sender, MouseEventArgs e)
        {
            this.player.Seek((ulong)(this.trcProgress.Value * 1000));
        }

        private void OnplayerPlayFile(object sender, Player.PlayFileEventArgs e)
        {
            this.trcProgress.Maximum = (int)(this.player.AudioLength / 1000UL);
            string withoutExtension = Path.GetFileNameWithoutExtension(this.player.FileName);
            NotifyIcon icoNotify = this.icoNotify;
            string str1 = withoutExtension.Length > 40 ? withoutExtension.Substring(0, 40) + "..." : withoutExtension;
            string str2;
            string str3 = str2 = "BABALOUBA: Playing " + str1;
            icoNotify.Text = str2;
            this.Text = str3;
            this.timer.Enabled = true;
            this.player.VolumeAll = this.trcVolume.Value * 50;
            this.lblCurrentlyPlaying.Text = withoutExtension;
            this.trcProgress.Enabled = true;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.trcProgress.Value = (int)(this.player.CurrentPosition / 1000UL);
            this.lblTime.Text = TimeSpan.FromMilliseconds((double)(int)this.player.CurrentPosition).ToString().Substring(0, 8) + "/" + TimeSpan.FromMilliseconds((double)(int)this.player.AudioLength).ToString().Substring(0, 8);
        }

        private void Fade()
        {
            if (this.player.CurrentPosition < this.player.AudioLength - 10000UL)
                return;
            int volumeAll = this.player.VolumeAll;
            Player player = new Player(this.player);
            player.VolumeAll = 0;
            this.lstCurrentPlaylistBox.SelectedItem = (object)this.currentSong;
            if (this.lstCurrentPlaylistBox.SelectedIndex + 1 <= this.lstCurrentPlaylistBox.Items.Count - 1)
            {
                ++this.lstCurrentPlaylistBox.SelectedIndex;
                this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
            }
            else
                this.lstCurrentPlaylistBox.SelectedIndex = 0;
            player.Open(this.mapPathFile[this.lstCurrentPlaylistBox.SelectedItem.ToString()]);
            player.Play();
            this.btnPlay.Image = (Image)Resources.pause;
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
            while ((long)this.player.CurrentPosition != (long)this.player.AudioLength)
            {
                ++player.VolumeAll;
                --this.player.VolumeAll;
            }
            this.player.Stop();
            this.player = player;
        }

        private void OnVolumeValueChanged(object sender, EventArgs e)
        {
            int num = this.trcVolume.Value * 50;
            this.player.VolumeAll = num;
            this.chkMute.Checked = this.trcVolume.Value == 0;
            new ToolTip().Show(((int)((double)num * 0.1)).ToString() + "%", (IWin32Window)this, new Point(this.trcVolume.Location.X + 10, this.trcVolume.Location.Y + 15), 1000);
        }

        private void OnVolumeScroll(object sender, EventArgs e)
        {
            int num = this.trcVolume.Value * 50;
            this.player.VolumeAll = num;
            this.chkMute.Checked = this.trcVolume.Value == 0;
            new ToolTip().Show(((int)((double)num * 0.1)).ToString() + "%", (IWin32Window)this, new Point(this.trcVolume.Location.X + 10, this.trcVolume.Location.Y + 15), 1000);
        }

        private void OnBalanceValueChanged(object sender, EventArgs e)
        {
            this.player.Balance = this.trcBalance.Value * 100;
        }

        private void OnTrebleValueChanged(object sender, EventArgs e)
        {
            this.player.VolumeTreble = this.trcTreble.Value * 50;
        }

        private void OnBassValueChanged(object sender, EventArgs e)
        {
            this.player.VolumeBass = this.trcBass.Value * 50;
        }

        private void OnPlay(object sender, EventArgs e)
        {
            if (this.lstCurrentPlaylistBox.Items.Count <= 0)
                return;
            if (this.lstCurrentPlaylistBox.SelectedItems.Count <= 0)
                this.lstCurrentPlaylistBox.SelectedIndex = 0;
            this.PlayCurrentlySelected();
        }

        private void OnPlaylistDoubleClick(object sender, EventArgs e)
        {
            this.lstCurrentPlaylistBox = (ListBox)this.ActiveControl;
            if (this.lstCurrentPlaylistBox.SelectedItems.Count <= 0)
                return;
            this.player.Stop();
            this.player.Open(this.mapPathFile[this.lstCurrentPlaylistBox.SelectedItem.ToString()]);
            this.player.Play();
            this.btnPlay.Image = (Image)Resources.pause;
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
        }

        private void OnStop(object sender, EventArgs e)
        {
            this.player.Stop();
            this.trcProgress.Enabled = false;
        }

        private void OnNext(object sender, EventArgs e)
        {
            this.PlayNext();
        }

        private void OnVolumeClicked(object sender, EventArgs e)
        {
            this.trcVolume.Visible = !this.trcVolume.Visible;
        }

        private void OnHideVolume(object sender, EventArgs e)
        {
            if (this.trcVolume.Focused)
                return;
            this.trcVolume.Visible = false;
        }

        private void OnMuteCheckedChanged(object sender, EventArgs e)
        {
            if (this.chkMute.Checked)
            {
                this.currentVolume = this.trcVolume.Value != 0 ? this.trcVolume.Value : 5;
                this.player.VolumeAll = 0;
                this.trcVolume.Value = 0;
                this.btnVolume.Image = (Image)Resources.mute;
            }
            else
            {
                this.player.VolumeAll = this.currentVolume * 50;
                this.trcVolume.Value = this.currentVolume;
                this.btnVolume.Image = (Image)Resources.volume;
            }
        }

        private void OnAddFolder(object sender, EventArgs e)
        {
            if (this.dlgAddFolder.ShowDialog() != DialogResult.OK)
                return;
            AddFolder(this.dlgAddFolder.SelectedPath);
            this.FillMap();
        }

        private void AddFolder(string folder)
        {
            this.lstFolder.Items.Add(folder);
            this.configuration.LibraryFolders.Add(folder);
        }

        private void RemoveSelectedFolder()
        {
            this.lstFolder.Items.RemoveAt(this.lstFolder.SelectedIndex);
            this.configuration.LibraryFolders.RemoveAt(this.lstFolder.SelectedIndex);
        }

        private void OnRemoveFolder(object sender, EventArgs e)
        {
            if (this.lstFolder.SelectedItems.Count <= 0)
                return;
            RemoveSelectedFolder();
            this.FillMap();
        }

        private void OnListfolderSelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnRemove.Enabled = this.lstFolder.SelectedItems.Count > 0;
        }

        private void OnclrMainBckgClick(object sender, EventArgs e)
        {
            if (this.dlgColor.ShowDialog() != DialogResult.OK)
                return;
            this.ApplyMainBcgColor(this.dlgColor.Color);
        }

        private void OnclrMainFontClick(object sender, EventArgs e)
        {
            if (this.dlgColor.ShowDialog() != DialogResult.OK)
                return;
            this.ApplyMainFontColor(this.dlgColor.Color);
        }

        private void OnclrSettingBckgClick(object sender, EventArgs e)
        {
            if (this.dlgColor.ShowDialog() != DialogResult.OK)
                return;
            this.ApplySettingsBcgColor(this.dlgColor.Color);
        }

        private void OnclrSettingFontClick(object sender, EventArgs e)
        {
            if (this.dlgColor.ShowDialog() != DialogResult.OK)
                return;
            this.ApplySettingsFontColor(this.dlgColor.Color);
            this.clrSettingFont.BackColor = this.dlgColor.Color;
        }

        private TabPage CreatePlayListTab(string playlistName)
        {
            ListBox listBox = new ListBox();
            listBox.AllowDrop = true;
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox.BackColor = Color.Black;
            listBox.ContextMenuStrip = this.ctxPlaylists;
            listBox.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(3, 4);
            listBox.Name = playlistName;
            listBox.Size = new Size(198, 100);
            listBox.TabIndex = 0;
            listBox.DoubleClick += new EventHandler(this.OnPlaylistDoubleClick);
            TabPage tabPage = new TabPage();
            tabPage.BackColor = Color.Black;
            tabPage.Controls.Add((Control)listBox);
            tabPage.Location = new Point(4, 22);
            tabPage.Name = playlistName;
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(342, 248);
            tabPage.TabIndex = 0;
            tabPage.Text = playlistName;
            return tabPage;
        }

        private bool LoadPlayLists()
        {
            this.tabPlaylist1.Controls.Clear();
            if (!File.Exists(PlaylistFileName))
            {
                PlayList playList = new PlayList();
                playList.Name = "PlayList1";
                this.BblPlayList = new PlaylistCollection();
                this.BblPlayList.PlayLists.Add(playList);
                this.playListLoader.SavePlayLists(this.BblPlayList, PlaylistFileName);
            }
            this.BblPlayList = this.playListLoader.LoadPlayLists(PlaylistFileName);
            List<Song> songList = new List<Song>();
            if (this.playListLoader.Success)
            {
                foreach (PlayList playList in this.BblPlayList.PlayLists)
                {
                    TabPage playListTab = this.CreatePlayListTab(playList.Name);
                    foreach (Song song in playList.Songs)
                    {
                        if (this.mapPathFile.ContainsKey(song.Name))
                            ((ListBox)playListTab.Controls[playList.Name]).Items.Add((object)song.Name);
                    }
                    foreach (Song song in songList)
                        playList.Songs.Remove(song);
                    this.tabPlaylist1.Controls.Add((Control)playListTab);
                }
            }
            return this.playListLoader.Success;
        }

        private bool LoadTheme()
        {
            if (!File.Exists(DefaultThemeFileName))
                this.SaveCurrentTheme(DefaultThemeFileName);
            this.LoadTheme(DefaultThemeFileName);
            return true;
        }

        private void LoadTheme(string filePath)
        {
            Theme theme = this.themeLoader.LoadTheme(filePath);
            if (!this.themeLoader.Success)
                return;
            this.ApplyMainBcgColor(Color.FromArgb(theme.MainBackColor.A, theme.MainBackColor.R, theme.MainBackColor.G, theme.MainBackColor.B));
            this.ApplyMainFontColor(Color.FromArgb(theme.MainFontColor.A, theme.MainFontColor.R, theme.MainFontColor.G, theme.MainFontColor.B));
            this.ApplySettingsBcgColor(Color.FromArgb(theme.SettingsBackColor.A, theme.SettingsBackColor.R, theme.SettingsBackColor.G, theme.SettingsBackColor.B));
            this.ApplySettingsFontColor(Color.FromArgb(theme.SettingsFontColor.A, theme.SettingsFontColor.R, theme.SettingsFontColor.G, theme.SettingsFontColor.B));
        }

        private void SaveCurrentTheme(string filePath)
        {
            this.themeLoader.SaveTheme(new Theme()
            {
                MainBackColor = {
          A = (int) this.clrMainBckg.BackColor.A,
          R = (int) this.clrMainBckg.BackColor.R,
          G = (int) this.clrMainBckg.BackColor.G,
          B = (int) this.clrMainBckg.BackColor.B
        },
                MainFontColor = {
          A = (int) this.clrMainFont.BackColor.A,
          R = (int) this.clrMainFont.BackColor.R,
          G = (int) this.clrMainFont.BackColor.G,
          B = (int) this.clrMainFont.BackColor.B
        },
                SettingsBackColor = {
          A = (int) this.clrSettingBckg.BackColor.A,
          R = (int) this.clrSettingBckg.BackColor.R,
          G = (int) this.clrSettingBckg.BackColor.G,
          B = (int) this.clrSettingBckg.BackColor.B
        },
                SettingsFontColor = {
          A = (int) this.clrSettingFont.BackColor.A,
          R = (int) this.clrSettingFont.BackColor.R,
          G = (int) this.clrSettingFont.BackColor.G,
          B = (int) this.clrSettingFont.BackColor.B
        }
            }, filePath);
        }

        private async Task<bool> SaveBeforeClosing()
        {
            this.playListLoader.SavePlayLists(this.BblPlayList, PlaylistFileName);
            this.SaveCurrentTheme(DefaultThemeFileName);
            await this.configuration.Save(ConfigFileName);
            return this.playListLoader.Success;
        }

        private void GotoTray()
        {
            if (this.toTray)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.toTray = false;
            }
            else
            {
                this.Hide();
                this.toTray = true;
            }
        }

        private void SetStopState()
        {
            this.Text = this.icoNotify.Text = "BABALOUBA: Stopped";
            this.timer.Enabled = false;
            this.trcProgress.Value = 0;
            this.btnPlay.Image = (Image)Resources.play;
            this.itmPlay.Text = "Play";
            this.lblTime.Text = "--:--:--/--:--:--";
            this.lblCurrentlyPlaying.Text = string.Empty;
        }

        private void FillMap()
        {
            this.mapPathFile.Clear();
            foreach (string path1 in this.lstFolder.Items)
            {
                foreach (string path2 in ((IEnumerable<string>)Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories)).Where<string>((Func<string, bool>)(s => this.IsAudio(s))))
                    this.mapPathFile[Path.GetFileName(path2)] = path2;
            }
            this.FillListWithFilter();
        }

        private void UpdateFileNumber()
        {
            this.grpPlaylist.Text = "Playlist : " + (object)this.lstPlayList.Items.Count + "/" + (object)this.mapPathFile.Count + " songs";
        }

        private void FillList()
        {
            this.lstPlayList.Items.Clear();
            foreach (KeyValuePair<string, string> keyValuePair in this.mapPathFile)
                this.lstPlayList.Items.Add((object)keyValuePair.Key);
            this.UpdateFileNumber();
        }

        private void FillListByFiltered()
        {
            List<string> filteredList = this.GetFilteredList();
            this.lstPlayList.Items.Clear();
            this.lstPlayList.Items.AddRange((object[])filteredList.ToArray());
            this.UpdateFileNumber();
        }

        private void OnBabaloubaLoad(object sender, EventArgs e)
        {
            this.FillMap();
            this.LoadPlayLists();
            this.LoadTheme();
        }

        private void PlayCurrentlySelected()
        {
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
            if (this.player.IsPaused)
            {
                this.player.Play();
                this.btnPlay.Image = (Image)Resources.pause;
                this.itmPlay.Text = "Pause";
            }
            else if (this.player.IsPlaying)
            {
                this.player.Pause();
                this.btnPlay.Image = (Image)Resources.play;
                this.itmPlay.Text = "Play";
            }
            else
            {
                this.player.Stop();
                this.player.Open(this.mapPathFile[this.lstCurrentPlaylistBox.SelectedItem.ToString()]);
                this.player.Play();
                this.btnPlay.Image = (Image)Resources.pause;
                this.itmPlay.Text = "Pause";
            }
            this.lstCurrentPlaylistBox.SelectedItem = (object)Path.GetFileName(this.player.FileName);
        }

        private void PlayNext()
        {
            if (this.lstCurrentPlaylistBox.SelectedItems.Count <= 0)
                this.lstCurrentPlaylistBox.SelectedIndex = 0;
            this.lstCurrentPlaylistBox.SelectedItem = (object)this.currentSong;
            if (this.lstCurrentPlaylistBox.SelectedIndex + 1 <= this.lstCurrentPlaylistBox.Items.Count - 1)
            {
                ++this.lstCurrentPlaylistBox.SelectedIndex;
                this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
            }
            else
                this.lstCurrentPlaylistBox.SelectedIndex = 0;
            this.player.Stop();
            this.player.Open(this.mapPathFile[this.lstCurrentPlaylistBox.SelectedItem.ToString()]);
            this.player.Play();
            this.btnPlay.Image = (Image)Resources.pause;
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
        }

        private void PlayPrevious()
        {
            if (this.lstCurrentPlaylistBox.Items.Count <= 0)
                return;
            if (this.lstCurrentPlaylistBox.SelectedItems.Count <= 0)
                this.lstCurrentPlaylistBox.SelectedIndex = 0;
            this.lstCurrentPlaylistBox.SelectedItem = (object)this.currentSong;
            if (this.lstCurrentPlaylistBox.SelectedIndex - 1 >= 0)
                --this.lstCurrentPlaylistBox.SelectedIndex;
            else
                this.lstCurrentPlaylistBox.SelectedIndex = this.lstCurrentPlaylistBox.Items.Count - 1;
            this.player.Stop();
            this.player.Open(this.mapPathFile[this.lstCurrentPlaylistBox.SelectedItem.ToString()]);
            this.player.Play();
            this.btnPlay.Image = (Image)Resources.pause;
            this.currentSong = this.lstCurrentPlaylistBox.SelectedItem.ToString();
        }

        private void OnPrevious(object sender, EventArgs e)
        {
            this.PlayPrevious();
        }

        private void OnFilter(object sender, EventArgs e)
        {
            this.FillListWithFilter();
        }

        private bool ContainsWordlist(string fileName, string[] words)
        {
            int index = 0;
            foreach (string word in words)
            {
                if (!fileName.ToLower().Contains(words[index].ToLower()))
                    return false;
                ++index;
            }
            return true;
        }

        private List<string> GetFilteredList()
        {
            List<string> stringList = new List<string>();
            string[] words = this.edtFilter.Text.Split(' ', ',', '.', ':', '\t');
            foreach (KeyValuePair<string, string> keyValuePair in this.mapPathFile)
            {
                if (this.ContainsWordlist(keyValuePair.Key, words) && !stringList.Contains(keyValuePair.Key))
                    stringList.Add(keyValuePair.Key);
            }
            return stringList;
        }

        private Babalouba.MediaTypes GetMediaType(string mediaName)
        {
            if (mediaName.EndsWith(".avi") || mediaName.EndsWith(".mp4") || (mediaName.EndsWith(".flv") || mediaName.EndsWith(".wmv")))
                return Babalouba.MediaTypes.Video;
            return mediaName.EndsWith(".mp3") || mediaName.EndsWith(".wav") ? Babalouba.MediaTypes.Audio : Babalouba.MediaTypes.Unknown;
        }

        private bool IsAudio(string mediaName)
        {
            return mediaName.EndsWith(".mp3") || mediaName.EndsWith(".wav");
        }

        private void OnListSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ApplyMainBcgColor(Color color)
        {
            Color backColor = this.tabMixer.BackColor;
            this.BackColor = color;
            this.clrMainBckg.BackColor = color;
            this.tabMixer.BackColor = backColor;
            this.grpSettings.BackColor = backColor;
            this.tabMixer.BackColor = backColor;
            this.grpPlaylists.BackColor = backColor;
        }

        private void ApplyMainFontColor(Color color)
        {
            Color foreColor = this.tabMixer.ForeColor;
            this.ForeColor = color;
            this.grpPlaylist.ForeColor = color;
            this.lblCurrentlyPlaying.ForeColor = color;
            this.lblSpeed.ForeColor = color;
            this.lblTime.ForeColor = color;
            this.clrMainFont.BackColor = color;
            this.tabMixer.ForeColor = foreColor;
            this.grpSettings.ForeColor = foreColor;
            this.tabMixer.ForeColor = foreColor;
        }

        private void ApplySettingsBcgColor(Color color)
        {
            Color backColor1 = this.clrMainBckg.BackColor;
            Color backColor2 = this.lstFolder.BackColor;
            this.tabMixer.BackColor = color;
            this.grpSettings.BackColor = color;
            this.tabMixer.BackColor = color;
            this.tabFolder.BackColor = color;
            this.tabTheme.BackColor = color;
            this.grpPlaylists.BackColor = color;
            this.clrSettingBckg.BackColor = color;
            foreach (TabPage control in (ArrangedElementCollection)this.tabPlaylist1.Controls)
            {
                control.BackColor = color;
                control.Controls[control.Name].BackColor = backColor2;
            }
            this.clrMainBckg.BackColor = backColor1;
            this.lstFolder.BackColor = backColor2;
        }

        private void ApplySettingsFontColor(Color color)
        {
            Color foreColor = this.lstFolder.ForeColor;
            this.tabMixer.ForeColor = color;
            this.grpSettings.ForeColor = color;
            this.tabTheme.ForeColor = color;
            this.lstFolder.ForeColor = color;
            this.grpPlaylists.ForeColor = color;
            this.clrSettingFont.BackColor = color;
            this.lstFolder.ForeColor = foreColor;
        }

        private void FillListWithFilter()
        {
            if (this.edtFilter.Text != string.Empty)
                this.FillListByFiltered();
            else
                this.FillList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Babalouba));
            this.grpPlaylist = new GroupBox();
            this.btnFull = new Button();
            this.trcVolume = new TrackBar();
            this.btnSpeedUp = new PictureBox();
            this.btnSlowDown = new PictureBox();
            this.chkMute = new CheckBox();
            this.btnVolume = new Button();
            this.btnNext = new Button();
            this.btnStop = new Button();
            this.btnPrevious = new Button();
            this.btnPlay = new Button();
            this.edtFilter = new TextBox();
            this.lblFilter = new Label();
            this.lstPlayList = new ListBox();
            this.ctxPlaylist = new ContextMenuStrip(this.components);
            this.itmPlay = new ToolStripMenuItem();
            this.itmStop = new ToolStripMenuItem();
            this.itmSend = new ToolStripMenuItem();
            this.trcProgress = new TrackBar();
            this.chkFade = new CheckBox();
            this.label7 = new Label();
            this.label8 = new Label();
            this.label9 = new Label();
            this.trcTreble = new TrackBar();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
            this.trcBass = new TrackBar();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.trcBalance = new TrackBar();
            this.timer = new Timer(this.components);
            this.dlgAddFolder = new FolderBrowserDialog();
            this.clrSettingFont = new PictureBox();
            this.clrSettingBckg = new PictureBox();
            this.clrMainFont = new PictureBox();
            this.clrMainBckg = new PictureBox();
            this.label14 = new Label();
            this.label13 = new Label();
            this.label12 = new Label();
            this.label11 = new Label();
            this.btnRemove = new Button();
            this.btnAddFolder = new Button();
            this.lstFolder = new ListBox();
            this.dlgColor = new ColorDialog();
            this.tipError = new ToolTip(this.components);
            this.icoNotify = new NotifyIcon(this.components);
            this.pnlName = new Panel();
            this.lblCurrentlyPlaying = new Label();
            this.pnlSpeed = new Panel();
            this.lblSpeed = new Label();
            this.pnlTime = new Panel();
            this.lblTime = new Label();
            this.tabSettings = new TabControl();
            this.tabFolder = new TabPage();
            this.tabTheme = new TabPage();
            this.btnImportTheme = new Button();
            this.btnExportTheme = new Button();
            this.tabMixer = new TabPage();
            this.grpSettings = new GroupBox();
            this.grpPlaylists = new GroupBox();
            this.btnRemovePlaylist = new Button();
            this.btnAddPlaylist = new Button();
            this.tabPlaylist1 = new TabControl();
            this.tabPage1 = new TabPage();
            this.lstPlaylist1 = new ListBox();
            this.ctxPlaylists = new ContextMenuStrip(this.components);
            this.itmNew = new ToolStripMenuItem();
            this.itmRename = new ToolStripMenuItem();
            this.itmDelPlaylist = new ToolStripMenuItem();
            this.itmAdd = new ToolStripMenuItem();
            this.itmDelSong = new ToolStripMenuItem();
            this.dlgExportTheme = new SaveFileDialog();
            this.dlgImportTheme = new OpenFileDialog();
            this.grpPlaylist.SuspendLayout();
            this.trcVolume.BeginInit();
            ((ISupportInitialize)this.btnSpeedUp).BeginInit();
            ((ISupportInitialize)this.btnSlowDown).BeginInit();
            this.ctxPlaylist.SuspendLayout();
            this.trcProgress.BeginInit();
            this.trcTreble.BeginInit();
            this.trcBass.BeginInit();
            this.trcBalance.BeginInit();
            ((ISupportInitialize)this.clrSettingFont).BeginInit();
            ((ISupportInitialize)this.clrSettingBckg).BeginInit();
            ((ISupportInitialize)this.clrMainFont).BeginInit();
            ((ISupportInitialize)this.clrMainBckg).BeginInit();
            this.pnlName.SuspendLayout();
            this.pnlSpeed.SuspendLayout();
            this.pnlTime.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabFolder.SuspendLayout();
            this.tabTheme.SuspendLayout();
            this.tabMixer.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpPlaylists.SuspendLayout();
            this.tabPlaylist1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.ctxPlaylists.SuspendLayout();
            this.SuspendLayout();
            this.grpPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.grpPlaylist.Controls.Add((Control)this.btnFull);
            this.grpPlaylist.Controls.Add((Control)this.trcVolume);
            this.grpPlaylist.Controls.Add((Control)this.btnSpeedUp);
            this.grpPlaylist.Controls.Add((Control)this.btnSlowDown);
            this.grpPlaylist.Controls.Add((Control)this.chkMute);
            this.grpPlaylist.Controls.Add((Control)this.btnVolume);
            this.grpPlaylist.Controls.Add((Control)this.btnNext);
            this.grpPlaylist.Controls.Add((Control)this.btnStop);
            this.grpPlaylist.Controls.Add((Control)this.btnPrevious);
            this.grpPlaylist.Controls.Add((Control)this.btnPlay);
            this.grpPlaylist.Controls.Add((Control)this.edtFilter);
            this.grpPlaylist.Controls.Add((Control)this.lblFilter);
            this.grpPlaylist.Controls.Add((Control)this.lstPlayList);
            this.grpPlaylist.Controls.Add((Control)this.trcProgress);
            this.grpPlaylist.ForeColor = Color.Yellow;
            this.grpPlaylist.Location = new Point(5, 12);
            this.grpPlaylist.Name = "grpPlaylist";
            this.grpPlaylist.Size = new Size(562, 553);
            this.grpPlaylist.TabIndex = 0;
            this.grpPlaylist.TabStop = false;
            this.grpPlaylist.Text = "Play list";
            this.btnFull.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnFull.Image = (Image)Resources.full;
            this.btnFull.Location = new Point(186, 515);
            this.btnFull.Name = "btnFull";
            this.btnFull.Size = new Size(30, 30);
            this.btnFull.TabIndex = 15;
            this.btnFull.UseVisualStyleBackColor = true;
            this.btnFull.Click += new EventHandler(this.OnFullScreen);
            this.trcVolume.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.trcVolume.Location = new Point(504, 393);
            this.trcVolume.Maximum = 20;
            this.trcVolume.Name = "trcVolume";
            this.trcVolume.Orientation = Orientation.Vertical;
            this.trcVolume.Size = new Size(45, 118);
            this.trcVolume.TabIndex = 11;
            this.trcVolume.TickStyle = TickStyle.Both;
            this.trcVolume.Value = 20;
            this.trcVolume.Visible = false;
            this.trcVolume.Scroll += new EventHandler(this.OnVolumeScroll);
            this.trcVolume.LostFocus += new EventHandler(this.OnHideVolume);
            this.btnSpeedUp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSpeedUp.Image = (Image)Resources.speed;
            this.btnSpeedUp.Location = new Point(542, 490);
            this.btnSpeedUp.Name = "btnSpeedUp";
            this.btnSpeedUp.Size = new Size(12, 12);
            this.btnSpeedUp.TabIndex = 14;
            this.btnSpeedUp.TabStop = false;
            this.btnSpeedUp.Click += new EventHandler(this.OnSpeedUp);
            this.btnSlowDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnSlowDown.Image = (Image)Resources.speedPlus;
            this.btnSlowDown.Location = new Point(9, 490);
            this.btnSlowDown.Name = "btnSlowDown";
            this.btnSlowDown.Size = new Size(12, 12);
            this.btnSlowDown.TabIndex = 13;
            this.btnSlowDown.TabStop = false;
            this.btnSlowDown.Click += new EventHandler(this.OnSlowDown);
            this.chkMute.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.chkMute.AutoSize = true;
            this.chkMute.Location = new Point(464, 524);
            this.chkMute.Name = "chkMute";
            this.chkMute.Size = new Size(50, 17);
            this.chkMute.TabIndex = 12;
            this.chkMute.Text = "Mute";
            this.chkMute.UseVisualStyleBackColor = true;
            this.chkMute.CheckedChanged += new EventHandler(this.OnMuteCheckedChanged);
            this.btnVolume.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnVolume.Image = (Image)Resources.volume;
            this.btnVolume.Location = new Point(513, 515);
            this.btnVolume.Name = "btnVolume";
            this.btnVolume.Size = new Size(30, 30);
            this.btnVolume.TabIndex = 9;
            this.btnVolume.UseVisualStyleBackColor = true;
            this.btnVolume.Click += new EventHandler(this.OnVolumeClicked);
            this.btnVolume.LostFocus += new EventHandler(this.OnHideVolume);
            this.btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnNext.Image = (Image)Resources.next;
            this.btnNext.Location = new Point(140, 515);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new Size(30, 30);
            this.btnNext.TabIndex = 8;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new EventHandler(this.OnNext);
            this.btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnStop.Image = (Image)Resources.stop;
            this.btnStop.Location = new Point(109, 515);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(30, 30);
            this.btnStop.TabIndex = 7;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new EventHandler(this.OnStop);
            this.btnPrevious.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnPrevious.Image = (Image)Resources.previous;
            this.btnPrevious.Location = new Point(79, 515);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new Size(30, 30);
            this.btnPrevious.TabIndex = 6;
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new EventHandler(this.OnPrevious);
            this.btnPlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnPlay.Image = (Image)Resources.play;
            this.btnPlay.Location = new Point(17, 515);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new Size(30, 30);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new EventHandler(this.OnPlay);
            this.edtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.edtFilter.BackColor = Color.Black;
            this.edtFilter.BorderStyle = BorderStyle.FixedSingle;
            this.edtFilter.ForeColor = Color.Yellow;
            this.edtFilter.Location = new Point(42, 23);
            this.edtFilter.Name = "edtFilter";
            this.edtFilter.Size = new Size(514, 20);
            this.edtFilter.TabIndex = 3;
            this.edtFilter.TextChanged += new EventHandler(this.OnFilter);
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new Point(7, 28);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new Size(29, 13);
            this.lblFilter.TabIndex = 2;
            this.lblFilter.Text = "Filter";
            this.lstPlayList.AllowDrop = true;
            this.lstPlayList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstPlayList.BackColor = Color.Black;
            this.lstPlayList.ContextMenuStrip = this.ctxPlaylist;
            this.lstPlayList.ForeColor = Color.White;
            this.lstPlayList.FormattingEnabled = true;
            this.lstPlayList.Location = new Point(6, 51);
            this.lstPlayList.Name = "lstPlayList";
            this.lstPlayList.Size = new Size(550, 433);
            this.lstPlayList.TabIndex = 1;
            this.lstPlayList.DoubleClick += new EventHandler(this.OnPlaylistDoubleClick);
            this.ctxPlaylist.Items.AddRange(new ToolStripItem[3]
            {
        (ToolStripItem) this.itmPlay,
        (ToolStripItem) this.itmStop,
        (ToolStripItem) this.itmSend
            });
            this.ctxPlaylist.Name = "ctxPlaylists";
            this.ctxPlaylist.Size = new Size(155, 70);
            this.ctxPlaylist.Opening += new CancelEventHandler(this.OnCtxPlaylistOpening);
            this.itmPlay.Name = "itmPlay";
            this.itmPlay.Size = new Size(154, 22);
            this.itmPlay.Text = "Play";
            this.itmPlay.Click += new EventHandler(this.OnPlay);
            this.itmStop.Name = "itmStop";
            this.itmStop.Size = new Size(154, 22);
            this.itmStop.Text = "Stop";
            this.itmStop.Click += new EventHandler(this.OnStop);
            this.itmSend.Name = "itmSend";
            this.itmSend.Size = new Size(154, 22);
            this.itmSend.Text = "Send to playlist";
            this.trcProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.trcProgress.Enabled = false;
            this.trcProgress.Location = new Point(17, 474);
            this.trcProgress.Name = "trcProgress";
            this.trcProgress.Size = new Size(526, 45);
            this.trcProgress.TabIndex = 10;
            this.trcProgress.TickStyle = TickStyle.Both;
            this.trcProgress.Scroll += new EventHandler(this.OnProgressScroll);
            this.trcProgress.MouseUp += new MouseEventHandler(this.OnProgressMouseUp);
            this.chkFade.AutoSize = true;
            this.chkFade.Location = new Point(76, 166);
            this.chkFade.Name = "chkFade";
            this.chkFade.Size = new Size(75, 17);
            this.chkFade.TabIndex = 15;
            this.chkFade.Text = "Auto Fade";
            this.chkFade.UseVisualStyleBackColor = true;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(60, 118);
            this.label7.Name = "label7";
            this.label7.Size = new Size(10, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "-";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(274, 118);
            this.label8.Name = "label8";
            this.label8.Size = new Size(13, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "+";
            this.label9.AutoSize = true;
            this.label9.Location = new Point(155, 143);
            this.label9.Name = "label9";
            this.label9.Size = new Size(37, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Treble";
            this.trcTreble.Location = new Point(63, 115);
            this.trcTreble.Maximum = 20;
            this.trcTreble.Name = "trcTreble";
            this.trcTreble.Size = new Size(216, 45);
            this.trcTreble.TabIndex = 18;
            this.trcTreble.Value = 20;
            this.trcTreble.Scroll += new EventHandler(this.OnTrebleValueChanged);
            this.label4.AutoSize = true;
            this.label4.Location = new Point(60, 77);
            this.label4.Name = "label4";
            this.label4.Size = new Size(10, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "-";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(274, 77);
            this.label5.Name = "label5";
            this.label5.Size = new Size(13, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "+";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(159, 102);
            this.label6.Name = "label6";
            this.label6.Size = new Size(30, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Bass";
            this.trcBass.Location = new Point(63, 74);
            this.trcBass.Maximum = 20;
            this.trcBass.Name = "trcBass";
            this.trcBass.Size = new Size(216, 45);
            this.trcBass.TabIndex = 14;
            this.trcBass.Value = 20;
            this.trcBass.Scroll += new EventHandler(this.OnBassValueChanged);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(58, 34);
            this.label3.Name = "label3";
            this.label3.Size = new Size(13, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "L";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(277, 34);
            this.label2.Name = "label2";
            this.label2.Size = new Size(15, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "R";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(152, 59);
            this.label1.Name = "label1";
            this.label1.Size = new Size(46, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Balance";
            this.trcBalance.Location = new Point(63, 31);
            this.trcBalance.Minimum = -10;
            this.trcBalance.Name = "trcBalance";
            this.trcBalance.Size = new Size(219, 45);
            this.trcBalance.TabIndex = 0;
            this.trcBalance.Scroll += new EventHandler(this.OnBalanceValueChanged);
            this.timer.Tick += new EventHandler(this.OnTimerTick);
            this.clrSettingFont.BackColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.clrSettingFont.BorderStyle = BorderStyle.FixedSingle;
            this.clrSettingFont.Location = new Point(294, 100);
            this.clrSettingFont.Name = "clrSettingFont";
            this.clrSettingFont.Size = new Size(30, 18);
            this.clrSettingFont.TabIndex = 29;
            this.clrSettingFont.TabStop = false;
            this.clrSettingFont.Click += new EventHandler(this.OnclrSettingFontClick);
            this.clrSettingBckg.BorderStyle = BorderStyle.FixedSingle;
            this.clrSettingBckg.Location = new Point(294, 75);
            this.clrSettingBckg.Name = "clrSettingBckg";
            this.clrSettingBckg.Size = new Size(30, 18);
            this.clrSettingBckg.TabIndex = 28;
            this.clrSettingBckg.TabStop = false;
            this.clrSettingBckg.Click += new EventHandler(this.OnclrSettingBckgClick);
            this.clrMainFont.BackColor = Color.Yellow;
            this.clrMainFont.BorderStyle = BorderStyle.FixedSingle;
            this.clrMainFont.Location = new Point(294, 50);
            this.clrMainFont.Name = "clrMainFont";
            this.clrMainFont.Size = new Size(30, 18);
            this.clrMainFont.TabIndex = 27;
            this.clrMainFont.TabStop = false;
            this.clrMainFont.Click += new EventHandler(this.OnclrMainFontClick);
            this.clrMainBckg.BorderStyle = BorderStyle.FixedSingle;
            this.clrMainBckg.Location = new Point(294, 25);
            this.clrMainBckg.Name = "clrMainBckg";
            this.clrMainBckg.Size = new Size(30, 18);
            this.clrMainBckg.TabIndex = 26;
            this.clrMainBckg.TabStop = false;
            this.clrMainBckg.Click += new EventHandler(this.OnclrMainBckgClick);
            this.label14.AutoSize = true;
            this.label14.Location = new Point(19, 101);
            this.label14.Name = "label14";
            this.label14.Size = new Size(66, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "Settings font";
            this.label13.AutoSize = true;
            this.label13.Location = new Point(19, 76);
            this.label13.Name = "label13";
            this.label13.Size = new Size(105, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Settings background";
            this.label12.AutoSize = true;
            this.label12.Location = new Point(19, 51);
            this.label12.Name = "label12";
            this.label12.Size = new Size(54, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Main Font";
            this.label11.AutoSize = true;
            this.label11.Location = new Point(19, 26);
            this.label11.Name = "label11";
            this.label11.Size = new Size(91, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Main Background";
            this.btnRemove.Image = (Image)Resources.remove;
            this.btnRemove.Location = new Point(279, 167);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new Size(30, 30);
            this.btnRemove.TabIndex = 23;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new EventHandler(this.OnRemoveFolder);
            this.btnAddFolder.Image = (Image)Resources.add;
            this.btnAddFolder.Location = new Point(310, 167);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new Size(30, 30);
            this.btnAddFolder.TabIndex = 13;
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new EventHandler(this.OnAddFolder);
            this.lstFolder.BackColor = Color.Black;
            this.lstFolder.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.lstFolder.FormattingEnabled = true;
            this.lstFolder.Location = new Point(3, 4);
            this.lstFolder.Name = "lstFolder";
            this.lstFolder.Size = new Size(337, 160);
            this.lstFolder.TabIndex = 0;
            this.lstFolder.SelectedIndexChanged += new EventHandler(this.OnListfolderSelectedIndexChanged);
            this.tipError.IsBalloon = true;
            this.tipError.ToolTipTitle = "Cannot read";
            this.icoNotify.Icon = (Icon)componentResourceManager.GetObject("icoNotify.Icon");
            this.icoNotify.Text = "BABALOUBA";
            this.icoNotify.Visible = true;
            this.icoNotify.MouseClick += new MouseEventHandler(this.OnicoNotifyRClick);
            this.pnlName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlName.BorderStyle = BorderStyle.Fixed3D;
            this.pnlName.Controls.Add((Control)this.lblCurrentlyPlaying);
            this.pnlName.ForeColor = Color.Yellow;
            this.pnlName.Location = new Point(5, 571);
            this.pnlName.Name = "pnlName";
            this.pnlName.Size = new Size(352, 21);
            this.pnlName.TabIndex = 15;
            this.lblCurrentlyPlaying.AutoSize = true;
            this.lblCurrentlyPlaying.Location = new Point(5, 2);
            this.lblCurrentlyPlaying.Name = "lblCurrentlyPlaying";
            this.lblCurrentlyPlaying.Size = new Size(0, 13);
            this.lblCurrentlyPlaying.TabIndex = 2;
            this.pnlSpeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.pnlSpeed.BorderStyle = BorderStyle.Fixed3D;
            this.pnlSpeed.Controls.Add((Control)this.lblSpeed);
            this.pnlSpeed.ForeColor = Color.Yellow;
            this.pnlSpeed.Location = new Point(363, 571);
            this.pnlSpeed.Name = "pnlSpeed";
            this.pnlSpeed.Size = new Size(62, 21);
            this.pnlSpeed.TabIndex = 16;
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new Point(22, 2);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new Size(18, 13);
            this.lblSpeed.TabIndex = 0;
            this.lblSpeed.Text = "1x";
            this.pnlTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.pnlTime.BorderStyle = BorderStyle.Fixed3D;
            this.pnlTime.Controls.Add((Control)this.lblTime);
            this.pnlTime.Location = new Point(431, 571);
            this.pnlTime.Name = "pnlTime";
            this.pnlTime.Size = new Size(136, 21);
            this.pnlTime.TabIndex = 17;
            this.lblTime.AutoSize = true;
            this.lblTime.ForeColor = Color.Yellow;
            this.lblTime.Location = new Point(19, 2);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new Size(60, 13);
            this.lblTime.TabIndex = 1;
            this.lblTime.Text = "--:--:--/--:--:--";
            this.tabSettings.Controls.Add((Control)this.tabFolder);
            this.tabSettings.Controls.Add((Control)this.tabTheme);
            this.tabSettings.Controls.Add((Control)this.tabMixer);
            this.tabSettings.Cursor = Cursors.Default;
            this.tabSettings.Dock = DockStyle.Fill;
            this.tabSettings.Location = new Point(3, 16);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new Size(350, 226);
            this.tabSettings.TabIndex = 27;
            this.tabFolder.BackColor = Color.Black;
            this.tabFolder.Controls.Add((Control)this.btnRemove);
            this.tabFolder.Controls.Add((Control)this.lstFolder);
            this.tabFolder.Controls.Add((Control)this.btnAddFolder);
            this.tabFolder.Location = new Point(4, 22);
            this.tabFolder.Name = "tabFolder";
            this.tabFolder.Padding = new Padding(3);
            this.tabFolder.Size = new Size(342, 200);
            this.tabFolder.TabIndex = 0;
            this.tabFolder.Text = "Folders";
            this.tabTheme.BackColor = Color.Black;
            this.tabTheme.BorderStyle = BorderStyle.FixedSingle;
            this.tabTheme.Controls.Add((Control)this.btnImportTheme);
            this.tabTheme.Controls.Add((Control)this.btnExportTheme);
            this.tabTheme.Controls.Add((Control)this.clrSettingFont);
            this.tabTheme.Controls.Add((Control)this.label12);
            this.tabTheme.Controls.Add((Control)this.label11);
            this.tabTheme.Controls.Add((Control)this.clrSettingBckg);
            this.tabTheme.Controls.Add((Control)this.label13);
            this.tabTheme.Controls.Add((Control)this.label14);
            this.tabTheme.Controls.Add((Control)this.clrMainFont);
            this.tabTheme.Controls.Add((Control)this.clrMainBckg);
            this.tabTheme.Location = new Point(4, 22);
            this.tabTheme.Name = "tabTheme";
            this.tabTheme.Padding = new Padding(3);
            this.tabTheme.Size = new Size(342, 200);
            this.tabTheme.TabIndex = 1;
            this.tabTheme.Text = "Theme";
            this.btnImportTheme.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnImportTheme.Image = (Image)Resources.import;
            this.btnImportTheme.Location = new Point(278, 166);
            this.btnImportTheme.Name = "btnImportTheme";
            this.btnImportTheme.Size = new Size(30, 30);
            this.btnImportTheme.TabIndex = 31;
            this.btnImportTheme.UseVisualStyleBackColor = true;
            this.btnImportTheme.Click += new EventHandler(this.OnImportTheme);
            this.btnExportTheme.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnExportTheme.Image = (Image)Resources.export;
            this.btnExportTheme.Location = new Point(308, 166);
            this.btnExportTheme.Name = "btnExportTheme";
            this.btnExportTheme.Size = new Size(30, 30);
            this.btnExportTheme.TabIndex = 30;
            this.btnExportTheme.UseVisualStyleBackColor = true;
            this.btnExportTheme.Click += new EventHandler(this.OnExportTheme);
            this.tabMixer.BackColor = Color.Black;
            this.tabMixer.Controls.Add((Control)this.chkFade);
            this.tabMixer.Controls.Add((Control)this.label3);
            this.tabMixer.Controls.Add((Control)this.label7);
            this.tabMixer.Controls.Add((Control)this.label2);
            this.tabMixer.Controls.Add((Control)this.label8);
            this.tabMixer.Controls.Add((Control)this.label6);
            this.tabMixer.Controls.Add((Control)this.label1);
            this.tabMixer.Controls.Add((Control)this.label9);
            this.tabMixer.Controls.Add((Control)this.label5);
            this.tabMixer.Controls.Add((Control)this.label4);
            this.tabMixer.Controls.Add((Control)this.trcBalance);
            this.tabMixer.Controls.Add((Control)this.trcBass);
            this.tabMixer.Controls.Add((Control)this.trcTreble);
            this.tabMixer.Location = new Point(4, 22);
            this.tabMixer.Name = "tabMixer";
            this.tabMixer.Padding = new Padding(3);
            this.tabMixer.Size = new Size(342, 200);
            this.tabMixer.TabIndex = 2;
            this.tabMixer.Text = "Mixer";
            this.grpSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.grpSettings.Controls.Add((Control)this.tabSettings);
            this.grpSettings.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.grpSettings.Location = new Point(576, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new Size(356, 245);
            this.grpSettings.TabIndex = 22;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            this.grpPlaylists.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            this.grpPlaylists.Controls.Add((Control)this.btnRemovePlaylist);
            this.grpPlaylists.Controls.Add((Control)this.btnAddPlaylist);
            this.grpPlaylists.Controls.Add((Control)this.tabPlaylist1);
            this.grpPlaylists.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.grpPlaylists.Location = new Point(576, 260);
            this.grpPlaylists.Name = "grpPlaylists";
            this.grpPlaylists.Size = new Size(356, 332);
            this.grpPlaylists.TabIndex = 28;
            this.grpPlaylists.TabStop = false;
            this.grpPlaylists.Text = "Playlists";
            this.btnRemovePlaylist.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnRemovePlaylist.Image = (Image)Resources.remove;
            this.btnRemovePlaylist.Location = new Point(293, 296);
            this.btnRemovePlaylist.Name = "btnRemovePlaylist";
            this.btnRemovePlaylist.Size = new Size(30, 30);
            this.btnRemovePlaylist.TabIndex = 23;
            this.btnRemovePlaylist.UseVisualStyleBackColor = true;
            this.btnRemovePlaylist.Click += new EventHandler(this.OnRemovePlaylist);
            this.btnAddPlaylist.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnAddPlaylist.Image = (Image)Resources.add;
            this.btnAddPlaylist.Location = new Point(322, 296);
            this.btnAddPlaylist.Name = "btnAddPlaylist";
            this.btnAddPlaylist.Size = new Size(30, 30);
            this.btnAddPlaylist.TabIndex = 13;
            this.btnAddPlaylist.UseVisualStyleBackColor = true;
            this.btnAddPlaylist.Click += new EventHandler(this.OnAddPlaylist);
            this.tabPlaylist1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.tabPlaylist1.Controls.Add((Control)this.tabPage1);
            this.tabPlaylist1.Location = new Point(3, 19);
            this.tabPlaylist1.Name = "tabPlaylist1";
            this.tabPlaylist1.SelectedIndex = 0;
            this.tabPlaylist1.Size = new Size(350, 274);
            this.tabPlaylist1.TabIndex = 28;
            this.tabPage1.BackColor = Color.Black;
            this.tabPage1.Controls.Add((Control)this.lstPlaylist1);
            this.tabPage1.Location = new Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(342, 248);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Playlist1";
            this.lstPlaylist1.AllowDrop = true;
            this.lstPlaylist1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lstPlaylist1.BackColor = Color.Black;
            this.lstPlaylist1.ContextMenuStrip = this.ctxPlaylists;
            this.lstPlaylist1.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.lstPlaylist1.FormattingEnabled = true;
            this.lstPlaylist1.Location = new Point(3, 4);
            this.lstPlaylist1.Name = "lstPlaylist1";
            this.lstPlaylist1.Size = new Size(336, 238);
            this.lstPlaylist1.TabIndex = 0;
            this.ctxPlaylists.Items.AddRange(new ToolStripItem[5]
            {
        (ToolStripItem) this.itmNew,
        (ToolStripItem) this.itmRename,
        (ToolStripItem) this.itmDelPlaylist,
        (ToolStripItem) this.itmAdd,
        (ToolStripItem) this.itmDelSong
            });
            this.ctxPlaylists.Name = "ctxPlaylists";
            this.ctxPlaylists.Size = new Size(158, 114);
            this.ctxPlaylists.Opening += new CancelEventHandler(this.OnCtxPlaylistsOpening);
            this.itmNew.Name = "itmNew";
            this.itmNew.Size = new Size(157, 22);
            this.itmNew.Text = "New playlist";
            this.itmNew.Click += new EventHandler(this.OnAddPlaylist);
            this.itmRename.Name = "itmRename";
            this.itmRename.Size = new Size(157, 22);
            this.itmRename.Text = "Rename playlist";
            this.itmRename.Click += new EventHandler(this.OnRenamePlaylist);
            this.itmDelPlaylist.Name = "itmDelPlaylist";
            this.itmDelPlaylist.Size = new Size(157, 22);
            this.itmDelPlaylist.Text = "Delete playlist";
            this.itmDelPlaylist.Click += new EventHandler(this.OnRemovePlaylist);
            this.itmAdd.Name = "itmAdd";
            this.itmAdd.Size = new Size(157, 22);
            this.itmAdd.Text = "Add song";
            this.itmDelSong.Name = "itmDelSong";
            this.itmDelSong.Size = new Size(157, 22);
            this.itmDelSong.Text = "Delete song";
            this.itmDelSong.Click += new EventHandler(this.OnDeleteSong);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(937, 595);
            this.Controls.Add((Control)this.pnlTime);
            this.Controls.Add((Control)this.pnlSpeed);
            this.Controls.Add((Control)this.pnlName);
            this.Controls.Add((Control)this.grpPlaylist);
            this.Controls.Add((Control)this.grpSettings);
            this.Controls.Add((Control)this.grpPlaylists);
            this.ForeColor = Color.FromArgb((int)byte.MaxValue, 128, 0);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.KeyPreview = true;
            this.MinimumSize = new Size(953, 633);
            this.Name = nameof(Babalouba);
            this.Opacity = 0.95;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "BABALOUBA";
            this.TransparencyKey = Color.FromArgb(224, 224, 224);
            this.Load += new EventHandler(this.OnBabaloubaLoad);
            this.Resize += new EventHandler(this.OnResize);
            this.grpPlaylist.ResumeLayout(false);
            this.grpPlaylist.PerformLayout();
            this.trcVolume.EndInit();
            ((ISupportInitialize)this.btnSpeedUp).EndInit();
            ((ISupportInitialize)this.btnSlowDown).EndInit();
            this.ctxPlaylist.ResumeLayout(false);
            this.trcProgress.EndInit();
            this.trcTreble.EndInit();
            this.trcBass.EndInit();
            this.trcBalance.EndInit();
            ((ISupportInitialize)this.clrSettingFont).EndInit();
            ((ISupportInitialize)this.clrSettingBckg).EndInit();
            ((ISupportInitialize)this.clrMainFont).EndInit();
            ((ISupportInitialize)this.clrMainBckg).EndInit();
            this.pnlName.ResumeLayout(false);
            this.pnlName.PerformLayout();
            this.pnlSpeed.ResumeLayout(false);
            this.pnlSpeed.PerformLayout();
            this.pnlTime.ResumeLayout(false);
            this.pnlTime.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabFolder.ResumeLayout(false);
            this.tabTheme.ResumeLayout(false);
            this.tabTheme.PerformLayout();
            this.tabMixer.ResumeLayout(false);
            this.tabMixer.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpPlaylists.ResumeLayout(false);
            this.tabPlaylist1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ctxPlaylists.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private enum MediaTypes
        {
            Audio,
            Video,
            Unknown,
        }
    }
}
