using Speakinator.Speakers.Animalese;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Speakinator
{
    public partial class Form1 : Form
    {
        private AnimaleseSpeaker _animalSpeaker;
        public Form1()
        {
            InitializeComponent();
            _animalSpeaker = new AnimaleseSpeaker();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _animalSpeaker.Speak(textBox1.Text, new AnimaleseOptions
            {
                Pitch = 1,
                Shorten = checkBox1.Checked
            });
        }
    }
}
