﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MicroVision.Modules.ImagePanel.ViewModels
{
    public class ImagePanelViewModel : BindableBase
    {
        private ImageSource _display = null;

        public ImageSource Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value);  }
        }



        public ImagePanelViewModel()
        {
            

        }

    }
}
