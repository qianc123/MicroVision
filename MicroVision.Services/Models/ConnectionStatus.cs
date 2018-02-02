﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace MicroVision.Services.Models
{
    public class ConnectionStatus : Status
    {
        private bool _isConnected = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }

        private bool _isError;

        public bool IsError
        {
            get { return _isError; }
            set { SetProperty(ref _isError, value); }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        public void RaiseError(string msg = null)
        {
            IsError = true;
            ErrorMessage = msg ?? "Error";
        }

        public void ResetError()
        {
            IsError = false;
            ErrorMessage = "";
        }

        public ConnectionStatus(string label) : base(label)
        {
        }
    }
}