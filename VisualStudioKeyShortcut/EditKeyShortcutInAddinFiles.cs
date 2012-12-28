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

namespace VisualStudioKeyShortcut
{
	/// <summary>
	/// Description of EditKeyShortcutInAddinFiles.
	/// </summary>
	public class EditKeyShortcutInAddinFiles
	{
		private string _addinDirectory;
		
		public List<KeyShortcut> KeyShortcuts { get;private  set; }
			
		public EditKeyShortcutInAddinFiles()
		{
			_addinDirectory = Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location);
			int prefixStopIndex = _addinDirectory.IndexOf("AddIns");
			_addinDirectory = _addinDirectory.Substring(0,prefixStopIndex+6); // add 6 for the word "AddIns"
			KeyShortcuts = new List<KeyShortcut>();
			string [] addinFiles = Directory.GetFiles(_addinDirectory,"*.addin",SearchOption.AllDirectories);
			foreach (var addin in addinFiles) {
				var xml = XDocument.Load(addin);				
				var shortcutsInXml = xml.Root.XPathSelectElements(@"//MenuItem[@shortcut]").ToList();
				foreach (var shortcuts in shortcutsInXml) {
					string name = shortcuts.Attribute("shortcut").Value;
					string label = shortcuts.Attribute("label").Value; 
					KeyShortcuts.Add(new KeyShortcut{AddinFileName = addin,HasModified = false,Shortcut=name,Label = label});
				}
			}
		}
		
		public void ChangeKeyShortcut(string label,string key){
			KeyShortcut shortcut = (from k in KeyShortcuts where k.Label == label select k).Single();
			shortcut.HasModified = true;
			shortcut.Shortcut = key;
		}
		
		public void Apply()
		{
			var keys = (from k in KeyShortcuts where k.HasModified == true select k ).ToLookup( s => s.AddinFileName , u => u);
			foreach (var file in keys) {
				using (var fileStream = new FileStream(file.Key,FileMode.Open)) {
					var xml = XDocument.Load(fileStream);						
					foreach (var shortcut in file) {
						var shortcutInXml = xml.Root.XPathSelectElement(string.Format(@"//MenuItem[@label='']",shortcut.Label));
						shortcutInXml.SetAttributeValue("shortcut",shortcut.Shortcut)												
					}
					fileStream.Seek(0,SeekOrigin.Begin);
					xml.Save(FileStream)
				}				
			}
		}
	}
}
