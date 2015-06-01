using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GuassappTheReal
{
	public partial class UsersViewController : UIViewController
	{
		public string NumeroAPasar;
		public UsersViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var ts = new TableSourceUsers (this);
			Users.Source = ts;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			(segue.DestinationViewController as UbicacionViewController).phone_number = NumeroAPasar;
		}
	}
}
