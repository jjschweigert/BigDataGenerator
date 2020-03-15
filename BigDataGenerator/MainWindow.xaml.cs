using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BigDataGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private StreamWriter OutputFileStream { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private bool IsFileInUse
        {
            get
            {
                try
                {
                    using (var fs = new FileStream(FilePath, FileMode.Open))
                    {
                        return !fs.CanWrite;
                    }
                }
                catch { }

                return true;
            }
        }

        private string _NumberOfRecords { get; set; }
        public string NumberOfRecords
        {
            get
            {
                return _NumberOfRecords;
            }
            set
            {
                _NumberOfRecords = value;
                OnPropertyChanged(nameof(NumberOfRecords));
            }
        }

        private string _FilePath { get; set; }
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        private string _Results { get; set; }
        public string Results
        {
            get
            {
                return _Results;
            }
            set
            {
                _Results = value;
                OnPropertyChanged(nameof(Results));
            }
        }

        private string _FilePathImageSource { get; set; }
        public string FilePathImageSource
        {
            get
            {
                return _FilePathImageSource;
            }
            set
            {
                _FilePathImageSource = value;
                OnPropertyChanged(nameof(FilePathImageSource));
            }
        }

        private int _CurrentProgressValue { get; set; }
        public int CurrentProgressValue
        {
            get
            {
                return _CurrentProgressValue;
            }
            set
            {
                _CurrentProgressValue = value;
                OnPropertyChanged(nameof(CurrentProgressValue));
            }
        }

        private ICommand _FilePathClickedEvent { get; set; }
        public ICommand FilePathClickedEvent
        {
            get
            {
                return _FilePathClickedEvent ?? (_FilePathClickedEvent = new CommandHandler(() => FilePathClicked(), null));
            }
        }

        private int RecordCount
        {
            get
            {
                return int.Parse(NumberOfRecords);
            }
        }

        private int CompletedTaskCount { get; set; }
        private int TotalTasksCount { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            NumberOfRecords = "1000000";
            BatchSize = "500";
            FilePath = @"C:\sample.csv";
            FilePathImageSource = @"\Resources\filepath_icon.png";
            IsGenerationInitializing = false;
            GenerationProgressVisibility = Visibility.Hidden;
        }

        private string _BatchSize { get; set; }
        public string BatchSize
        {
            get
            {
                return _BatchSize;
            }
            set
            {
                _BatchSize = value;
                OnPropertyChanged(nameof(BatchSize));
            }
        }

        private int RecordsPerTask
        { 
            get
            {
                return int.Parse(BatchSize);
            }
        }

        private bool _IsGenerationInitializing { get; set; }
        public bool IsGenerationInitializing
        {
            get
            {
                return _IsGenerationInitializing;
            }
            set
            {
                _IsGenerationInitializing = value;

                if(value)
                {
                    InitializationProgressVisibility = Visibility.Visible;
                    GenerationProgressVisibility = Visibility.Visible;
                }
                else
                {
                    InitializationProgressVisibility = Visibility.Hidden;
                }

                OnPropertyChanged(nameof(IsGenerationInitializing));
            }
        }

        private Visibility _InitializationProgressVisibility { get; set; }
        public Visibility InitializationProgressVisibility
        {
            get
            {
                return _InitializationProgressVisibility;
            }
            set
            {
                _InitializationProgressVisibility = value;
                OnPropertyChanged(nameof(InitializationProgressVisibility));
            }
        }

        private Visibility _GenerationProgressVisibility { get; set; }
        public Visibility GenerationProgressVisibility
        {
            get
            {
                return _GenerationProgressVisibility;
            }
            set
            {
                _GenerationProgressVisibility = value;
                OnPropertyChanged(nameof(GenerationProgressVisibility));
            }
        }

        private async void GeneratorLoop(int count, string identifier)
        {
            string Output = "";

            for (int i = 0; i < count; i++)
            {
                await Task.Run(() =>
                {
                    string Line = identifier.ToString() + i.ToString() + ",10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000,10000";
                    Output += Line + "\n";
                });
            }

            try 
            {
                int retry = 0;

                while(retry < 5)
                {
                    try
                    {
                        await OutputFileStream.WriteLineAsync(Output).ConfigureAwait(false);
                        //await OutputFileStream.FlushAsync().ConfigureAwait(false);

                        break;
                    }
                    catch { await Task.Delay(500); }
                }

                if (retry >= 5)
                {
                    StatusUpdate("Failed", "Unable to add generated data set to the file " + FilePath);
                }
            }
            catch(Exception ex)
            {
                StatusUpdate("Error", ex.Message);
            }

            CompletedTaskCount++;
            CurrentProgressValue = (int)((double)CompletedTaskCount / (double)TotalTasksCount * 100);
        }

        private async void FilePathClicked()
        {
            MessageBox.Show("Filepath clicked.");
        }

        private void StatusUpdate(string Title, string Message)
        {
            Results += (Title + " - " + Message) + "\n";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            IsGenerationInitializing = true;
            Results = "";
            CompletedTaskCount = 0;
            Stopwatch ProgressTimer = new Stopwatch();

            try { await File.WriteAllTextAsync(FilePath, string.Empty); }
            catch (Exception ex)
            {
                StatusUpdate("Error", ex.Message);
                IsGenerationInitializing = false;
                GenerationProgressVisibility = Visibility.Hidden;
                return;
            }

            ProgressTimer.Start();

            try
            {
                OutputFileStream = new StreamWriter
                (
                    path: FilePath,
                    append: true,
                    Encoding.UTF8,
                    bufferSize: 65536
                );
            }
            catch(Exception ex)
            {
                StatusUpdate("Error", ex.Message);
                IsGenerationInitializing = false;
                GenerationProgressVisibility = Visibility.Hidden;
                return;
            }

            OutputFileStream.AutoFlush = true;
            TotalTasksCount = RecordCount / RecordsPerTask;
            CurrentProgressValue = 0;

            if (TotalTasksCount == 0)
            {
                await Task.Factory.StartNew(() => { GeneratorLoop(RecordCount, "sample"); });
            }
            else
            {
                await Task.Run(() =>
                {
                    Parallel.For(0, TotalTasksCount, async i =>
                    {
                        await Task.Factory.StartNew(() => { GeneratorLoop(RecordsPerTask, "sample"); });
                    });
                });
            }

            IsGenerationInitializing = false;

            await Task.Run(async () =>
            {
                while (CompletedTaskCount < TotalTasksCount)
                {
                    await Task.Delay(300);
                }
            });

            try
            {
                OutputFileStream.Close();
            }
            catch(Exception ex)
            {
                StatusUpdate("Error", ex.Message);
            }

            ProgressTimer.Stop();
            TimeSpan ProgressTimeElapsed = ProgressTimer.Elapsed;

            StatusUpdate("Completed", "Generated dataset in " + ProgressTimeElapsed.TotalSeconds.ToString() + " seconds.");
        }
    }
}
