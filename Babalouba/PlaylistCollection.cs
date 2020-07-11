using System;
using System.Collections.Generic;

namespace Babalouba
{
    [Serializable]
    public class PlaylistCollection
    {
        private List<PlayList> playLists = new List<PlayList>();

        public List<PlayList> PlayLists
        {
            get
            {
                return this.playLists;
            }
            set
            {
                this.playLists = value;
            }
        }
    }
}
