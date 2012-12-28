/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 26/12/2012
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VisualStudioKeyShortcut
{
	/// <summary>
	/// Description of KeyShortcut.
	/// </summary>
	public class KeyShortcut
	{
		public bool HasModified { get; set; }
		
		public string Shortcut { get; set; }
		
		public string AddinFileName { get; set; }
		
		public string Label { get; set; }
		
		public KeyShortcut()
		{
			HasModified = false;
		}		
	}
}
