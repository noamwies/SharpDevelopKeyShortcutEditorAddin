/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 26/12/2012
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace KeyShortcutEditorAddin
{
	/// <summary>
	/// Description of KeyShortcut.
	/// </summary>
	public class KeyShortcutModel
	{
		public bool HasModified { get; set; }
		
		public string Key { get; set; }
		
		public string AddinFileName { get; set; }
		
		public string Operation { get; set; }
		
		public KeyShortcutModel()
		{
			HasModified = false;
		}		
	}
}
