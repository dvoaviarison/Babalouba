using System;

namespace Babalouba
{
    [Serializable]
    public class Configuration
    {
        private string defaultTheme;

        public string DefaultTheme
        {
            get
            {
                return this.defaultTheme;
            }
            set
            {
                this.defaultTheme = value;
            }
        }
    }
}
