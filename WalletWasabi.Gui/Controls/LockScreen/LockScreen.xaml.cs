﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml; 
namespace WalletWasabi.Gui.Controls.LockScreen
{
	public class LockScreen : UserControl
	{
		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
