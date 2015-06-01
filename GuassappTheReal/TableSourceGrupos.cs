using System;
using UIKit;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace GuassappTheReal
{
	public class TableSourceGrupos : UITableViewSource
	{
		public const string Yo = "56983362592";

		string cellIdentifier1 = "Grupo";
		public GuassappViewController controlador;
		private List<Grupo> NoGrupos;
		private List<Grupo> Grupos;
		//Los voy a separar en 2 secciones: conversaciones y grupos. Por mientras, las conversaciones solo van a tener el 
		//telefono del otro contacto como titulo.

		//si no es un grupo, hago un request para ver sus integrantes. (si es un grupo también)
		public TableSourceGrupos (GuassappViewController contr)
		{
			controlador = contr;
			var db = new GruposDB ();
			//db.DeleteAll ();
			//db.AddGrupo (new Grupo{ id = "111", title = "Nuevo Grupo", admin = "56983362592", group = true });
			List<Grupo> grupos = db.GetGrupos();
			//separarlo por grupo y no grupo.
			NoGrupos = new List<Grupo>();
			Grupos = new List<Grupo>();
			foreach (Grupo g in grupos) {
				if (g.group == true)
					Grupos.Add (g);
				else if (g.group == false)
					NoGrupos.Add (g);
			}


		}

		public List<User> ObtenerIntegrantesGrupoConID(string id)
		{
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/get_users?conversation_id="+id;
			string token = "UAGIw55wqzjwzLKLuVVHvwtt";
			List<User> Usuarios;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();

				Usuarios = JsonConvert.DeserializeObject<List<User>>(jsonRespuesta);
				sr.Close ();
				return Usuarios;


			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}
			return null;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			//seccion cero, conversaciones de a 2. Seccion 1 grupos.
			return 2;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (section == 0)
				return NoGrupos.Count;
			else
				return Grupos.Count;
		
				
		}
		
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier1);
			if (cell == null) 
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier1);
			if (indexPath.Section == 0) {
				var g = NoGrupos [indexPath.Row];
				//List<User> Usuarios = ObtenerIntegrantesGrupoConID (g.id);
				//foreach (User u in Usuarios) {
				//	if (u.phone_number != Yo) {
				//		cell.TextLabel.Text = u.name;
				//		break;
				//	}
				//}
				cell.TextLabel.Text = g.title;

			}
			else {
				var g = Grupos [indexPath.Row];
				cell.TextLabel.Text = g.title;
			}

		
			return cell;
		}

		public override string[] SectionIndexTitles (UITableView tableView)
		{
			return new string[]{ "C", "G" };
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			if (section == 0)
				return "Contactos";
			else
				return "Grupos";
		}
		
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//Esto lo voy a ver cuando ya pueda visualizar todos mis grupos en la pantalla
			Grupo g = new Grupo();
			if (indexPath.Section == 0)
				g = NoGrupos [indexPath.Row];
			else
				g = Grupos [indexPath.Row];
			//Debo ir a una ventana donde vea los mensajes de la conversación. Debo pasarle el grupo
			(controlador).grupoAPasar = g;
			controlador.PerformSegue ("AChat", this);
			tableView.DeselectRow (indexPath, true);
		
		}
	
	}
}

