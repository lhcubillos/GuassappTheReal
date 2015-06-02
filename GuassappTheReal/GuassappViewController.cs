using System;
using System.Drawing;
using System.Collections.Generic;
using Foundation;
using UIKit;
using AddressBook;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JSQMessagesViewController;
using System.Net;
using System.IO;

namespace GuassappTheReal
{
	public class User2{
		public string phone_number;
	}


	//23-05: 13:33 Lo que tengo que hacer ahora es modificar las clases de ChatViewController para soportar
	//bien una conversacion. Además, sería bueno arreglar el GruposViewController, para que muestre bien los grupos
	//en los que estoy.

	//14:33: Voy a partir por crear conversaciones de a 2 personas. Cuando apreto el contacto, primero tengo q ver si tengo alguna
	//conversacion existente con él. Si no tengo, entonces crear una.

	//14:58: Lo que voy a hacer es que en la pantalla principal me aparezcan todas mis conversaciones separadas por 2 secciones: grupos y no
	//grupos. Voy a poner también que se puedan crear nuevas conversaciones. Si solo se pone un numero, entonces se crea una conversacion de a 2.
	//De lo contrario, se crea un grupo.


	//25-05 19:15: Ya hice en el TableSourceGrupos para que aparezcan las conversaciones y los grupos. Me falla eso sí xq cree un grupo con null
	//ahora voy a hacer que aparezcan las conversaciones en el grupo.

	//20:02: Puedo ver las conversaciones en el chat, y lo tengo coordinado con la base de datos. Sin embargo, me falta conectar todo al
	//servidor, recibir los mensajes desde allá, y guardar los grupos desde allá. También me falta crear un nuevo grupo. No debe ser tan difícil

	//20:43: Puedo actualizar los grupos directo desde el servidor, pero todavía no logro que la tabla se actualice instantáneamente. Hice que
	//se hicieran two conversations, pero parece que no funciona muy bien todavía. Me falta: recibir los mensajes desde el servidor, mandar
	//mensajes, crear grupos y actualizar bien las tablas.

	//23:55: Todavia no voy a hacer que la tabla se actualice al toque. Todavía no voy a agregar grupos, voy a hacer lo de los mensajes.

	//26/05 0:53: Puedo mandar y recibir mensajes en todos los grupos de más de un integrante. Todavía no puedo crear grupos, pero no hay
	//para qué implementarlo. Con esto voy a desarrollar lo de los mapas. Todavía no logro que aparezca el numero de quien envía el mensaje
	//arriba de la burbuja.

	//14:12: Ahora me toca trabajar en compartir la ubicación. Para esto, el que mande el mensaje tiene q crear un socket servidor,
	//mandar las propiedades (IP+Puerto) de este socket al API, y a través de éste mandar la ubicación constante. Primero  voy a crear un socket 
	//servidor. Voy a cambiar un poco la interfaz del chat, para permitirme enviar ubicación. Lo voy a poner en el clip.

	//15:23: Lo que voy a hacer es que pueda ver los contactos del API en una table view, y que cuando los aprete, me muestre su ubicacion
	//si es que me la están compartiendo. Voy a compartir la ubicación desde el chat, pero solo voy a poder ver la del otro en la otra pantalla.

	//16:11: En el view controller de ubicacion voy a intentar mostrar mi ubicacion primero, antes de mostrar la de los demas.

	//17:39: Puedo mostrar mi ubicación en el UbicacionViewController, y cuando la cambio, muestra la nueva ubicación.

	//18:21: Estoy recibiendo las ubicaciones que me compartieron en el UbicacionesViewController, pero todavía falta que me pongan el sender
	// en el json de respuesta para poder ver si el usuario en cuestión me está compartiendo su ubicacioón o no.

	//22:44: Estoy metido en un gran lío. No me funciona la licensia de Xamarin. Lo que estoy haciendo ahora es crear el socket cliente en
	//el UbicacionesViewController, y recibiendo los mensajes. El string lo voy a mandar de la forma "latitud,longitud".

