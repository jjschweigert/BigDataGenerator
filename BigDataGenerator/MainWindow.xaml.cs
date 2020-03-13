using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace BigDataGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
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

        private int RecordCount
        {
            get
            {
                return int.Parse(NumberOfRecords);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            NumberOfRecords = "1000000";
            FilePath = @"C:\sample.csv";
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
                await File.AppendAllTextAsync(FilePath, Output + "\n");
                StatusUpdate("Data Generated", "Added " + count + " records to " + FilePath);
            }
            catch(Exception ex)
            {
                StatusUpdate("Error", ex.Message);
            }
        }

        private void StatusUpdate(string Title, string Message)
        {
            Results += (Title + " - " + Message) + "\n";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Results = "";

            for(int i = 0; i < RecordCount / 1000; i ++)
            {
                await Task.Factory.StartNew(() => { GeneratorLoop(1000, "sample"); });
            }
        }
    }
}
