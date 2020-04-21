using System;
using System.Windows;
using WindowsInput;

namespace GhostWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardInterceptor _keyboard;
        private InputSimulator _inputSimulator;
        private int _maxKeyboardDelay = 200;
        private int _minKeyboardDelay = 40;
        private Random _rnd = new Random();
        private int _textIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            _keyboard = new KeyboardInterceptor();
            _inputSimulator = new InputSimulator();

            _keyboard.PrintScreenKeyPressed += OnPintScreenKeyPressed;

            Text.Text = "The quick brown fox jumps over the lazy dog\r\rAnd some more text";
        }

        private void OnPintScreenKeyPressed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var text = Text.Text;
                char key;
                bool hasHadCharacter = false;
                while (_textIndex < text.Length)
                {
                    key = text[_textIndex];
                    if (hasHadCharacter && key == '\r')
                    {
                        return;
                    }
                    hasHadCharacter = key != '\r';
                    _inputSimulator.Keyboard.TextEntry(key);
                    _inputSimulator.Keyboard.Sleep(_rnd.Next(_minKeyboardDelay, _maxKeyboardDelay));
                    _textIndex++;
                }
            }));
        }
    }
}
