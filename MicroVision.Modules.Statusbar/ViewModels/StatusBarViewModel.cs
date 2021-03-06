﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Events;
using MicroVision.Core.Events;
using MicroVision.Modules.Statusbar.Models;
using MicroVision.Modules.Statusbar.Notifications;
using Prism.Interactivity.InteractionRequest;

namespace MicroVision.Modules.Statusbar.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        private List<StatusEntry> _log = new List<StatusEntry>();
        private readonly IEventAggregator _eventAggregator;
        private ImageSource _statusIcon;
        public ImageSource StatusIcon
        {
            get { return _statusIcon; }
            set { SetProperty(ref _statusIcon, value); }
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private DelegateCommand _ShowStatusLogCommand;
        public DelegateCommand ShowStatusLogCommand =>
            _ShowStatusLogCommand ?? (_ShowStatusLogCommand = new DelegateCommand(ExecuteShowStatusLogCommand));

        public InteractionRequest<INotification> ShowStatusLogRequest { get; } = new InteractionRequest<INotification>();

        void ExecuteShowStatusLogCommand()
        {
            InitializeStatus();
            ShowStatusLogRequest.Raise(new StatusLogNotification(_log));
        }

        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ExceptionEvent>().Subscribe(NotifyException, ThreadOption.UIThread);
            _eventAggregator.GetEvent<NotifyOperationEvent>().Subscribe(NotifyOperation, ThreadOption.UIThread);

            InitializeStatus();
        }

        private void InitializeStatus()
        {
            Status = "No Message";
            StatusIcon = CreateIcon(SystemIcons.Information);
        }

        private static ImageSource CreateIcon(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void NotifyOperation(string operation)
        {
            _log.Add(new StatusEntry(DateTime.Now, operation));
            StatusIcon = CreateIcon(SystemIcons.Information);
            Status = operation;
        }
        private void NotifyException(Exception exception)
        {
            _log.Add(new StatusEntry(DateTime.Now, exception.Message));
            StatusIcon = CreateIcon(SystemIcons.Error);
            Status = exception.Message;
        }
    }
}
