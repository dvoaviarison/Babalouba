using System;

namespace Babalouba
{
    [Serializable]
    public class Theme
    {
        private ThemeColor mainBackColor;
        private ThemeColor mainFontColor;
        private ThemeColor settingsBackColor;
        private ThemeColor settingsFontColor;

        public Theme()
        {
            this.mainBackColor = new ThemeColor();
            this.mainFontColor = new ThemeColor();
            this.settingsBackColor = new ThemeColor();
            this.settingsFontColor = new ThemeColor();
        }

        public ThemeColor MainBackColor
        {
            get
            {
                return this.mainBackColor;
            }
            set
            {
                this.mainBackColor = value;
            }
        }

        public ThemeColor MainFontColor
        {
            get
            {
                return this.mainFontColor;
            }
            set
            {
                this.mainFontColor = value;
            }
        }

        public ThemeColor SettingsBackColor
        {
            get
            {
                return this.settingsBackColor;
            }
            set
            {
                this.settingsBackColor = value;
            }
        }

        public ThemeColor SettingsFontColor
        {
            get
            {
                return this.settingsFontColor;
            }
            set
            {
                this.settingsFontColor = value;
            }
        }
    }
}
