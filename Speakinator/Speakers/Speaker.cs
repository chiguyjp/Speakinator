using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Media;
using System.IO;

namespace Speakinator
{
    public interface ISpeaker
    {
        void Speak(string text);
        void Speak(string text, object options);
    }

    public class Speaker
    {
        protected SpeechSynthesizer Synth;
        protected SoundPlayer SoundPlayer;

        public Speaker()
        {
            Synth = new SpeechSynthesizer();
            Synth.SetOutputToDefaultAudioDevice();
            SoundPlayer = new SoundPlayer();
        }

        public void Speak(string input)
        {
            Synth.SpeakAsync(input);
        }

        public void PlayWav(Stream stream)
        {
            SoundPlayer.Stream = stream;
            try
            {
                SoundPlayer.Play();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
