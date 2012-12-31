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
        public ObservableCollection<ShortcutView> Shortcuts { get; set; }

		static EditShortcutsuPanel(){
			SHORTCUTS_DEFAULT_DIRECTORY = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(EditShortcutsuPanel)).Location),"Shortcuts");
		}
		
		public EditShortcutsuPanel()
		{
			InitializeComponent();
			_serialzer = new XmlSerializer(typeof(List<KeyShortcutModel>));
			_shortcutsEditor = new EditKeyShortcutInAddinFiles();
            Shortcuts = ModelToView(_shortcutsEditor.KeyShortcuts);
            _shortcutsView.DataContext = Shortcuts;
		}
		
		public void Export(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = SHARP_DEVELOP_SHORTCUTS_FILTER;
			dialog.InitialDirectory = SHORTCUTS_DEFAULT_DIRECTORY;
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK) {
				string path = dialog.FileName;
				if (false == String.IsNullOrEmpty(path)){
					using (var file = new FileStream(path,FileMode.Create))
					{
						_serialzer.Serialize(file,_shortcutsEditor.KeyShortcuts);
					}
				}
			}
		}
		

		public void Import(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			var result = dialog.ShowDialog();
			dialog.Filter = SHARP_DEVELOP_SHORTCUTS_FILTER;
			dialog.InitialDirectory = SHORTCUTS_DEFAULT_DIRECTORY;
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
			_shortcutsEditor.Apply();
		}

        private ObservableCollection<ShortcutView> ModelToView(List<KeyShortcutModel> model) {
            return model.Select(s => new ShortcutView { Key = s.Key, Operation = s.Operation }).ToObservableCollection<ShortcutView>();
        }

		
		public void Apply(object sender, RoutedEventArgs e){
			_shortcutsEditor.Apply();
		}
	}
}