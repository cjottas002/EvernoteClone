using EvernoteClone.Model;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteClone.View.UserControls
{

    /// <summary>
    /// Interaction logic for DisplayNoteControl.xaml
    /// </summary>
    public partial class DisplayNoteControl : UserControl
    {
        public Note Note
        {
            get { return (Note)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(Note), typeof(DisplayNoteControl), new PropertyMetadata(null, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DisplayNoteControl noteUserControl)
            {
                noteUserControl.DataContext = noteUserControl.Note;
            }
        }
        public DisplayNoteControl()
        {
            InitializeComponent();
        }
    }
}
