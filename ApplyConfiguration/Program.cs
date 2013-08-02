/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 02/08/2013
 * Time: 11:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Linq;
using KeyShortcutEditorAddin;

namespace ApplyConfiguration
{
	class Program
	{
		public static void Main(string[] args)
		{
			var serialzer = new XmlSerializer(typeof(List<KeyShortcutModel>));
			string path = args[0];
			if (false == String.IsNullOrEmpty(path)){
				using (var file = new FileStream(path,FileMode.Open))
				{
					var shortcuts = serialzer.Deserialize(file) as List<KeyShortcutModel>;
					Apply(shortcuts);
				}
			}
		}
		
		public static void Apply(List<KeyShortcutModel> shortcuts)
		{
			var keys = (from k in shortcuts where k.HasModified == true select k ).ToLookup( s => s.AddinFileName , u => u);
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