/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 26/12/2012
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace KeyShortcutEditorAddin
{
	/// <summary>
	/// Description of EditKeyShortcutInAddinFiles.
	/// </summary>
	public class EditKeyShortcutInAddinFiles
	{
		private string _additionalAddinDirectory;
		
		private string _defaultAddinDirectory;
		
		public List<KeyShortcutModel> KeyShortcuts { get;private  set; }
		
		public EditKeyShortcutInAddinFiles()
		{
			KeyShortcuts = new List<KeyShortcutModel>();
			
			// add shortcuts of manualy installed plugins
			_additionalAddinDirectory = Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location);
			int prefixStopIndex = _additionalAddinDirectory.IndexOf("AddIns");
			_additionalAddinDirectory = _additionalAddinDirectory.Substring(0,prefixStopIndex+6); // add 6 for the word "AddIns"
			foreach (var shortcut in ExtractShortcutsFromDirectory(_additionalAddinDirectory)) {
				KeyShortcuts.Add(shortcut);
			}
			
			// add shortcuts of automatically installed plugins
			_defaultAddinDirectory = FindSharpDeveloperApplication();
			_defaultAddinDirectory = _defaultAddinDirectory.Substring(0,_defaultAddinDirectory.IndexOf("bin")+3); // add 6 for the word "bin"
			_defaultAddinDirectory = Directory.GetParent(_defaultAddinDirectory).GetDirectories("AddIns",SearchOption.AllDirectories).FirstOrDefault().FullName;
			if (!string.IsNullOrEmpty(_defaultAddinDirectory) && Directory.Exists(_defaultAddinDirectory)) {
				foreach (var shortcut in ExtractShortcutsFromDirectory(_defaultAddinDirectory)) {
					KeyShortcuts.Add(shortcut);
				}				
			}			
		}
		
		private string FindSharpDeveloperApplication(){
			if (Environment.CurrentDirectory.IndexOf("SharpDevelop") > 0 ) {
				return Environment.CurrentDirectory;
			}
			string temp = Path.Combine(Environment.SpecialFolder.ProgramFilesX86.ToString(),"SharpDevelop");
			if (Directory.Exists(temp)) {
				return Path.Combine(temp,Directory.EnumerateDirectories(temp).LastOrDefault());
			}
			temp = Path.Combine(Environment.SpecialFolder.ProgramFiles.ToString(),"SharpDevelop");
			if (Directory.Exists(temp)) {
				return Path.Combine(temp,Directory.EnumerateDirectories(temp).LastOrDefault());
			}
			return "";			
		}
		
		private IEnumerable<KeyShortcutModel> ExtractShortcutsFromDirectory(string dir)
		{
			string[] addinFiles = Directory.GetFiles(dir, "*.addin", SearchOption.AllDirectories);
			foreach (var addin in addinFiles) {
				var xml = XDocument.Load(addin);
				var shortcutsInXml = xml.Root.XPathSelectElements("//MenuItem[@shortcut]").ToList();
				foreach (var shortcuts in shortcutsInXml) {
					string name = shortcuts.Attribute("shortcut").Value;
					string label = shortcuts.Attribute("label").Value;
					yield return new KeyShortcutModel {
					                 	AddinFileName = addin,
					                 	HasModified = false,
					                 	Key = name,
					                 	Operation = label
					                 };
				}
			}
		}

		public void ChangeKeyShortcut(string label,string key){
			IEnumerable<KeyShortcutModel> shortcuts = from k in KeyShortcuts where k.Operation == label select k;
			foreach (var shortcut in shortcuts) {
				if (shortcut.Key != key) {
					shortcut.HasModified = true;
					shortcut.Key = key;
				}
			}
		}
		
		public void Apply()
		{
			var keys = (from k in KeyShortcuts where k.HasModified == true select k ).ToLookup( s => s.AddinFileName , u => u);
			foreach (var file in keys) {
				XDocument xml;
				using (var fileStream = new FileStream(file.Key,FileMode.Open)) {
					xml = XDocument.Load(fileStream);
					foreach (var shortcut in file) {
						var shortcutsInXml = xml.Root.XPathSelectElements(string.Format(@"//MenuItem[@label='{0}']",shortcut.Operation));
						foreach (var shortcutInXml in shortcutsInXml) {
							shortcutInXml.SetAttributeValue("shortcut",shortcut.Key);
						}
					}
				}
				File.Delete(file.Key);
				using (var fileStream = new FileStream(file.Key,FileMode.CreateNew)) {
					using (var writer = new StreamWriter(fileStream)) {
						writer.Write(xml.ToString());
					}
				}
			}
		}
	}
}