	//23:54: En teoría está funcionando el compartir ubicación, pero todavía no lo he podido probar bien. Ya lo deployed a mi celular.


	//1/06 15:08: Tengo que implementar una annotation personalizada, en la que aparezca una foto. Además, tengo que hacer que pueda dejar de 
	//compartir la ubicacion (matar el thread), y para mandar la ubicacion, que los primeros bytes representen el largo del mensaje. Además,
	//podría mejorar los detalles chicos. Sería muy bueno arreglar el método para cuando se recibe una nueva ubicacion.

	//17:05: Me funciona mostrar una imagen, pero el problema es que se ve pésimo. No sé como editar la view que se muestra cuando 
	//apreto el annotation.

	//18:06: Para mostrar la imagen, esta tiene que ser de 32x32. Una página buena para convertirla es http://picresize.com/. Ya puedo
	//mostrar la imagen, ahora quiero subirlo a github y despues corregir errores.

	//2/06 11:37: Seria ideal poder arreglar lo de los sockets, para que funcione impecablemente.

	//12:28: Los sockets funcionan perfecto, se apagan cuando salgo del mapa, y se prenden cuando entro. Lo que me falta por implementar es
	//crear grupos, y mostrar los numeros que me llegan con los nombres de los contactos del teléfono. Eso lo puedo implementar para la proxima.

	//13:00: En la view de Users para las ubicaciones, ahora puedo ver cuales tengo en el telefono, y los veo con el nombre que los tengo
	//guardados. Ademas, la annotation la veo con ese nombre. Me falta crear grupos.

	public partial class GuassappViewController : UIViewController
	{
		public const string Yo = "56983362592";
		public const string token = "UAGIw55wqzjwzLKLuVVHvwtt";

		public Grupo grupoAPasar;

		public static string GetPrimerNumeroPersona(ABPerson persona){
			string numero = persona.GetPhones ().GetValues () [0];
			string Numero = "";
			for (int i = 0; i < numero.Length; i++) {
				if (i != 0 && i != 3 && i != 5 && i != 10)
					Numero += numero[i];
			}
			return Numero;
		}
		public GuassappViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			//Forma de acceder a los contactos del teléfono.

			//var db1 = new GruposDB ();
			//var db2 = new MensajesDB ();
			//db1.DeleteAll ();
			//db2.DeleteAll ();

			//botones nav bar de add y refresh
			var botonAgregar = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			var RefreshGrupos = new UIBarButtonItem (UIBarButtonSystemItem.Refresh);
			botonAgregar.Clicked += BotonAgregar_Clicked;
			RefreshGrupos.Clicked += RefreshGrupos_Clicked;
			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
				botonAgregar,
				RefreshGrupos
			};
			var botonUsuarios = new UIBarButtonItem (UIBarButtonSystemItem.Bookmarks);
			botonUsuarios.Clicked += BotonUsuarios_Clicked;
			this.NavigationItem.LeftBarButtonItem = botonUsuarios;





			var ts = new TableSourceGrupos (this);
			Contactos.Source = ts;



