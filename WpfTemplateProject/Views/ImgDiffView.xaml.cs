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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImgDiffTool.Views
{
    /// <summary>
    /// Interaction logic for ImgDiffView.xaml
    /// </summary>
    public partial class ImgDiffView : UserControl
    {
        public ImgDiffView()
        {
            InitializeComponent();
        }

        private void ResetZoom1_Click(object sender, RoutedEventArgs e)
        {
            Zoom1.Reset();
        }

        private void ResetZoom2_Click(object sender, RoutedEventArgs e)
        {
            Zoom2.Reset();
        }
    }
}
