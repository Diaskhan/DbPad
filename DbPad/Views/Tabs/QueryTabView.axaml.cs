using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TextMateSharp.Grammars;

namespace DbPad.Views
{
    public class MyOverloadProvider : IOverloadProvider
    {
        private readonly IList<(string header, string content)> _items;
        private int _selectedIndex;

        public MyOverloadProvider(IList<(string header, string content)> items)
        {
            _items = items;
            SelectedIndex = 0;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                // ReSharper disable ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(CurrentHeader));
                OnPropertyChanged(nameof(CurrentContent));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public int Count => _items.Count;
        public string CurrentIndexText => $"{SelectedIndex + 1} of {Count}";
        public object CurrentHeader => _items[SelectedIndex].header;
        public object CurrentContent => _items[SelectedIndex].content;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            Text = text;
        }

        public IImage Image => null;

        public string Text { get; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => _contentControl ??= BuildContentControl();

        public object Description => "Description for " + Text;

        public double Priority { get; } = 0;

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        Control BuildContentControl()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = Text;
            textBlock.Margin = new Thickness(5);

            return textBlock;
        }

        Control _contentControl;
    }

    public partial class QueryTabView : UserControl
    {
        private readonly TextEditor? _textEditor;
        private CompletionWindow? _completionWindow;
        private OverloadInsightWindow? _insightWindow;

        public QueryTabView()
        {
            InitializeComponent();
            _textEditor = this.FindControl<TextEditor>("textEditor");
            _textEditor.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
            _textEditor.Background = Brushes.Transparent;
            _textEditor.ShowLineNumbers = true;
            _textEditor.TextArea.Background = this.Background;
            _textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _textEditor.Options.AllowToggleOverstrikeMode = true;
            _textEditor.Options.EnableTextDragDrop = true;
            _textEditor.Options.ShowBoxForControlCharacters = true;
            _textEditor.Options.ColumnRulerPositions = new List<int>() { 80, 100 };
            //_textEditor.TextArea.IndentationStrategy = new Indentation.CSharp.CSharpIndentationStrategy(_textEditor.Options);
            _textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _textEditor.TextArea.RightClickMovesCaret = true;
            _textEditor.Options.HighlightCurrentLine = true;
            _textEditor.Options.CompletionAcceptAction = CompletionAcceptAction.DoubleTapped;


            var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
            var textMateInstallation = _textEditor.InstallTextMate(registryOptions);
            Language sqlLanguage = registryOptions.GetLanguageByExtension(".sql");
            textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(sqlLanguage.Id));
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            //_statusTextBlock.Text = string.Format("Line {0} Column {1}",
            //    _textEditor.TextArea.Caret.Line,
            //    _textEditor.TextArea.Caret.Column);
        }

        private void textEditor_TextArea_TextEntering(object sender, TextInputEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            _insightWindow?.Hide();

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void textEditor_TextArea_TextEntered(object sender, TextInputEventArgs e)
        {
            if (e.Text == ".")
            {

                _completionWindow = new CompletionWindow(_textEditor.TextArea);
                _completionWindow.Closed += (o, args) => _completionWindow = null;

                var data = _completionWindow.CompletionList.CompletionData;

                for (int i = 0; i < 500; i++)
                {
                    data.Add(new MyCompletionData("Item" + i.ToString()));
                }

                data.Insert(20, new MyCompletionData("long item to demosntrate dynamic poup resizing"));

                _completionWindow.Show();
            }
            else if (e.Text == "(")
            {
                _insightWindow = new OverloadInsightWindow(_textEditor.TextArea);
                _insightWindow.Closed += (o, args) => _insightWindow = null;

                _insightWindow.Provider = new MyOverloadProvider(new[]
                {
                    ("Method1(int, string)", "Method1 description"),
                    ("Method2(int)", "Method2 description"),
                    ("Method3(string)", "Method3 description"),
                });

                _insightWindow.Show();
            }
        }
    }
}