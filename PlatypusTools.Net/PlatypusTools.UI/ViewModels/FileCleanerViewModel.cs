using PlatypusTools.Core.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlatypusTools.UI.ViewModels
{
    public class FileCleanerViewModel : BindableBase
    {
        public ObservableCollection<string> Results { get; } = new ObservableCollection<string>();

        private string _targetDir = "";
        public string TargetDir { get => _targetDir; set { _targetDir = value; RaisePropertyChanged(); } }

        private string _patterns = "";
        public string Patterns { get => _patterns; set { _patterns = value; RaisePropertyChanged(); } }

        private bool _dryRun = true;
        public bool DryRun { get => _dryRun; set { _dryRun = value; RaisePropertyChanged(); } }

        public ICommand ScanCommand { get; }

        public FileCleanerViewModel()
        {
            ScanCommand = new RelayCommand(async _ => await Scan());
        }

        public async Task Scan()
        {
            Results.Clear();
            var pats = string.IsNullOrWhiteSpace(Patterns) ? new string[0] : Patterns.Split(';');
            await Task.Run(() =>
            {
                foreach (var f in FileCleaner.GetFiles(TargetDir, pats, true)) Results.Add(f);
            });
        }
    }
}