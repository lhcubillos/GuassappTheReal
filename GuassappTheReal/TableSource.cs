using System;
using AddressBook;
using System.Collections.Generic;
using UIKit;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace GuassappTheReal
{
	public class TwoConversation{
		public string first;
		public string second;
		public string title;
		public TwoConversation(string f, string s, string t){
			first = f;
			second = s;
			title = t;
		}
	}
	public class TableSource:UITableViewSource
	{
		const string Yo = "56983362592";
		const string token = "UAGIw55wqzjwzLKLuVVHvwtt";
		const string cellIdentifier1 = "CeldaContacto";

		public List<ABPerson> Personas;
		public UIViewController controlador;
		public Dictionary<string, List<ABPerson>> IndexedTableItems = new Dictionary<string, List<ABPerson>>();
		public List<string> keys = new List<string>();

		public TableSource (List<ABPerson> personas, UIViewController controlador)
		{
			Personas = personas;
			this.controlador = controlador;
			ParsearPersonas ();
		}

		private void ParsearPersonas()
		{			
			foreach (ABPerson a in Personas) {
				string key = "";
				if (a.LastName != null) {
					key = a.LastName [0].ToString ().ToUpper ();
				}else if(a.FirstName != null){
					key = a.FirstName [0].ToString ().ToUpper ();
				}else{
					continue;}
				if (IndexedTableItems.ContainsKey (key))
					IndexedTableItems [key].Add (a);
				else {
					IndexedTableItems.Add (key, new List<ABPerson> (){ a });
					keys.Add (key);
				}
			}
			keys.Sort ();
			foreach (string key in keys)
				IndexedTableItems [key].OrderBy(o=>o.LastName);
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return IndexedTableItems[(keys.ToArray())[section]].Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier1);
			if (cell == null) 
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier1);
			ABPerson a = IndexedTableItems [(keys.ToArray ()) [indexPath.Section]] [indexPath.Row];
			cell.TextLabel.Text = a.FirstName + " " + a.LastName;

			return cell;
		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//Para la transicion.

			//En este momento quiero ver si ya hay una conversacion entre los 2, o si debo crearla. Por mientras solo voy a crearla.
			ABPerson a = IndexedTableItems [keys[indexPath.Section]][indexPath.Row];

			//Esto al final no va a ser así
			//CrearConversacion (a);
			//(controlador as GuassappViewController).contactoAPasar = a;
			//controlador.PerformSegue ("AMensajes", this);
			tableView.DeselectRow (indexPath, true);

		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return keys.Count;
		}

		public override string[] SectionIndexTitles (UITableView tableView)
		{
			return keys.ToArray ();
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return (keys.ToArray ()) [section];
		}


		protected void CrearConversacion(ABPerson persona){
			//Primero hago el request, después agrego a la base de datos
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/create_two_conversation";


			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);
				var sw = new StreamWriter(request.GetRequestStream());
				var g = new TwoConversation(Yo, GuassappViewController.GetPrimerNumeroPersona(persona), persona.FirstName+" "+persona.LastName);
				string json = JsonConvert.SerializeObject(g);
				Console.WriteLine(json);
				sw.Write(json);
				//sw.Flush();
				sw.Close();

				//Agrego el nuevo mensaje a la base de datos.
				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();
				var g2 = JsonConvert.DeserializeObject<Grupo>(jsonRespuesta);
				var db = new GruposDB();
				//db.DeleteAll();

				db.AddGrupo(g2);
				//AgregarUsuarios(usuarios, g2.id);

				//No me está funcionando insertar una row
				//var grupos = db.GetGrupos();
				//var row = grupos.FindIndex( g3 => g3==g2);
				//Grupos.ReloadData();
				//var index = NSIndexPath.FromRowSection(row, 0);
				//Grupos.InsertRows(new NSIndexPath[]{index}, UITableViewRowAnimation.Fade);
				sr.Close ();
			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}
		}
	}
}

