﻿namespace StreamingChart
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainWindowViewModel();
		}
	}
}
