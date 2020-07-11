using System;
using System.Collections.Generic;

namespace Babalouba
{
    [Serializable]
    public class PlayList
    {
        private List<Song> songs = new List<Song>();
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public List<Song> Songs
        {
            get
            {
                return this.songs;
            }
            set
            {
                this.songs = value;
            }
        }
    }
}
