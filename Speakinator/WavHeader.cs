using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakinator
{

    public class ExtraParams
    {
        public short ExtraParamsSize;
        public byte[] ExtraParamsData;
    }

    public class WavHeader
    {
        private ASCIIEncoding _asciiEncoder = new ASCIIEncoding();
        private byte[] _chunkId = new byte[4];
        private byte[] _chunkSize = new byte[4];
        private byte[] _format = new byte[4];
        private byte[] _subchunk1Id = new byte[4];
        private byte[] _subchunk1Size = new byte[4];
        private byte[] _audioFormat = new byte[2];
        private byte[] _numChannels = new byte[2];
        private byte[] _sampleRate = new byte[4];
        private byte[] _byteRate = new byte[4];
        private byte[] _blockAlign = new byte[2];
        private byte[] _bitsPerSample = new byte[2];
        private byte[] _extraParamSize = new byte[2];
        private byte[] _subchunk2Id = new byte[4];
        private byte[] _subchunk2Size = new byte[4];

        /// <summary>
        /// ChunkID Contains the letters "RIFF" in ASCII form
        /// (0x52494646 big-endian form).
        /// </summary>
        public string ChunkId
        {
            get
            {
                return _asciiEncoder.GetString(_chunkId, 0, _chunkId.Length);
            }
            set
            {
                _chunkId = _asciiEncoder.GetBytes(value);
            }
        }

        /// <summary>
        /// 36 + SubChunk2Size, or more precisely:
        /// 4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
        /// This is the size of the rest of the chunk
        /// following this number.This is the size of the
        /// entire file in bytes minus 8 bytes for the
        /// two fields not included in this count:
        /// ChunkID and ChunkSize.
        /// </summary>
        public int ChunkSize
        {
            get
            {
                return BitConverter.ToInt32(_chunkSize, 0);
            }
            set
            {
                _chunkSize = BitConverter.GetBytes(value);
            }
        }

        /*The "WAVE" format consists of two subchunks: "fmt " and "data":
          The "fmt " subchunk describes the sound data's format:*/

        /// <summary>
        /// Contains the letters "WAVE"
        /// (0x57415645 big-endian form).
        /// </summary>
        public string Format
        {
            get
            {
                return _asciiEncoder.GetString(_format, 0, _format.Length);
            }
            set
            {
                _format = _asciiEncoder.GetBytes(value);
            }
        }

        /// <summary>
        /// Contains the letters "fmt "
        /// (0x666d7420 big-endian form).
        /// </summary>
        public string SubChunk1Id
        {
            get
            {
                return _asciiEncoder.GetString(_subchunk1Id, 0, _subchunk1Id.Length);
            }
            set
            {
                _subchunk1Id = _asciiEncoder.GetBytes(value);
            }
        }

        /// <summary>
        /// 16 for PCM.  This is the size of the
        /// rest of the Subchunk which follows this number.
        /// </summary>
        public int SubChunk1Size
        {
            get
            {
                return BitConverter.ToInt32(_subchunk1Size, 0);
            }
            set
            {
                _subchunk1Size = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// PCM = 1 (i.e. Linear quantization)
        /// Values other than 1 indicate some
        /// form of compression.
        /// </summary>
        public short AudioFormat
        {
            get
            {
                return BitConverter.ToInt16(_audioFormat, 0);
            }
            set
            {
                _audioFormat = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// Mono = 1, Stereo = 2, etc.
        /// </summary>
        public short NumChannels
        {
            get
            {
                return BitConverter.ToInt16(_numChannels, 0);
            }
            set
            {
                _numChannels = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// 8000, 44100, etc.
        /// </summary>
        public int SampleRate
        {
            get
            {
                return BitConverter.ToInt32(_sampleRate, 0);
            }
            set
            {
                _sampleRate = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// SampleRate * NumChannels * BitsPerSample/8
        /// </summary>
        public int ByteRate
        {
            get
            {
                return BitConverter.ToInt32(_byteRate, 0);
            }
            set
            {
                _byteRate = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// NumChannels * BitsPerSample/8
        /// The number of bytes for one sample including
        /// all channels.I wonder what happens when
        /// this number isn't an integer?
        /// </summary>
        public short BlockAlign
        {
            get
            {
                return BitConverter.ToInt16(_blockAlign, 0);
            }
            set
            {
                _blockAlign = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// 8 bits = 8, 16 bits = 16, etc.
        /// </summary>
        public short BitsPerSample
        {
            get
            {
                return BitConverter.ToInt16(_bitsPerSample, 0);
            }
            set
            {
                _bitsPerSample = BitConverter.GetBytes(value);
            }
        }

        /// <summary>
        /// if PCM, then doesn't exist
        /// </summary>
        public short ExtraParamSize
        {
            get
            {
                return BitConverter.ToInt16(_extraParamSize, 0);
            }
            set
            {
                _extraParamSize = BitConverter.GetBytes(value);
            }
        }

        //Add extra data
        /*The "data" subchunk contains the size of the data and the actual sound:*/

        /// <summary>
        /// Contains the letters "data"
        /// (0x64617461 big-endian form).
        /// </summary>
        public string Subchunk2Id
        {
            get
            {
                return _asciiEncoder.GetString(_subchunk2Id, 0, _subchunk2Id.Length);
            }
            set
            {
                _subchunk2Id = _asciiEncoder.GetBytes(value);
            }
        }

        /// <summary>
        /// NumSamples * NumChannels * BitsPerSample/8
        /// This is the number of bytes in the data.
        /// You can also think of this as the size
        /// of the read of the subchunk following this
        /// number.
        /// </summary>
        public int Subchunk2Size
        {
            get
            {
                return BitConverter.ToInt32(_subchunk2Size, 0);
            }
            set
            {
                _subchunk2Size = BitConverter.GetBytes(value);
            }
        }

        public WavHeader(Stream waveStream)
        {
            
            //Ensure stream position is 0
            waveStream.Seek(0, SeekOrigin.Begin);

            waveStream.Read(_chunkId, 0, 4);
            waveStream.Read(_chunkSize, 0, 4);
            waveStream.Read(_format, 0, 4);
            waveStream.Read(_subchunk1Id, 0, 4);
            waveStream.Read(_subchunk1Size, 0, 4);
            waveStream.Read(_audioFormat, 0, 2);
            waveStream.Read(_numChannels, 0, 2);
            waveStream.Read(_sampleRate, 0, 4);
            waveStream.Read(_byteRate, 0, 4);
            waveStream.Read(_blockAlign, 0, 2);
            waveStream.Read(_bitsPerSample, 0, 2);
            waveStream.Read(_subchunk2Id, 0, 4);
            waveStream.Read(_subchunk2Size, 0, 4);
        }

        public WavHeader(byte[] waveArray)
        {
            Array.Copy(waveArray, _chunkId, 4);
            Array.Copy(waveArray, 4, _chunkSize, 0, 4);
            Array.Copy(waveArray, 8, _format, 0, 4);
            Array.Copy(waveArray, 12, _subchunk1Id, 0, 4);
            Array.Copy(waveArray, 16, _subchunk1Size, 0, 4);
            Array.Copy(waveArray, 20, _audioFormat, 0, 2);
            Array.Copy(waveArray, 22, _numChannels, 0, 2);
            Array.Copy(waveArray, 24, _sampleRate, 0, 4);
            Array.Copy(waveArray, 28, _byteRate, 0, 4);
            Array.Copy(waveArray, 32, _blockAlign, 0, 2);
            Array.Copy(waveArray, 34, _bitsPerSample, 0, 2);
            Array.Copy(waveArray, 36, _subchunk2Id, 0, 4);
            Array.Copy(waveArray, 40, _subchunk2Size, 0, 4);
        }
    }
}