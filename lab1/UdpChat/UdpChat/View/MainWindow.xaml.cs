﻿using System.Windows;

namespace UdpChat.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += (sender, args) => { ViewModel.OnWindowClosed(); };
        }
    }
}
