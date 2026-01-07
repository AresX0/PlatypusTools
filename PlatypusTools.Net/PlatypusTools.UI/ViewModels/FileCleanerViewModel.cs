using PlatypusTools.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;
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

        private string _backupPath = string.Empty;
        public string BackupPath { get => _backupPath; set { _backupPath = value; RaisePropertyChanged(); } }

        public ICommand ScanCommand { get; }
        public ICommand DeleteCommand { get; }

        public FileCleanerViewModel()
        {
            ScanCommand = new RelayCommand(async _ => await Scan());
            DeleteCommand = new RelayCommand(async _ => await Delete());
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

        public async Task Delete()
        {
            // Simple confirmation
            var confirm = System.Windows.MessageBox.Show("Proceed to delete matched files?", "Confirm", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (confirm != System.Windows.MessageBoxResult.Yes) return;
            var pats = string.IsNullOrWhiteSpace(Patterns) ? new string[0] : Patterns.Split(';');
            var files = FileCleaner.GetFiles(TargetDir, pats, true).ToList();
            var removed = await Task.Run(() => FileCleaner.RemoveFiles(files, dryRun: DryRun, backupPath: string.IsNullOrWhiteSpace(BackupPath) ? null : BackupPath));
            System.Windows.MessageBox.Show($"Removed {removed.Count} files (dry-run={DryRun}).", "Done", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            await Scan();
        }
    }
}