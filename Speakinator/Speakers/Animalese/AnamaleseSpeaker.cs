using Speakinator.Speakers;
using Speakinator.Speakers.Animalese;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Speakinator.Speakers.Animalese
{
    public class AnimaleseSpeaker : Speaker
    {
        private byte[] _wavData;
        private AnimaleseOptions _options;
        public AnimaleseSpeaker()
        {
            _wavData = LoadWav(@".\Resources\animalese.wav");
        }

        public void Speak(string somethingToSay, ISpeakerOptions speakerOptions)
        {
            _options = (AnimaleseOptions)speakerOptions;
            var processedText = somethingToSay.ToUpper();

            if (_options.Shorten)
            {
                Regex.Replace(processedText, "/[^a - z]/gi", " ");

                var shortenedPhrase = new List<string>();
                var splitWords = processedText.Split(' ').ToList();
                foreach (var word in splitWords)
                {
                    var shortWord = ShortenWord(word);
                    shortenedPhrase.Add(shortWord);
                }
                processedText = string.Join(" ", shortenedPhrase);
            }

            var sample_freq = 44100;
            var library_letter_secs = 0.15;
            var library_samples_per_letter = (int)Math.Floor(library_letter_secs * sample_freq);
            var output_letter_secs = 0.075;
            var output_samples_per_letter = (int)Math.Floor(output_letter_secs * sample_freq);

            var arraySize = processedText.Length * output_samples_per_letter;
            var data = new byte[arraySize];

            for (var c_index = 0; c_index < processedText.Length; c_index++)
            {
                var c = processedText[c_index];
                if (char.IsLetter(c))
                {
                    var library_letter_start =
                        library_samples_per_letter * (c - 'A');

                    for (var i = 0; i < output_samples_per_letter; i++)
                    {
                        data[c_index * output_samples_per_letter + i] =
                            _wavData[44 + library_letter_start + (int)Math.Floor(i * _options.Pitch)];
                    }
                }
                else
                { // non pronouncable character or space
                    for (var i = 0; i < output_samples_per_letter; i++)
                    {
                        data[c_index * output_samples_per_letter + i] = 127;
                    }
                }
            }

            using (var stream = new MemoryStream())
            {

                CreateWav(stream, data);
                PlayWav(stream);
            }
        }

        private Stream CreateWav(Stream stream, byte[] data)
        {
            stream.Write(_wavData, 0, 44);
            var dataSize = BitConverter.GetBytes(data.Length);

            stream.Seek(40, SeekOrigin.Begin);
            stream.Write(dataSize, 0, 4);

            stream.Write(data, 0, data.Length);

            stream.Seek(4, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(stream.Length - 8), 0, 4);

            stream.Seek(0, SeekOrigin.Begin);

            //stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private string ShortenWord(string str)
        {

            if (string.IsNullOrWhiteSpace(str)) return str;

            var tempStr = str.ToCharArray();
            return $"{tempStr[0]} + {tempStr[tempStr.Length - 1]}";
        }

        private byte[] LoadWav(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public void Speak(string text)
        {
            throw new NotImplementedException();
        }

        public void Speak(string text, object options)
        {
            throw new NotImplementedException();
        }
    }
}