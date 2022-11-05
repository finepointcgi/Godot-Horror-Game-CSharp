using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public static class AudioManager 
    {
        public static int lastPlayedAudio;
        public static AudioStreamPlayer playerAudioStream;
        public static Random random = new Random();
        public static AudioStream getNonLastAudioStream(AudioStream audioStreams)
        {
            //lastPlayedAudio = getUniqueRandomValue(0, audioStreams.Count);
            return audioStreams;
        }

        public static int getUniqueRandomValue(int startValue, int maxValue)
        {
            int value = random.Next(0, maxValue);
            if(value == lastPlayedAudio)
            {
                value = getUniqueRandomValue(startValue, maxValue);
            }
            return value;
        }
    }
}
