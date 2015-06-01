using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;



namespace GuassappTheReal
{
	public class Usuario{
		public string phone_number{ get; set; }
		public string conversation_id{ get; set; }
		public string nombre{ get; set; }
	}
	partial class GruposViewController : UIViewController
	{
		const string Yo = "56983362592";
		const string token = "UAGIw55wqzjwzLKLuVVHvwtt";
		public Grupo grupo;
		public GruposViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var botonAgregarGrupo = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			var RefreshGrupos = new UIBarButtonItem (UIBarButtonSystemItem.Refresh);
			botonAgregarGrupo.Clicked += BotonAgregarGrupo_Clicked;
			RefreshGrupos.Clicked += RefreshGrupos_Clicked;
			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
				botonAgregarGrupo,
				RefreshGrupos
			};
			var db = new GruposDB ();
			//db.DeleteAll ();
			//var source = new TableSourceGrupos (this);
			//source.controlador = this;
			//this.Grupos.Source = source;


		}

		protected void RefreshGrupos_Clicked (object sender, EventArgs e)
		{
			//Hago el request para ver las conversaciones mías
		}

		protected void BotonAgregarGrupo_Clicked (object sender, EventArgs e)
		{
			//Agrego una nueva conversacion al servidor y a mi base de datos.
			string nombreGrupo;
			string integrantes;
			var alerta = UIAlertController.Create("Nuevo Grupo", null,UIAlertControllerStyle.Alert);
			alerta.AddTextField (new Action<UITextField> ((UITextField obj) => obj.Placeholder = "Nombre Grupo"));
			alerta.AddTextField (new Action<UITextField> ((UITextField obj) => obj.Placeholder = "Integrantes: 56983362592, 56977313306"));

			alerta.AddAction (UIAlertAction.Create ("Create", UIAlertActionStyle.Default, new Action<UIAlertAction> (delegate(UIAlertAction obj) { 
				nombreGrupo = alerta.TextFields [0].Text;
				integrantes = alerta.TextFields [1].Text;
				CrearConversacion(nombreGrupo,integrantes);
			})));
			alerta.Message = "Ingrese el Nombre del grupo, y los integrantes de la forma 56977313306,56983362592, etc.";
			alerta.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
			PresentViewController (alerta, true, null);
			//this.PerformSegue("NuevoGrupo", this);
		}

		protected void CrearConversacion(string nombreGrupo, string integrantes){
			//Primero hago el request, después agrego a la base de datos, y actualizo la tabla.
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations";
			var usuarios = parsearUsuarios (integrantes);

			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);
				var sw = new StreamWriter(request.GetRequestStream());

				var g = new Grupo(){title=nombreGrupo, admin=Yo};
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
				Console.WriteLine(g2.id);
				var db = new GruposDB();
				//db.DeleteAll();

				db.AddGrupo(g2);
				AgregarUsuarios(usuarios, g2.id);

				//No me está funcionando insertar una row
				var grupos = db.GetGrupos();
				var row = grupos.FindIndex( g3 => g3==g2);
				//Grupos.ReloadData();
				var index = NSIndexPath.FromRowSection(row, 0);
				//Grupos.InsertRows(new NSIndexPath[]{index}, UITableViewRowAnimation.Fade);
				sr.Close ();
			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}
		}

		public string[] parsearUsuarios(string usuarios){
			var paraDevolver = usuarios.Split (new char[]{ ',' });
			return paraDevolver;
		}

		void AgregarUsuarios(string[] usuarios, string idGrupo)
		{
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/add_user";
			foreach (string usuario in usuarios) {
				var request = WebRequest.Create (URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add ("Authorization", "Token token=" + token);
				var sw = new StreamWriter (request.GetRequestStream ());

				var u = new Usuario (){ phone_number = usuario, conversation_id = idGrupo };
				string json = JsonConvert.SerializeObject (u);
				Console.WriteLine (json);
				sw.Write (json);
				//sw.Flush();
				sw.Close ();

				//Los integrantes de cada grupo los voy a ir llamando con un request, no los voy a guardar en base de datos.;
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "AChat") {
				//Console.WriteLine (segue.DestinationViewController is ChatViewController);
				//var infoParaNuevaVista = (segue.DestinationViewController as ChatViewControllerGrupos);
				//infoParaNuevaVista.grupo = this.grupoAPasar;
			}
		}
	}
}
