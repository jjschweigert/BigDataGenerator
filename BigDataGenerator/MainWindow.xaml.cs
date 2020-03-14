using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            FilePath = @"C:\sample.csv";
            FilePathImageSource = @"\Resources\filepath_icon.png";
        }


        private async void GeneratorLoop(int count, string identifier)
        {
            string Output = "";
            for(int i = 0; i < count; i ++)
            {
                string Line = identifier.ToString() + i.ToString() + ",100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100";
                Output += Line + "\n";
            }

            try 
            {
                //while(IsFileInUse)
                //{
                //    await Task.Delay(500);
                //}

                int retry = 0;

                while(retry < 5)
                {
                    try
                    {
                        await OutputFileStream.WriteLineAsync(Output);
                        break;
                    }
                    catch { await Task.Delay(300); }
                }

                if (retry < 5)
                {
                    StatusUpdate("Data Generated", "Added " + count + " records to " + FilePath);
                }
                else
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
            //Results += (Title + " - " + Message) + "\n";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Results = "";
            int SplitValue = 500;
            CompletedTaskCount = 0;

            try
            {
                OutputFileStream = new StreamWriter(FilePath);
            }
            catch(Exception ex)
            {
                StatusUpdate("Error", ex.Message);
                return;
            }

            OutputFileStream.AutoFlush = true;

            TotalTasksCount = RecordCount / SplitValue;

            for (int i = 0; i < TotalTasksCount; i ++)
            {
                await Task.Factory.StartNew(() => { GeneratorLoop(SplitValue, "sample"); }, TaskCreationOptions.LongRunning);
            }

            await Task.Run(async () =>
            {
                while (CompletedTaskCount < TotalTasksCount)
                {
                    //await Dispatcher.BeginInvoke((Action)(() => { CurrentProgressValue = (CompletedTaskCount / TotalTasksCount) * 10; }));
                    
                    await Task.Delay(500);
                }
            });

            OutputFileStream.Close();
        }
    }
}