			var db = new MensajesDB ();
			//db.DeleteAll ();

		}

		void BotonUsuarios_Clicked (object sender, EventArgs e)
		{
			this.PerformSegue ("AUbicaciones",this);
		}

		void BotonAgregar_Clicked (object sender, EventArgs e)
		{
			//Agregar un nuevo grupo.
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
		}

		protected void CrearConversacion(string nombreGrupo, string integrantes){
			//Primero hago el request, después agrego a la base de datos, y actualizo la tabla.
			var usuarios = parsearUsuarios (integrantes);
			if (usuarios.Length == 1)
				CrearTwoConversation (usuarios [0], nombreGrupo);

			//Esto no esta funcionando para nada.
			Contactos.ReloadData();

		}

		////Todavía no voy a crear grupos, voy a trabajar con los que están.
		//public void CrearGrupo(string nombreGrupo, string[] usuarios){
		//	//debo crear un json con un array de los integrantes que quiero meter al grupo.
		//	string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/create_group_conversation";
		//	try 
		//	{
		//		var request = WebRequest.Create(URL) as HttpWebRequest;
		//		request.Method = "POST";
		//		request.ContentType = "application/json";
		//		request.Headers.Add("Authorization", "Token token="+token);
		//		var sw = new StreamWriter(request.GetRequestStream());
		//		
		//		//var g = new Grupo(){title=nombreGrupo, admin=Yo};
		//		//string json = JsonConvert.SerializeObject(g);
		//		Console.WriteLine(json);
		//		sw.Write(json);
		//		//sw.Flush();
		//		sw.Close();
		//		
		//		//Agrego el nuevo mensaje a la base de datos.
		//		HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
		//		StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
		//		string jsonRespuesta = sr.ReadToEnd ();
		//		var g2 = JsonConvert.DeserializeObject<Grupo>(jsonRespuesta);
		//		Console.WriteLine(g2.id);
		//		var db = new GruposDB();
		//		//db.DeleteAll();
		//		
		//		db.AddGrupo(g2);
		//		//AgregarUsuarios(usuarios, g2.id);
		//		
		//		//No me está funcionando insertar una row
		//		var grupos = db.GetGrupos();
		//		var row = grupos.FindIndex( g3 => g3==g2);
		//		//Grupos.ReloadData();
		//		var index = NSIndexPath.FromRowSection(row, 0);
		//		//Grupos.InsertRows(new NSIndexPath[]{index}, UITableViewRowAnimation.Fade);
		//		sr.Close ();
		//	}
		//	catch(Exception e)
		//	{
		//		UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
		//		_error.Show ();
		//	}
		//}

		//Funciona perfecto.
		public void CrearTwoConversation(string usuario, string nombreGrupo){
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/create_two_conversation";
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);
				var sw = new StreamWriter(request.GetRequestStream());
				var tc = new TwoConversation(Yo, usuario, nombreGrupo);
				string json = JsonConvert.SerializeObject(tc);
				Console.WriteLine(json);
				sw.Write(json);
				sw.Flush();
				sw.Close();

				//Agrego el nuevo mensaje a la base de datos.
				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();
				var g2 = JsonConvert.DeserializeObject<Grupo>(jsonRespuesta);
				Console.WriteLine(g2.id);
				var db = new GruposDB();
				db.AddGrupo(g2);

				//AgregarUsuarios(usuarios, g2.id);

				//No me está funcionando insertar una row
				//var grupos = db.GetGrupos();
				//var row = grupos.FindIndex( g3 => g3==g2);
				////Grupos.ReloadData();
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

		public string[] parsearUsuarios(string usuarios){
			var paraDevolver = usuarios.Split (new char[]{ ',' });
			return paraDevolver;
		}

		void RefreshGrupos_Clicked (object sender, EventArgs e)
		{
			//Refrescar la lista de grupos. Obtener todos los grupos de un usuario y meterlos a la base de datos
			string URL = "http://guasapuc.herokuapp.com/api/v2/users/get_conversations?phone_number="+Yo;

			List<Grupo> grupos;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();

				grupos = JsonConvert.DeserializeObject<List<Grupo>>(jsonRespuesta);
				sr.Close ();
				var db = new GruposDB();
				List<Grupo> Grupos = db.GetGrupos();
				foreach(Grupo g in grupos){
					if (!Grupos.Contains(g))
						db.AddGrupo(g);
				}
				Console.WriteLine("hola");
				//Esto no esta funcionando
				Contactos.ReloadData();

			}
			catch(Exception error)
			{
				UIAlertView _error = new UIAlertView ("Error", error.Message, null, "Ok", null);
				_error.Show ();
			}

		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "AChat") {
				var infoNuevaVista = (segue.DestinationViewController as ChatViewController2);
				infoNuevaVista.grupo = grupoAPasar;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}




	}
}
#endregion