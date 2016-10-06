﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace OverallTest
{
    /// <summary>
    /// Interaction logic for WindowView.xaml
    /// </summary>
    public partial class WindowView : Window
    {
        private readonly WindowViewModel ViewModel;

        public WindowView()
        {
            ViewModel = new WindowViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            ViewModel.Dispose();
        }
    }
}
