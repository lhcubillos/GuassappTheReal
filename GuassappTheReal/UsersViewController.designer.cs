// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GuassappTheReal
{
	[Register ("UsersViewController")]
	partial class UsersViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView Users { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Users != null) {
				Users.Dispose ();
				Users = null;
			}
		}
	}
}
