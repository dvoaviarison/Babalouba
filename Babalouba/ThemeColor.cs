using System;

namespace Babalouba
{
    [Serializable]
    public class ThemeColor
    {
        private int a;
        private int r;
        private int g;
        private int b;

        public int A
        {
            get
            {
                return this.a;
            }
            set
            {
                this.a = value;
            }
        }

        public int R
        {
            get
            {
                return this.r;
            }
            set
            {
                this.r = value;
            }
        }

        public int G
        {
            get
            {
                return this.g;
            }
            set
            {
                this.g = value;
            }
        }

        public int B
        {
            get
            {
                return this.b;
            }
            set
            {
                this.b = value;
            }
        }
    }
}
