using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Destiny
{
    public class AudioFile : IEquatable<AudioFile>
    {
        public String FileName { get; set; }
        public String Cuename { get; set; }
        public bool RequiresInstance { get; set; }
        public bool AutoStart { get; set; }
        public bool isLooped { get; set; }
        [ContentSerializerIgnore]
        public SoundEffect soundEffect { get; set; }
        [ContentSerializerIgnore]
        public SoundEffectInstance soundEffectInstance { get; set; }

        public bool Equals(AudioFile audioCue)
        {
            if (this.Cuename == audioCue.Cuename)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class AudioConfiguration : List<AudioFile> { }
}
