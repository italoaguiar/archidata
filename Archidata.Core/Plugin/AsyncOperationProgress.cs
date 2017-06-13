using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archidata.Core
{
    public class AsyncOperationProgress: IProgress<OperationReport>
    {

        private static OperationReport _currentReport = new OperationReport();


        public static OperationReport CurrentStatus
        {
            get { return _currentReport; }
            set { _currentReport = value; }
        }

        
        public void Report(OperationReport value)
        {
            CurrentStatus.IsActive = true;
            CurrentStatus.Progress = value.Progress;
            CurrentStatus.OperationLabel = value.OperationLabel;                     
        }

        public void Report(string detail, double progress)
        {
            Report(new OperationReport(detail, progress));
        }

        public void End()
        {
            CurrentStatus.IsActive = false;
        }
    }

    public class OperationReport : INotifyPropertyChanged
    {
        public OperationReport()
        {
            
        }

        internal OperationReport(bool end)
        {
            IsActive = !end;
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            internal set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public OperationReport(string operationLabel, double progress)
        {
            this.OperationLabel = operationLabel;
            this.Progress = progress;
            IsActive = true;
        }

        private string operationName;
        private double progress;

        public string OperationLabel
        {
            get { return operationName; }
            set
            {
                operationName = value;
                OnPropertyChanged("OperationLabel");
            }
        }
        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
