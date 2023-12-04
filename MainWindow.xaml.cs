using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using FontAwesome.WPF;
using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;
using Timer = System.Threading.Timer;
using UserControl = System.Windows.Controls.UserControl;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            _doLists = new ToDoList(this);
            InitializeComponent();
            Time();
            _doLists.LoadData(StackPanel);
            calendar.AddHandler(PreviewMouseLeftButtonDownEvent,
                new RoutedEventHandler(DayButton_PreviewMouseLeftButtonDown));
        }

        private readonly ToDoList _doLists;
        DateTime _time = DateTime.Today;
        private readonly List<Button> _buttonListOfYears = new();
        private readonly List<Button> _countMonthButtons = new();
        private int _currentYearIndex;
        private int _currentYear = DateTime.Today.Year, _currentMonth = DateTime.Today.Month;
        private int _currentDay = DateTime.Today.Day;

        private void Time()
        {
            calendar.SelectedDate = _time;
            Day.Text = _time.ToString("dd");
            Mounth.Text = ToUpperTitle(_time.ToString("MMMM"));
            MounthCalendar.Text = ToUpperTitle(_time.ToString("MMMM"));
            DayOfWeek.Text = _time.ToString("dddd");
            SetStyleForYears(_time);
            SetStyleForMonthCount(_time);
            for (DateTime time = new DateTime(2023, 1, 1); time < DateTime.Today; time = time.AddDays(1))
            {
                calendar.BlackoutDates.Add(new CalendarDateRange(time));
            }
        }

        private void DayButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = calendar.SelectedDate;
            if (selectedDate != null)
            {
                _time = (DateTime)selectedDate;
            }

            _currentDay = _time.Day;
            _currentMonth = _time.Month;
            _currentYear = _time.Year;
            _doLists.UpdateStackPanel(StackPanel, _currentYear, _currentMonth, _currentDay);
            UpdateDate();
            ChangeCalendar();
            SetStyleForYears(_time);
            SetStyleForMonthCount(_time);
        }

        private void SetStyleForYears(DateTime time)
        {
            foreach (UIElement element in ListOfYears.Children)
            {
                if (element is Button button)
                {
                    _buttonListOfYears.Add(button ?? throw new InvalidOperationException());
                }
            }

            Style defaultButtonStyle1 = (Style)FindResource("Button");

            foreach (var elem in _buttonListOfYears)
            {
                elem.Style = defaultButtonStyle1;
            }

            foreach (Button element in _buttonListOfYears)
            {
                // ReSharper disable once InvertIf
                if ((string)element.Content == time.ToString("yyyy"))
                {
                    Style defaultButtonStyle = (Style)FindResource("ButtonForMonth");
                    element.Style = defaultButtonStyle;
                }
            }
        }

        private void SetStyleForMonthCount(DateTime time)
        {
            foreach (UIElement element in CountMonth.Children)
            {
                if (element is Button button)
                {
                    _countMonthButtons.Add(button ?? throw new InvalidOperationException());
                }
            }

            Style defaultStyle = (Style)FindResource("ButtonMonth");

            foreach (Button btn in _countMonthButtons)
            {
                btn.Style = defaultStyle;
            }

            foreach (Button btn in _countMonthButtons)
            {
                // ReSharper disable once InvertIf
                if ((string)btn.Content == time.ToString("MM"))
                {
                    Style defaultButtonStyle = (Style)FindResource("ButtonForCountMonth");
                    btn.Style = defaultButtonStyle;
                }
            }
        }

        private string ToUpperTitle(string time)
        {
            return char.ToUpper(time[0]) + time.Substring(1);
        }

        private void ClickOnYear(object sender, MouseButtonEventArgs mouseEventArgs)
        {
            Style defaultButtonStyle = (Style)FindResource("Button");

            foreach (var elem in _buttonListOfYears)
            {
                elem.Style = defaultButtonStyle;
            }

            if (sender is Button selectedBtn)
            {
                Style selectedButtonStyle = new Style(typeof(Button), defaultButtonStyle);
                selectedButtonStyle.Setters.Add(new Setter(FontSizeProperty, (double)22));
                selectedButtonStyle.Setters.Add(new Setter(ForegroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c76f69"))));
                selectedBtn.Style = selectedButtonStyle;
                _currentYearIndex = _buttonListOfYears.IndexOf(selectedBtn);
                _currentYear = Int32.Parse((string)selectedBtn.Content);
                _time = new DateTime(_currentYear, _currentMonth, _currentDay);
                DayOfWeek.Text = _time.ToString("dddd");
                calendar.SelectedDate = _time;

                ChangeCalendar();
            }
        }

        private void ClickOnMonth(object sender, MouseButtonEventArgs e)
        {
            Style defaultStyle = (Style)FindResource("ButtonMonth");

            foreach (Button btn in _countMonthButtons)
            {
                btn.Style = defaultStyle;
            }

            if (sender is Button selectedButton)
            {
                Style selectedButtonStyle = new Style(typeof(Button), defaultStyle);
                selectedButtonStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
                selectedButtonStyle.Setters.Add(new Setter(FontSizeProperty, (double)22));
                selectedButtonStyle.Setters.Add(new Setter(ForegroundProperty,
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c73f69"))));
                selectedButton.Style = selectedButtonStyle;
                _currentMonth = Int32.Parse((string)selectedButton.Content);
                MounthCalendar.Text = GetMonthName(_currentMonth);
                Mounth.Text = GetMonthName(_currentMonth);
                _time = new DateTime(_currentYear, _currentMonth, _currentDay);
                DayOfWeek.Text = _time.ToString("dddd");

                ChangeCalendar();
            }
        }

        private void ChangeCalendar()
        {
            calendar.DisplayDate = new DateTime(_currentYear, _currentMonth, _currentDay);
        }

        private string GetMonthName(int index)
        {
            switch (index)
            {
                case 1:
                    return "Январь";
                case 2:
                    return "Февраль";
                case 3:
                    return "Март";
                case 4:
                    return "Апрель";
                case 5:
                    return "Май";
                case 6:
                    return "Июнь";
                case 7:
                    return "Июль";
                case 8:
                    return "Август";
                case 9:
                    return "Сентябрь";
                case 10:
                    return "Октябрь";
                case 11:
                    return "Ноябрь";
                case 12:
                    return "Декабрь";
                default:
                    return new Exception("Invalid Error!").ToString();
            }
        }

        private void GoLeft(object sender, MouseButtonEventArgs e)
        {
            if (_currentYearIndex > 0)
            {
                _currentYearIndex--;
                SetActiveYear(_currentYearIndex);
            }
        }

        private void GoRight(object sender, MouseButtonEventArgs e)
        {
            if (_currentYearIndex < ListOfYears.Children.Count - 1)
            {
                _currentYearIndex++;
                SetActiveYear(_currentYearIndex);
            }
        }

        private void SetActiveYear(int index)
        {
            foreach (Button elem in ListOfYears.Children)
            {
                elem.Style = FindResource("Button") as Style;
            }

            ((Button)ListOfYears.Children[index]).Style = FindResource("ActiveButton") as Style;
            _currentYear = Int32.Parse((string)((Button)ListOfYears.Children[index]).Content);
            _time = new DateTime(_currentYear, _currentMonth, _currentDay);
            calendar.SelectedDate = _time;
            ChangeCalendar();
        }

        private void GoNextDay(object sender, MouseButtonEventArgs e)
        {
            if (_time == new DateTime(2027, 12, 31))
            {
                return;
            }

            _time = _time.AddDays(1);
            calendar.SelectedDate = _time;
            _currentDay = _time.Day;
            _currentMonth = _time.Month;
            _currentYear = _time.Year;
            MounthCalendar.Text = ToUpperTitle(_time.ToString("MMMM"));
            UpdateDate();
            ChangeCalendar();
            _doLists.UpdateStackPanel(StackPanel, _currentYear, _currentMonth, _currentDay);
            SetStyleForYears(_time);
            SetStyleForMonthCount(_time);
        }

        private void GoPreviousDay(object sender, MouseButtonEventArgs e)
        {
            if (_time == DateTime.Today)
            {
                return;
            }
            else
            {
                _time = _time.AddDays(-1);
                calendar.SelectedDate = _time;
                _currentDay = _time.Day;
                _currentMonth = _time.Month;
                _currentYear = _time.Year;
                MounthCalendar.Text = ToUpperTitle(_time.ToString("MMMM"));
                UpdateDate();
                ChangeCalendar();
                _doLists.UpdateStackPanel(StackPanel, _currentYear, _currentMonth, _currentDay);
                SetStyleForYears(_time);
                SetStyleForMonthCount(_time);
            }
        }

        private void AddTask(object sender, MouseButtonEventArgs e)
        {
            string task = TxtNote.Text, time = TxtTime.Text;
            if (!string.IsNullOrEmpty(task) && !string.IsNullOrEmpty(time))
            {
                if (StackPanel.Children.Count < 5)
                {
                    TxtNote.Clear();
                    TxtTime.Clear();
                    _doLists.AddTask(task, time, StackPanel, _currentYear, _currentMonth, _currentDay);
                    UpdateTextBlock();
                }
                else
                {
                    MessageBox.Show("Купите про-версию приложения!\n+375297603910", "Дайте деняк", MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                }
            }
        }

        private void UpdateDate()
        {
            Day.Text = _time.ToString("dd");
            DayOfWeek.Text = _time.ToString("dddd");
            Mounth.Text = ToUpperTitle(_time.ToString("MMMM"));
        }

        private void Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            TaskbarIcon.Visibility = Visibility.Visible;
            this.Hide();
        }

        private void ClickOnTaskBar(object sender, RoutedEventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Collapsed;
            this.Show();
        }

        private void CloseInTaskBar(object sender, RoutedEventArgs e)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // ReSharper disable once RedundantDelegateCreation
            Thread thread = new Thread(new ThreadStart(ShowSaveDialogAndShutdown));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ShowSaveDialogAndShutdown()
        {
            MessageBoxResult result = (MessageBoxResult)MessageBox.Show(
                "Хотите сохранить данные?\nНесохраненные данные будут удалены",
                "Выход",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == MessageBoxResult.Yes)
            {
                _doLists.SaveData();
                Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            }
        }

        public void UpdateTextBlock()
        {
            TextBlock.Text = $"{StackPanel.Children.Count} задач";
        }

        private void TextChanged(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void LblNote_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtNote.Focus();
        }

        private void TxtNote_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtNote.Text) && TxtNote.Text.Length > 0)
            {
                LblNote.Visibility = Visibility.Collapsed;
            }
            else
            {
                LblNote.Visibility = Visibility.Visible;
            }
        }

        private void TxtTime_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtTime.Text) && TxtTime.Text.Length > 0)
            {
                LblTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                LblTime.Visibility = Visibility.Visible;
            }
        }

        private void LblTime_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtTime.Focus();
        }
    }

    public partial class ToDoList : UserControl
    {
        public ToDoList(MainWindow mainWindow)
        {
            _main = mainWindow;
            InitializeComponent();
        }

        private readonly MainWindow _main;

        public string Title
        {
            get => (string)GetValue(Titleproperty);
            set => SetValue(Titleproperty, value);
        }

        public static readonly DependencyProperty Titleproperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ToDoList));

        public string Time
        {
            get => (string)GetValue(Timeproperty);
            init => SetValue(Timeproperty, value);
        }

        public static readonly DependencyProperty Timeproperty =
            DependencyProperty.Register(nameof(Time), typeof(string), typeof(ToDoList));

        public SolidColorBrush Color
        {
            get => (SolidColorBrush)GetValue(Colorproperty);
            set => SetValue(Colorproperty, value);
        }

        public static readonly DependencyProperty Colorproperty =
            DependencyProperty.Register(nameof(Color), typeof(SolidColorBrush), typeof(ToDoList));

        public FontAwesomeIcon Icon
        {
            get => (FontAwesomeIcon)GetValue(Iconproperty);
            set => SetValue(Iconproperty, value);
        }

        public static readonly DependencyProperty Iconproperty =
            DependencyProperty.Register(nameof(Icon), typeof(FontAwesomeIcon), typeof(ToDoList));

        public FontAwesomeIcon IconBell
        {
            get => (FontAwesomeIcon)GetValue(IconBellproperty);
            set => SetValue(IconBellproperty, value);
        }

        public static readonly DependencyProperty IconBellproperty =
            DependencyProperty.Register(nameof(IconBell), typeof(FontAwesomeIcon), typeof(ToDoList));

        private static List<ToDoList?>? ListData { get; } = new();

        private int CurrentYear { get; init; }
        private int CurrentMonth { get; init; }
        private int CurrentDay { get; init; }

        private Timer? _taskTimer;
        private readonly bool _timerEnabled = false;

        public void AddTask(string text, string time, StackPanel panel, int currentYear, int currentMonth,
            int currentDay)
        {
            var doLists = new ToDoList(_main)
            {
                Title = text,
                Time = time,
                Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f1f1f1")),
                Icon = FontAwesomeIcon.CircleThin,
                IconBell = FontAwesomeIcon.BellSlash,
                CurrentYear = currentYear,
                CurrentMonth = currentMonth,
                CurrentDay = currentDay,
            };
            panel.Children.Add(doLists);
            ListData?.Add(doLists);
            Timer(doLists, _timerEnabled);
        }

        private void RemoveTask(object sender, MouseButtonEventArgs e)
        {
            if (ListData != null)
            {
                ListData.Remove(this);
                if (Parent is StackPanel stackPanel)
                {
                    stackPanel.Children.Remove(this);
                    _main.UpdateTextBlock();
                }
            }
        }

        private void ChangeIcon(object sender, MouseButtonEventArgs e)
        {
            ToDoList? list = null;
            if (ListData != null)
            {
                foreach (var item in ListData)
                {
                    if (item?.Title == Title && item.Time == Time)
                    {
                        list = item;
                    }
                }
            }

            switch (Icon)
            {
                case FontAwesomeIcon.CircleThin:
                    Icon = FontAwesomeIcon.CheckCircle;
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eba5bb"));
                    IconBell = FontAwesomeIcon.BellSlash;
                    Timer(list, false);
                    break;
                case FontAwesomeIcon.CheckCircle:
                    Icon = FontAwesomeIcon.CircleThin;
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f1f1f1"));
                    IconBell = FontAwesomeIcon.Bell;
                    Timer(list, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeIconBell(object sender, MouseButtonEventArgs e)
        {
            ToDoList? list = null;
            if (ListData != null)
            {
                foreach (var item in ListData)
                {
                    if (item?.Title == Title && item.Time == Time)
                    {
                        list = item;
                    }
                }
            }

            switch (IconBell)
            {
                case FontAwesomeIcon.Bell:
                    IconBell = FontAwesomeIcon.BellSlash;
                    Timer(list, false);
                    break;
                case FontAwesomeIcon.BellSlash:
                    IconBell = FontAwesomeIcon.Bell;
                    Timer(list, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Edit(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Prompt();
            if (dialog.ShowDialog() == true)
            {
                Title = dialog.ResponseText;
            }
        }

        private void Timer(ToDoList? list, bool timerEnabled)
        {
            if (!timerEnabled || list == null)
            {
                return;
            }

            var timeParts = list.Time.Split('-');

            if (timeParts.Length == 2)
            {
                if (TimeSpan.TryParse(timeParts[0], out TimeSpan startTime) &&
                    TimeSpan.TryParse(timeParts[1], out TimeSpan endTime))
                {
                    DateTime currentTime = DateTime.Now;

                    DateTime taskStartTime = new DateTime(list.CurrentYear, list.CurrentMonth, list.CurrentDay,
                        endTime.Hours, endTime.Minutes, endTime.Seconds);

                    if (taskStartTime < currentTime)
                    {
                        taskStartTime = taskStartTime.AddDays(1);
                    }

                    TimeSpan delayTime = taskStartTime - currentTime;

                    if (delayTime.TotalMilliseconds > 0)
                    {
                        int days = (int)delayTime.TotalDays;
                        int remainingHours = delayTime.Hours;
                        int remainingMinutes = delayTime.Minutes;
                        int remainingSeconds = delayTime.Seconds;

                        _taskTimer = new Timer(state =>
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    list.Color =
                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eba5bb"));
                                    list.Icon = FontAwesomeIcon.CheckCircle;
                                    list.IconBell = FontAwesomeIcon.BellSlash;
                                    Message(null);
                                });
                            }, null, (days * 24 + remainingHours) * 60 * 60 * 1000 + remainingMinutes * 60 * 1000 +
                                     remainingSeconds * 1000, Timeout.Infinite);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            list.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eba5bb"));
                            list.Icon = FontAwesomeIcon.CheckCircle;
                            list.IconBell = FontAwesomeIcon.BellSlash;
                            Message(null);
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный формат времени.", "Ошибка", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Некорректный формат интервала времени.", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void Message(object? state)
        {
            var message = new ToastContentBuilder();
            message.AddText("Время задачи вышло!");
            message.Show();
        }

        public void SaveData()
        {
            using (StreamWriter file =
                   new StreamWriter(
                       "Data.txt",
                       false))
            {
                DateTime date = default;
                if (ListData != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var item in ListData)
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            file.WriteLine(
                                $"{date.ToString($"{item?.CurrentYear}-{item?.CurrentMonth}-{item?.CurrentDay}")} `{item?.Title}` {item?.Time} {item?.Color} FontAwesomeIcon.{item?.Icon} FontAwesomeIcon.{item?.IconBell}");
                        }
                    });
                }
            }
        }

        public void LoadData(StackPanel stackPanel)
        {
            try
            {
                if (File.Exists(
                        "Data.txt"))
                {
                    ListData?.Clear();
                    stackPanel.Children.Clear();

                    using (StreamReader reader = new StreamReader(
                               "Data.txt"))
                    {
                        string? line;
                        FontAwesomeIcon tempIcon = FontAwesomeIcon.None, tempIconBell = FontAwesomeIcon.None;

                        while ((line = reader.ReadLine()) != null)
                        {
                            var title = "";
                            int start = line.IndexOf('`') + 1, end = line.IndexOf('`', start);

                            if (start != -1 && end != -1)
                            {
                                title = line.Substring(start, end - start).Trim();
                            }

                            var parts = line.Split(' ');

                            var date = DateTime.Parse(parts[0]);
                            var time = parts[^4];
                            var color = parts[^3];
                            var icon = parts[^2];
                            var iconBell = parts[^1];

                            if (icon == "FontAwesomeIcon.CircleThin")
                            {
                                tempIcon = FontAwesomeIcon.CircleThin;
                            }
                            else if (icon == "FontAwesomeIcon.CheckCircle")
                            {
                                tempIcon = FontAwesomeIcon.CheckCircle;
                            }

                            if (iconBell == "FontAwesomeIcon.BellSlash")
                            {
                                tempIconBell = FontAwesomeIcon.BellSlash;
                            }
                            else if (iconBell == "FontAwesomeIcon.Bell")
                            {
                                tempIconBell = FontAwesomeIcon.Bell;
                            }

                            if (tempIcon == FontAwesomeIcon.CircleThin)
                            {
                                var task = new ToDoList(_main)
                                {
                                    CurrentYear = date.Year,
                                    CurrentMonth = date.Month,
                                    CurrentDay = date.Day,
                                    Title = title,
                                    Time = time,
                                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                                    Icon = tempIcon,
                                    IconBell = tempIconBell
                                };

                                if (task.IconBell == FontAwesomeIcon.Bell)
                                {
                                    Timer(task, true);
                                }

                                if (tempIcon == FontAwesomeIcon.CircleThin)
                                {
                                    ListData?.Add(task);
                                    stackPanel.Children.Add(task);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        public void UpdateStackPanel(StackPanel stackPanel, int currentYear, int currentMonth, int currentDay)
        {
            if (ListData != null)
            {
                stackPanel.Children.Clear();
                foreach (var item in ListData)
                {
                    if (item?.CurrentYear == currentYear && item.CurrentMonth == currentMonth &&
                        item.CurrentDay == currentDay)
                    {
                        stackPanel.Children.Add(item);
                    }
                }

                _main.UpdateTextBlock();
            }
        }
    }
}