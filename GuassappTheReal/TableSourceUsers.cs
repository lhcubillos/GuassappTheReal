using System;
using AddressBook;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using SQLite;

namespace GuassappTheReal
{
	public class NewUser{
		
		[PrimaryKey, AutoIncrement]
		public int id{ get; set; }

		public string phone_number{ get; set; }
		public string api_key{ get; set; }
		public string password{ get; set; }
		public string url{ get; set; }
		public NewUser(){
		}

		public override string ToString ()
		{
			return string.Format ("{1}", phone_number);
		}
	}

	public class TableSourceUsers:UITableViewSource
	{
		const string Yo = "56983362592";
		const string token = "UAGIw55wqzjwzLKLuVVHvwtt";
		const string cellIdentifier1 = "CeldaContacto";

		public List<string> usuarios;
		//public List<ABPerson> Personas;
		public UsersViewController controlador;
		//public Dictionary<string, List<ABPerson>> IndexedTableItems = new Dictionary<string, List<ABPerson>>();
		//public List<string> keys = new List<string>();

		public TableSourceUsers (UsersViewController users)
		{
			controlador = users;
			usuarios = new List<string>{ "56961567267", "56911234567", "56968799501", "56962448489", "56979781752", "56977313306", "56911111111", "56989060056", "56981362982", "56995345775" };
			//GetUsers ();
		}

		public void GetUsers()
		{
			string URL = "http://guasapuc.herokuapp.com/users.json";
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();
				Console.WriteLine(jsonRespuesta);
				var usuarios2 = JsonConvert.DeserializeObject<List<NewUser>>(jsonRespuesta);
				foreach(NewUser u in usuarios2)
					Console.WriteLine(u);
				sr.Close ();
				//foreach(NewUser u in usuarios2)
				//	if (u.phone_number != Yo)
				//		usuarios.Add(u);

			}
			catch(Exception error)
			{
				UIAlertView _error = new UIAlertView ("Error", error.Message, null, "Ok", null);
				_error.Show ();
			}
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return usuarios.Count;
		}
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier1);
			if (cell == null) 
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier1);
			cell.TextLabel.Text = usuarios [indexPath.Row];//.phone_number;

			return cell;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//Para la transicion.

			//En este momento quiero ver si ya hay una conversacion entre los 2, o si debo crearla. Por mientras solo voy a crearla.


			//Esto al final no va a ser así
			//CrearConversacion (a);
			var a = usuarios[indexPath.Row];//.phone_number;
			controlador.NumeroAPasar = a;
			controlador.PerformSegue ("AMapa", this);

			tableView.DeselectRow (indexPath, true);

		}



	}
}

