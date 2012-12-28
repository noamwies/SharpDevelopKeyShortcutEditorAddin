/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 28/12/2012
 * Time: 14:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Gui;

namespace KeyShortcutEditorAddin
{
	/// <summary>
	/// Interaction logic for EditShortcutsuPanel.xaml
	/// </summary>
	public partial class EditShortcutsuPanel : OptionPanel
	{
		public EditShortcutsuPanel()
		{
			InitializeComponent();
		}
		
		public void Export(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Export");
		}
		
		public void Import(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Import");
		}
		
	}
}