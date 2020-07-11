using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PlayerAPI
{
    public class Player : Form
    {
        private const int MM_MCINOTIFY = 953;
        private string Pcommand;
        private string FName;
        private string Phandle;
        public bool Opened;
        public bool Playing;
        public bool Paused;
        public bool Looping;
        public bool MutedAll;
        public bool MutedLeft;
        public bool MutedRight;
        private int Err;
        private int aVolume;
        private int bVolume;
        private int lVolume;
        private int pSpeed;
        private int rVolume;
        private int tVolume;
        private int VolBalance;
        private ulong Lng;

        [DllImport("winmm.dll")]
        private static extern int mciSendString(
          string command,
          StringBuilder buffer,
          int bufferSize,
          IntPtr hwndCallback);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 1:
                    this.OnOtherEvent(new Player.OtherEventArgs(MCINotify.Success));
                    break;
                case 2:
                    this.OnOtherEvent(new Player.OtherEventArgs(MCINotify.Superseded));
                    break;
                case 4:
                    this.OnOtherEvent(new Player.OtherEventArgs(MCINotify.Aborted));
                    break;
                case 8:
                    this.OnOtherEvent(new Player.OtherEventArgs(MCINotify.Failure));
                    break;
                case 953:
                    if (m.WParam.ToInt32() == 1)
                    {
                        this.Stop();
                        this.OnSongEnd(new Player.SongEndEventArgs());
                        break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        public Player(Player _player)
        {
            this.Opened = _player.Opened;
            this.Pcommand = _player.Pcommand;
            this.FName = _player.FName;
            this.Playing = _player.Playing;
            this.Paused = _player.Paused;
            this.Looping = _player.Looping;
            this.MutedAll = this.MutedLeft = this.MutedRight = _player.MutedAll;
            this.rVolume = this.lVolume = this.aVolume = this.tVolume = this.bVolume = _player.rVolume;
            this.pSpeed = _player.pSpeed;
            this.Lng = _player.Lng;
            this.VolBalance = _player.VolBalance;
            this.Err = _player.Err;
            this.Phandle = _player.Phandle;
        }

        public Player()
        {
            this.Opened = false;
            this.Pcommand = string.Empty;
            this.FName = string.Empty;
            this.Playing = false;
            this.Paused = false;
            this.Looping = false;
            this.MutedAll = this.MutedLeft = this.MutedRight = false;
            this.rVolume = this.lVolume = this.aVolume = this.tVolume = this.bVolume = 1000;
            this.pSpeed = 1000;
            this.Lng = 0UL;
            this.VolBalance = 0;
            this.Err = 0;
            this.Phandle = "MP3Player";
        }

        public Player(string handle)
        {
            this.Opened = false;
            this.Pcommand = string.Empty;
            this.FName = string.Empty;
            this.Playing = false;
            this.Paused = false;
            this.Looping = false;
            this.MutedAll = this.MutedLeft = this.MutedRight = false;
            this.rVolume = this.lVolume = this.aVolume = this.tVolume = this.bVolume = 1000;
            this.pSpeed = 1000;
            this.Lng = 0UL;
            this.VolBalance = 0;
            this.Err = 0;
            this.Phandle = handle;
        }

        public int Balance
        {
            get
            {
                return this.VolBalance;
            }
            set
            {
                if (!this.Opened || value < -1000 || value > 1000)
                    return;
                this.VolBalance = value;
                double num = Convert.ToDouble(this.aVolume) / 1000.0;
                if (value < 0)
                {
                    this.Pcommand = string.Format("setaudio {0} left volume to {1:#}", (object)this.Phandle, (object)this.aVolume);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                        this.OnError(new Player.ErrorEventArgs(this.Err));
                    this.Pcommand = string.Format("setaudio {0} right volume to {1:#}", (object)this.Phandle, (object)((double)(1000 + value) * num));
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
                else
                {
                    this.Pcommand = string.Format("setaudio {0} right volume to {1:#}", (object)this.Phandle, (object)this.aVolume);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                        this.OnError(new Player.ErrorEventArgs(this.Err));
                    this.Pcommand = string.Format("setaudio {0} left volume to {1:#}", (object)this.Phandle, (object)((double)(1000 - value) * num));
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
            }
        }

        public bool MuteAll
        {
            get
            {
                return this.MutedAll;
            }
            set
            {
                this.MutedAll = value;
                if (this.MutedAll)
                {
                    this.Pcommand = string.Format("setaudio {0} off", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
                else
                {
                    this.Pcommand = string.Format("setaudio {0} on", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
            }
        }

        public bool MuteLeft
        {
            get
            {
                return this.MutedLeft;
            }
            set
            {
                this.MutedLeft = value;
                if (this.MutedLeft)
                {
                    this.Pcommand = string.Format("setaudio {0} left off", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
                else
                {
                    this.Pcommand = string.Format("setaudio {0} left on", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
            }
        }

        public bool MuteRight
        {
            get
            {
                return this.MutedRight;
            }
            set
            {
                this.MutedRight = value;
                if (this.MutedRight)
                {
                    this.Pcommand = string.Format("setaudio {0} right off", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
                else
                {
                    this.Pcommand = string.Format("setaudio {0} right on", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
            }
        }

        public int VolumeAll
        {
            get
            {
                return this.aVolume;
            }
            set
            {
                if (!this.Opened || value < 0 || value > 1000)
                    return;
                this.aVolume = value;
                this.Pcommand = string.Format("setaudio {0} volume to {1}", (object)this.Phandle, (object)this.aVolume);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public int VolumeBass
        {
            get
            {
                return this.bVolume;
            }
            set
            {
                if (!this.Opened || value < 0 || value > 1000)
                    return;
                this.bVolume = value;
                this.Pcommand = string.Format("setaudio {0} bass to {1}", (object)this.Phandle, (object)this.bVolume);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public int VolumeLeft
        {
            get
            {
                return this.lVolume;
            }
            set
            {
                if (!this.Opened || value < 0 || value > 1000)
                    return;
                this.lVolume = value;
                this.Pcommand = string.Format("setaudio {0} left volume to {1}", (object)this.Phandle, (object)this.lVolume);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public int VolumeRight
        {
            get
            {
                return this.rVolume;
            }
            set
            {
                if (!this.Opened || value < 0 || value > 1000)
                    return;
                this.rVolume = value;
                this.Pcommand = string.Format("setaudio {0} right volume to {1}", (object)this.Phandle, (object)this.rVolume);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public int VolumeTreble
        {
            get
            {
                return this.tVolume;
            }
            set
            {
                if (!this.Opened || value < 0 || value > 1000)
                    return;
                this.tVolume = value;
                this.Pcommand = string.Format("setaudio {0} treble to {1}", (object)this.Phandle, (object)this.tVolume);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public ulong AudioLength
        {
            get
            {
                if (this.Opened)
                    return this.Lng;
                return 0;
            }
        }

        public ulong CurrentPosition
        {
            get
            {
                if (!this.Opened || !this.Playing)
                    return 0;
                StringBuilder buffer = new StringBuilder(128);
                this.Pcommand = string.Format("status {0} position", (object)this.Phandle);
                if ((this.Err = Player.mciSendString(this.Pcommand, buffer, 128, IntPtr.Zero)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                ulong result = 0;
                if (!ulong.TryParse(buffer.ToString(), out result))
                    return 0;
                return result;
            }
        }

        public string FileName
        {
            get
            {
                return this.FName;
            }
        }

        public string PHandle
        {
            get
            {
                return this.Phandle;
            }
        }

        public bool IsOpened
        {
            get
            {
                return this.Opened;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return this.Playing;
            }
        }

        public bool IsPaused
        {
            get
            {
                return this.Paused;
            }
        }

        public bool IsLooping
        {
            get
            {
                return this.Looping;
            }
            set
            {
                this.Looping = value;
                if (!this.Opened || !this.Playing || this.Paused)
                    return;
                if (this.Looping)
                {
                    this.Pcommand = string.Format("play {0} notify repeat", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
                else
                {
                    this.Pcommand = string.Format("play {0} notify", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) == 0)
                        return;
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                }
            }
        }

        public int Speed
        {
            get
            {
                return this.pSpeed;
            }
            set
            {
                if (value < 3 || value > 4353)
                    return;
                this.pSpeed = value;
                this.Pcommand = string.Format("set {0} speed {1}", (object)this.Phandle, (object)this.pSpeed);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        private bool CalculateLength()
        {
            try
            {
                StringBuilder buffer = new StringBuilder(128);
                this.Pcommand = "status " + this.Phandle + " length";
                if ((this.Err = Player.mciSendString(this.Pcommand, buffer, 128, IntPtr.Zero)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                this.Lng = Convert.ToUInt64(buffer.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CloseMedia()
        {
            if (!this.Opened)
                return;
            this.Pcommand = string.Format("close {0}", (object)this.Phandle);
            if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                this.OnError(new Player.ErrorEventArgs(this.Err));
            this.FName = string.Empty;
            this.Opened = false;
            this.Playing = false;
            this.Paused = false;
            this.OnCloseFile(new Player.CloseFileEventArgs());
        }

        public bool Open(string sFileName)
        {
            if (!this.Opened)
            {
                this.Pcommand = string.Format("open \"" + sFileName + "\" type mpegvideo alias {0}", (object)this.Phandle);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                {
                    this.Pcommand = string.Format("open \"" + sFileName + "\" alias {0}", (object)this.Phandle);
                    if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                    {

                        this.OnError(new Player.ErrorEventArgs(this.Err));
                        return false;
                    }
                }
                this.FName = sFileName;
                this.Opened = true;
                this.Playing = false;
                this.Paused = false;
                this.Pcommand = string.Format("set {0} time format milliseconds", (object)this.Phandle);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                this.Pcommand = string.Format("set {0} seek exactly on", (object)this.Phandle);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                if (!this.CalculateLength())
                    return false;
                this.OnOpenFile(new Player.OpenFileEventArgs(sFileName));
                return true;
            }
            this.CloseMedia();
            return this.Open(sFileName);
        }

        public void Pause()
        {
            if (!this.Opened || this.Paused)
                return;
            this.Paused = true;
            this.Pcommand = string.Format("pause {0}", (object)this.Phandle);
            if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                this.OnError(new Player.ErrorEventArgs(this.Err));
            this.OnPauseFile(new Player.PauseFileEventArgs());
        }

        public void Play()
        {
            if (!this.Playing)
            {
                this.Playing = true;
                this.Pcommand = string.Format("play {0}{1} notify", (object)this.Phandle, this.Looping ? (object)" repeat" : (object)string.Empty);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                this.OnPlayFile(new Player.PlayFileEventArgs());
            }
            else if (this.Paused)
            {
                this.Paused = false;
                this.Pcommand = string.Format("play {0}{1} notify", (object)this.Phandle, this.Looping ? (object)" repeat" : (object)string.Empty);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                this.OnPlayFile(new Player.PlayFileEventArgs());
            }
            this.Pcommand = string.Format("set {0} speed {1}", (object)this.Phandle, (object)this.Speed);
            if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) == 0)
                return;
            this.OnError(new Player.ErrorEventArgs(this.Err));
        }

        public void Seek(ulong milliseconds)
        {
            if (!this.Opened || milliseconds > this.Lng || !this.Playing)
                return;
            if (this.Paused)
            {
                this.Pcommand = string.Format("seek {0} to {1}", (object)this.Phandle, (object)milliseconds);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
            else
            {
                this.Pcommand = string.Format("seek {0} to {1}", (object)this.Phandle, (object)milliseconds);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) != 0)
                    this.OnError(new Player.ErrorEventArgs(this.Err));
                this.Pcommand = string.Format("play {0}{1} notify", (object)this.Phandle, this.Looping ? (object)" repeat" : (object)string.Empty);
                if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, this.Handle)) == 0)
                    return;
                this.OnError(new Player.ErrorEventArgs(this.Err));
            }
        }

        public void Stop()
        {
            if (!this.Opened || !this.Playing)
                return;
            this.Playing = false;
            this.Paused = false;
            this.Pcommand = string.Format("seek {0} to start", (object)this.Phandle);
            if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                this.OnError(new Player.ErrorEventArgs(this.Err));
            this.Pcommand = string.Format("stop {0}", (object)this.Phandle);
            if ((this.Err = Player.mciSendString(this.Pcommand, (StringBuilder)null, 0, IntPtr.Zero)) != 0)
                this.OnError(new Player.ErrorEventArgs(this.Err));
            this.OnStopFile(new Player.StopFileEventArgs());
        }

        public event Player.OpenFileEventHandler OpenFile;

        public event Player.PlayFileEventHandler PlayFile;

        public event Player.PauseFileEventHandler PauseFile;

        public event Player.StopFileEventHandler StopFile;

        public event Player.CloseFileEventHandler CloseFile;

        public event Player.SongMixEventHandler Fade;

        public event Player.ErrorEventHandler Error;

        public event Player.SongEndEventHandler SongEnd;

        public event Player.OtherEventHandler OtherEvent;

        protected virtual void OnOpenFile(Player.OpenFileEventArgs oea)
        {
            if (this.OpenFile == null)
                return;
            this.OpenFile((object)this, oea);
        }

        protected virtual void OnPlayFile(Player.PlayFileEventArgs pea)
        {
            if (this.PlayFile == null)
                return;
            this.PlayFile((object)this, pea);
        }

        protected virtual void OnPauseFile(Player.PauseFileEventArgs paea)
        {
            if (this.PauseFile == null)
                return;
            this.PauseFile((object)this, paea);
        }

        protected virtual void OnStopFile(Player.StopFileEventArgs sea)
        {
            if (this.StopFile == null)
                return;
            this.StopFile((object)this, sea);
        }

        protected virtual void OnCloseFile(Player.CloseFileEventArgs cea)
        {
            if (this.CloseFile == null)
                return;
            this.CloseFile((object)this, cea);
        }

        protected virtual void OnError(Player.ErrorEventArgs eea)
        {
            if (this.Error == null)
                return;
            this.Error((object)this, eea);
        }

        protected virtual void OnFade(Player.SongMixEventArgs fad)
        {
            if (this.Fade == null)
                return;
            this.Fade((object)this, fad);
        }

        protected virtual void OnSongEnd(Player.SongEndEventArgs seea)
        {
            if (this.SongEnd == null)
                return;
            this.SongEnd((object)this, seea);
        }

        protected virtual void OnOtherEvent(Player.OtherEventArgs oea)
        {
            if (this.OtherEvent == null)
                return;
            this.OtherEvent((object)this, oea);
        }

        public class SongMixEventArgs : EventArgs
        {
        }

        public class OpenFileEventArgs : EventArgs
        {
            public readonly string FileName;

            public OpenFileEventArgs(string filename)
            {
                this.FileName = filename;
            }
        }

        public class PlayFileEventArgs : EventArgs
        {
        }

        public class PauseFileEventArgs : EventArgs
        {
        }

        public class StopFileEventArgs : EventArgs
        {
        }

        public class CloseFileEventArgs : EventArgs
        {
        }

        public class ErrorEventArgs : EventArgs
        {
            public readonly int ErrorCode;
            public readonly string ErrorString;

            [DllImport("winmm.dll")]
            private static extern bool mciGetErrorString(
              int errorCode,
              StringBuilder errorText,
              int errorTextSize);

            public ErrorEventArgs(int ErrorCode)
            {
                this.ErrorCode = ErrorCode;
                StringBuilder errorText = new StringBuilder(256);
                Player.ErrorEventArgs.mciGetErrorString(ErrorCode, errorText, 256);
                this.ErrorString = errorText.ToString();
            }
        }

        public class SongEndEventArgs : EventArgs
        {
        }

        public class OtherEventArgs : EventArgs
        {
            public readonly MCINotify Notification;

            public OtherEventArgs(MCINotify Notification)
            {
                this.Notification = Notification;
            }
        }

        public delegate void SongMixEventHandler(object sender, Player.SongMixEventArgs fad);

        public delegate void OpenFileEventHandler(object sender, Player.OpenFileEventArgs oea);

        public delegate void PlayFileEventHandler(object sender, Player.PlayFileEventArgs pea);

        public delegate void PauseFileEventHandler(object sender, Player.PauseFileEventArgs paea);

        public delegate void StopFileEventHandler(object sender, Player.StopFileEventArgs sea);

        public delegate void CloseFileEventHandler(object sender, Player.CloseFileEventArgs cea);

        public delegate void ErrorEventHandler(object sender, Player.ErrorEventArgs eea);

        public delegate void SongEndEventHandler(object sender, Player.SongEndEventArgs seea);

        public delegate void OtherEventHandler(object sender, Player.OtherEventArgs oea);
    }
}
