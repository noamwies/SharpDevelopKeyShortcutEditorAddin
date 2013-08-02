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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;
using System.Collections.ObjectModel;
using MoreLinq;

namespace KeyShortcutEditorAddin
{
	/// <summary>
	/// Interaction logic for EditShortcutsuPanel.xaml
	/// </summary>
	public partial class EditShortcutsuPanel : OptionPanel
	{
		private EditKeyShortcutInAddinFiles _shortcutsEditor;
		private XmlSerializer _serialzer;
		private static readonly string SHARP_DEVELOP_SHORTCUTS_FILTER = "#Develop Shortcuts|*.sht";
		private static readonly string SHORTCUTS_DEFAULT_DIRECTORY;
		private static readonly string SHORTCUTS_EDITOR_PLUGIn_DIRECTORY;
		public ObservableCollection<ShortcutView> Shortcuts { get; set; }
	
		static EditShortcutsuPanel(){
			SHORTCUTS_EDITOR_PLUGIn_DIRECTORY = Path.GetDirectoryName(Assembly.GetAssembly(typeof(EditShortcutsuPanel)).Location);
			SHORTCUTS_DEFAULT_DIRECTORY = Path.Combine(SHORTCUTS_EDITOR_PLUGIn_DIRECTORY,"Shortcuts");
			if (!Directory.Exists(SHORTCUTS_DEFAULT_DIRECTORY)) {
				Directory.CreateDirectory(SHORTCUTS_DEFAULT_DIRECTORY);
			}
		}
		
		public EditShortcutsuPanel()
		{
			InitializeComponent();
			_serialzer = new XmlSerializer(typeof(List<KeyShortcutModel>));
			_shortcutsEditor = new EditKeyShortcutInAddinFiles();
			Shortcuts = ModelToView(_shortcutsEditor.KeyShortcuts);
			_shortcutsView.DataContext = Shortcuts;
			foreach (var element in Shortcuts) {
				element.PropertyChanged += new PropertyChangedEventHandler(ShortcutChange);
			}
		}

		void ShortcutChange(object sender,  PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Key") {
				var s = sender as ShortcutView;
				ICSharpCode.Core.LoggingService.InfoFormatted("key shortcut of {0} changed to {1}",s.Operation,s.Key);
				_shortcutsEditor.ChangeKeyShortcut(s.Operation,s.Key);
				_applayLabel.Visibility = Visibility.Visible;
			}
		}

		
		
		public void Export(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = SHARP_DEVELOP_SHORTCUTS_FILTER;
			dialog.InitialDirectory = SHORTCUTS_DEFAULT_DIRECTORY;
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK) {
				string path = dialog.FileName;
				
				SaveKeyShortcutsToFile(path);
			}
		}

		void SaveKeyShortcutsToFile(string path)
		{
			if (false == String.IsNullOrEmpty(path)) {
				using (var file = new FileStream(path, FileMode.Create)) {
					_serialzer.Serialize(file, _shortcutsEditor.KeyShortcuts);
				}
			}
		}
		

		public void Import(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = SHARP_DEVELOP_SHORTCUTS_FILTER;
			dialog.InitialDirectory = SHORTCUTS_DEFAULT_DIRECTORY;
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK) {
				string path = dialog.FileName;
				if (false == String.IsNullOrEmpty(path)){
					using (var file = new FileStream(path,FileMode.Open))
					{
						var shortcuts = _serialzer.Deserialize(file) as List<KeyShortcutModel>;
						foreach (var s in shortcuts) {
							_shortcutsEditor.ChangeKeyShortcut(s.Operation,s.Key);
						}
					}
				}
			}
			Shortcuts = ModelToView(_shortcutsEditor.KeyShortcuts);
			Apply(null,null);
		}

		private ObservableCollection<ShortcutView> ModelToView(List<KeyShortcutModel> model) {
			return model.Select(s => new ShortcutView { Key = s.Key, Operation = s.Operation}).DistinctBy( s => s.Operation ).ToObservableCollection<ShortcutView>();
		}

		
		public void Apply(object sender, RoutedEventArgs e){
			string tempPath = Path.Combine(SHORTCUTS_DEFAULT_DIRECTORY,"current.tmp");
			SaveKeyShortcutsToFile(tempPath);
			var psi = new ProcessStartInfo();
			psi.FileName =Path.Combine(SHORTCUTS_EDITOR_PLUGIn_DIRECTORY,"ApplyConfiguration.exe");
			psi.Arguments = tempPath;
			psi.Verb = "runas";
			
			var process = new Process();
			process.StartInfo = psi;
			process.Start();
			process.WaitForExit();
			_restartLabel.Visibility = Visibility.Visible;
			_applayLabel.Visibility = Visibility.Collapsed;
		}
		
		void SelectAll_Checked(object sender, RoutedEventArgs e)
		{
			if (Shortcuts != null) {
				foreach (var element in Shortcuts) {
					element.ShouldExport = _selectAll.IsChecked.Value;
				}
			}
		}
		
		void Restart(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.Application.Restart();
			Environment.Exit(0);
		}
	}
}