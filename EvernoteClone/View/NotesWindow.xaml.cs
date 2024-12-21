using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        public NotesWindow()
        {
            InitializeComponent();

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            var fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48 };
            FontSizeComboBox.ItemsSource = fontSizes;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            string region = "southcentralus";
            string key = "xxxx"; // Azure Service Speech

            var speechConfig = SpeechConfig.FromSubscription(key, region);
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(result.Text)));

        }

        private void ContentTichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountOfCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBlock.Text = $"Document lenght: {amountOfCharacters} characters";
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)      
        {
            bool isButtonChecked = (sender as ToggleButton).IsChecked ?? false;

            if (isButtonChecked)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            else
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);

        }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = ContentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            BoldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedWeight.Equals(FontWeights.Bold));

            var selectedStyle = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selectedDecoration = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineButton.IsChecked = (selectedDecoration !=  DependencyProperty.UnsetValue) && (selectedDecoration.Equals(TextDecorations.Underline));

            FontFamilyComboBox.SelectedItem = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontSizeComboBox.Text = (ContentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty)).ToString();
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;

            if(isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            else
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;

            if(isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                (ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out TextDecorationCollection textDecorations);
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FontFamilyComboBox.SelectedItem is not null)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSizeComboBox.Text);
        }
    }
}
