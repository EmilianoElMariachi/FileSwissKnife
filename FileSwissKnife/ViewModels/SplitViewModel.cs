using System;
using System.Collections.Generic;
using System.Windows.Input;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public class SplitViewModel : TabViewModelBase
    {
        private readonly List<Unit> _units = new List<Unit>();
        private string _inputFile;
        private string _splitSizeStr;
        private ulong? _splitSize;
        private Unit _selectedUnit;
        private string _errorMessage;
        private string _outputFolder;

        public SplitViewModel()
        {
            const int K_FACTOR = 1000;
            const int KI_FACTOR = 1024;

            _units.Add(new Unit(Localizer.Instance.UnitB, (int)Math.Pow(K_FACTOR, 0)));
            _units.Add(new Unit(Localizer.Instance.UnitKB, (int)Math.Pow(K_FACTOR, 1)));
            _units.Add(new Unit(Localizer.Instance.UnitKiB, (int)Math.Pow(KI_FACTOR, 1)));
            _units.Add(new Unit(Localizer.Instance.UnitMB, (int)Math.Pow(K_FACTOR, 2)));
            _units.Add(new Unit(Localizer.Instance.UnitMiB, (int)Math.Pow(KI_FACTOR, 2)));
            _units.Add(new Unit(Localizer.Instance.UnitGB, (int)Math.Pow(K_FACTOR, 3)));
            _units.Add(new Unit(Localizer.Instance.UnitGiB, (int)Math.Pow(KI_FACTOR, 3)));

            SplitOrCancelCommand = new RelayCommand(SplitOrCancel);
        }

        public override string DisplayName => Localizer.Instance.TabNameSplit;

        public override string TechName => "Split";

        public List<Unit> Units => _units;

        public string InputFile
        {
            get => _inputFile;
            set
            {
                _inputFile = value;
                NotifyPropertyChanged();
            }
        }

        public string SplitSizeStr
        {
            get => _splitSizeStr;
            set
            {
                _splitSizeStr = value;
                _splitSize = null;

                try
                {
                    _splitSize = ParseSplitSize(value);
                }
                catch (Exception ex)
                {
                    this.ErrorMessage = ex.Message;
                }

                NotifyPropertyChanged();
            }
        }

        private static ulong ParseSplitSize(string splitSizeStr)
        {
            if (string.IsNullOrWhiteSpace(splitSizeStr))
                throw new Exception(Localizer.Instance.SplitSizeShouldBeDefined);

            if (!ulong.TryParse(splitSizeStr, out var splitSize))
                throw new Exception(string.Format(Localizer.Instance.SplitSizeInvalid, splitSizeStr));

            return splitSize;
        }

        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                NotifyPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged();
            }
        }

        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                _outputFolder = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SplitOrCancelCommand { get; }

        private void SplitOrCancel()
        {

            //TODO: à implémenter
        }

    }

    public class Unit
    {
        public Unit(string name, int factor)
        {
            Factor = factor;
            Name = name;
        }

        public string Name { get; }

        public int Factor { get; }
    }



}