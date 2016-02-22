using System;
using System.Linq;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Zeitansage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            string text = String.Format("Beim nächsten Ton ist es {0} Uhr, {1} Minuten und {2} Sekunden", dt.Hour, dt.Minute, dt.Second);
            Speak(text);
        }

        private async void Speak(string text)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Voice = (SpeechSynthesizer.AllVoices.First(x => x.Gender == VoiceGender.Female));
            SpeechSynthesisStream synthesisStream = null;
            try
            {
                synthesisStream = await synth.SynthesizeTextToStreamAsync(text);

            }
            catch (Exception)
            {
                synthesisStream = null;
            }

            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
        }
    }
}
