using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using Microsoft.Win32;
using System.IO;

namespace TextToSpeech
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechSynthesizer sp = new SpeechSynthesizer();
        SpeechRecognitionEngine spe = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-IN"));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbMale.IsChecked == true)
                    sp.SelectVoiceByHints(VoiceGender.Male);
                else
                    sp.SelectVoiceByHints(VoiceGender.Female);

                sp.SpeakAsync(textBox.Text);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            sp.Pause();
        }

        private void buttonResume_Click(object sender, RoutedEventArgs e)
        {
            sp.Resume();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog savedlg = new SaveFileDialog();
                savedlg.Filter = "wav files (*.wav)|*.wav";
                savedlg.Title = "Save to wav";
                savedlg.FilterIndex =1;

                Nullable<bool> result = savedlg.ShowDialog();

                if(result == true)
                {
                    FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write);
                    sp.SetOutputToWaveStream(fs);
                    sp.Speak(textBox.Text);
                    fs.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSaveTXT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog savedlg = new SaveFileDialog();
                savedlg.Filter = "txt files (*.txt)|*.txt";
                savedlg.Title = "Save as TXT";
                savedlg.FilterIndex = 1;

                Nullable<bool> result = savedlg.ShowDialog();

                if (result == true)
                {
                    FileStream fs = new FileStream(savedlg.FileName, FileMode.Create, FileAccess.Write);
                    byte[] info = new UTF8Encoding(true).GetBytes(textBoxDisplay.Text);
                    fs.Write(info, 0, info.Length);

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog opendlg = new OpenFileDialog();
                opendlg.Filter = "wav files (*.wav)|*.wav";
                opendlg.Title = "Open a Wav File";

                Nullable<bool> result = opendlg.ShowDialog();

                if(result == true)
                {
                    FileStream fs = new FileStream(opendlg.FileName, FileMode.Open, FileAccess.Read);

                    spe.SetInputToWaveStream(fs);
                    spe.LoadGrammar(new DictationGrammar());
                    spe.RecognizeAsync(RecognizeMode.Multiple);
                    
                    spe.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(spe_recognised);
                    
                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void spe_recognised(object sender, SpeechRecognizedEventArgs e)
        {
            textBoxDisplay.Text = e.Result.Text;
        }

        private void buttonOpenTXT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog opendlg = new OpenFileDialog();
                opendlg.Filter = "txt files (*.txt)|*.txt";
                opendlg.Title = "Open a TXT File";

                Nullable<bool> result = opendlg.ShowDialog();

                if (result == true)
                {
                    FileStream fs = new FileStream(opendlg.FileName, FileMode.Open, FileAccess.Read);

                    StreamReader sr = new StreamReader(fs);
                    while (sr.ReadLine() != null)
                    {
                        textBox.Text = textBox.Text + sr.ReadLine();
                        textBox.Text = textBox.Text + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
