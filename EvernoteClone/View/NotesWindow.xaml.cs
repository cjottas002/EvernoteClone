using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Azure.Storage.Blobs;
using EvernoteClone.ViewModel;
using EvernoteClone.ViewModel.Helpers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        private readonly NotesVm _viewModel;
        public NotesWindow()
        {
            InitializeComponent();

            _viewModel = Resources["Vm"] as NotesVm;
            if (_viewModel != null) _viewModel.SelectedNoteChanged += ViewModel_SelectedNoteChanged;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            var fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48 };
            FontSizeComboBox.ItemsSource = fontSizes;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!string.IsNullOrEmpty(App.UserId)) return;
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        private async void ViewModel_SelectedNoteChanged(object sender, EventArgs e)
        {
            ContentRichTextBox.Document.Blocks.Clear();

            if (_viewModel.SelectedNote?.FileLocation == null) return;

            var downloadPath = $"{_viewModel.SelectedNote.Id}.rtf";
            await new BlobClient(new Uri(_viewModel.SelectedNote.FileLocation)).DownloadToAsync(downloadPath);
            var fs = new FileStream(downloadPath, FileMode.Open);
            var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
            contents.Load(fs, DataFormats.Rtf);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            const string region = "southcentralus";
            const string key = "xxxx"; // Azure Service Speech

            var speechConfig = SpeechConfig.FromSubscription(key, region);
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(result.Text)));

        }

        private void ContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var amountOfCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBlock.Text = $"Document lenght: {amountOfCharacters} characters";
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            var isButtonChecked = (((ToggleButton)sender)).IsChecked ?? false;

            ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, isButtonChecked ? FontWeights.Bold : FontWeights.Normal);
        }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = ContentRichTextBox.Selection.GetPropertyValue(FontWeightProperty);
            BoldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedWeight.Equals(FontWeights.Bold));

            var selectedStyle = ContentRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicButton.IsChecked = (selectedStyle != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            var selectedDecoration = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineButton.IsChecked = (selectedDecoration != DependencyProperty.UnsetValue) && (selectedDecoration.Equals(TextDecorations.Underline));

            FontFamilyComboBox.SelectedItem = ContentRichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            FontSizeComboBox.Text = (ContentRichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty)).ToString();
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            var isButtonEnabled = ((ToggleButton)sender).IsChecked ?? false;

            if (isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            else
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            var isButtonEnabled = ((ToggleButton)sender).IsChecked ?? false;

            if (isButtonEnabled)
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            else
            {
                ((TextDecorationCollection)ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty)).TryRemove(TextDecorations.Underline, out var textDecorations);
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem is not null)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ContentRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeComboBox.Text);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileName = $"{_viewModel.SelectedNote.Id}.rtf";
            var rtfFile = Path.Combine(Environment.CurrentDirectory, fileName);

            using var fileStream = new FileStream(rtfFile, FileMode.Create);
            var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
            contents.Save(fileStream, DataFormats.Rtf);

            _viewModel.SelectedNote.FileLocation = await UpdateFileAsync(rtfFile, fileName);
            await DatabaseHelper.Update(_viewModel.SelectedNote);

        }

        private async Task<string> UpdateFileAsync(string rtfFilePath, string fileName)
        {
            var connectionString = "";
            var containerName = "notes";

            var container = new BlobContainerClient(connectionString, containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(rtfFilePath);

            return $"https://evernotestoragelpa.blob.core.windows.net/notes/{fileName}";
        }
    }
}
