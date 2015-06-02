using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using JSQMessagesViewController;
using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
using CoreLocation;
using MapKit;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace GuassappTheReal
{

	public class User {
		public string phone_number;
		public string name;
	}

	public class ConexionSalida{
		public string sender{ get; set; }
		public string port{ get; set; }
		public string ip{ get; set; }
		public string[] users{get;set;}

		public ConexionSalida(string sender, string ip, string port, string[] users){
			this.sender = sender;
			this.ip = ip;
			this.port = port;
			this.users = users;
		}

	}
	[Register ("ChatViewController2")]
	public class ChatViewController2 : MessagesViewController
	{
		public const string Yo = "56983362592";
		public const int PUERTO = 8000;
		public event Action UsuarioConectado;
		private const int CONEXIONESMAXIMAS = Int32.MaxValue;
		Socket servidor;
		List<Socket> clientes;
		List<User> numerosDeEsteGrupo;
		Thread threadLocations;

		public Grupo grupo;
		MessagesBubbleImage outgoingBubbleImageData, incomingBubbleImageData;
		//List<Message> messages = new List<Message> ();

		User sender = new User {phone_number = "56983362592", name = "LH Cubillos"};
		//User friend = new User {phone_number = "BADB229", name = "Tom Anderson"};

		public ChatViewController2(IntPtr handle):base(handle){
		}

		public List<User> GetNumerosGrupo(){

			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/get_users?conversation_id="+grupo.id;
			string token = "UAGIw55wqzjwzLKLuVVHvwtt";
			List<User> usuarios;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();

				usuarios = JsonConvert.DeserializeObject<List<User>>(jsonRespuesta);
				sr.Close ();
				return usuarios;

			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}
			return null;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = grupo.title;

			// You must set your senderId and display name
			SenderId = sender.phone_number;
			SenderDisplayName = sender.name;

			// These MessagesBubbleImages will be used in the GetMessageBubbleImageData override
			var bubbleFactory = new MessagesBubbleImageFactory ();
			outgoingBubbleImageData = bubbleFactory.CreateOutgoingMessagesBubbleImage (UIColorExtensions.MessageBubbleLightGrayColor);
			incomingBubbleImageData = bubbleFactory.CreateIncomingMessagesBubbleImage (UIColorExtensions.MessageBubbleBlueColor);

			// Remove the AccessoryButton as we will not be sending pics
			//InputToolbar.ContentView.LeftBarButtonItem 

			// Remove the Avatars
			CollectionView.CollectionViewLayout.IncomingAvatarViewSize = CoreGraphics.CGSize.Empty;
			CollectionView.CollectionViewLayout.OutgoingAvatarViewSize = CoreGraphics.CGSize.Empty;
			AutomaticallyScrollsToMostRecentMessage = true;

			numerosDeEsteGrupo = GetNumerosGrupo ();

			// Load some messagees to start
			//messages.Add (new Message (sender.phone_number, sender.name, NSDate.DistantPast, "Hi There"));
			//messages.Add (new Message (friend.phone_number, friend.name, NSDate.DistantPast, "I'm sorry, my responses are limited. You must ask the right questions."));
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath) 
		{
			var cell =  base.GetCell (collectionView, indexPath) as MessagesCollectionViewCell;

			var db = new MensajesDB ();
			List<Mensaje> mensajes = db.GetMensajesConIDConversacion (grupo.id);
			Mensaje m = mensajes [indexPath.Row];
			// Override GetCell to make modifications to the cell
			// In this case darken the text for the sender
			//var message = messages [indexPath.Row];
			if (m.sender == SenderId)
				cell.TextView.TextColor = UIColor.Black;
			cell.CellTopLabel.Text = m.sender;
			cell.MessageBubbleTopLabel.Text = m.sender;

			return cell;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			var db = new MensajesDB ();

			return db.GetMensajesConIDConversacion(grupo.id).Count;
		}

		public override IMessageData GetMessageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			var db = new MensajesDB ();
			List<Mensaje> mensajes = db.GetMensajesConIDConversacion (grupo.id);
			Mensaje m = mensajes [indexPath.Row];
			var message = new Message (m.sender, m.sender, NSDate.Now, m.content);	
			return message;
		}

		public override IMessageBubbleImageDataSource GetMessageBubbleImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			var db = new MensajesDB ();
			var messages = db.GetMensajesConIDConversacion (grupo.id);
			var message = messages [indexPath.Row];
			if (message.sender == SenderId)
				return outgoingBubbleImageData;
			return incomingBubbleImageData;

		}

		public override IMessageAvatarImageDataSource GetAvatarImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			return null;
		}

		public override async void PressedSendButton (UIButton button, string text, string senderId, string senderDisplayName, NSDate date)
		{
			SystemSoundPlayer.PlayMessageSentSound ();
			EnviarMensaje (text);
			FinishSendingMessage (true);

			await Task.Delay (500);

		}

		public void ShareLocation(UIAlertAction obj)
		{
			//Debo crear un server socket con mi IP y Puerto Actual, y luego mandar un POST al API diciendo que debo compartir esos datos con
			//los que correspondan en el grupo o conversacion. 
			var ip = IPAddress.Any;



			//Hago el POST:
			string URL = "http://guasapuc.herokuapp.com/api/v2/shared_locations";
			string token = "UAGIw55wqzjwzLKLuVVHvwtt";
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);
				var sw = new StreamWriter(request.GetRequestStream());

				//me faltan los usuarios que recibirían la conexión.
				List<string> usuarios = new List<string>();
				foreach(User u in numerosDeEsteGrupo)
					usuarios.Add(u.phone_number);
				var recipientes = usuarios.ToArray();
				var cs = new ConexionSalida(Yo, ip.ToString(), PUERTO.ToString(), recipientes);
				string json = JsonConvert.SerializeObject(cs);
				Console.WriteLine(json);
				sw.Write(json);
				sw.Flush();
				sw.Close();

			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}



			servidor = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			IPEndPoint Ep = new IPEndPoint (ip, PUERTO);
			servidor.Bind (Ep);
			servidor.Listen (CONEXIONESMAXIMAS);
			threadLocations = new Thread (RecibirConexiones);
			threadLocations.IsBackground = false;
			threadLocations.Start ();

		}

		public void RecibirConexiones()
		{
			while (true) {
				var clm = new CLLocationManager ();
				clm.RequestAlwaysAuthorization ();

				//var mapview = new MKMapView ();

				clm.LocationsUpdated += Clm_LocationsUpdated;
				var cliente = servidor.Accept ();
				clientes.Add (cliente);

				foreach(Socket client in clientes){
					var coords = clm.Location.Coordinate;
					string message = coords.Latitude.ToString () + "," + coords.Longitude.ToString ();
					var data = Encoding.ASCII.GetBytes(message);
					client.Send (data);
				}
			}
		}

		void Clm_LocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			//var mapview = new MKMapView ();
			var clm = new CLLocationManager();
			foreach(Socket client in clientes){
				var coords = clm.Location.Coordinate;
				string message = coords.Latitude.ToString () + "," + coords.Longitude.ToString ();
				var data = Encoding.ASCII.GetBytes(message);
				client.Send (data);
			}
		}

		void Mapview_DidUpdateUserLocation (object sender, MKUserLocationEventArgs e)
		{
			
		}

		public void RefreshMessages(UIAlertAction obj){

			ShowTypingIndicator = true;
			RecibirMensajesConversacion();
			FinishReceivingMessage(true);
			ScrollToBottom (true);
			Console.WriteLine ("perro");
		}

		public void StopShareLocation(UIAlertAction obj){
			if (threadLocations != null)
				threadLocations.Abort ();
		}


		public override void PressedAccessoryButton (UIButton sender)
		{
			base.PressedAccessoryButton (sender);
			var acc = new Action<UIAlertAction> (ShareLocation);
			var acc2 = new Action<UIAlertAction> (RefreshMessages);
			var acc3 = new Action<UIAlertAction> (StopShareLocation);
			var alerta = UIAlertController.Create(null, null,UIAlertControllerStyle.ActionSheet);
			alerta.AddAction(UIAlertAction.Create("Share Location", UIAlertActionStyle.Default, acc));
			alerta.AddAction(UIAlertAction.Create("Stop Share Location", UIAlertActionStyle.Default, acc3));
			alerta.AddAction(UIAlertAction.Create("Refresh Messages", UIAlertActionStyle.Default, acc2));
			alerta.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
			PresentViewController (alerta, true, null);

		}

		public void RecibirMensajesConversacion()
		{
			//Debo hacer un get con los mensajes de la conversacion, y actualizar la base de datos.
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/get_messages?conversation_id="+grupo.id;
			string token = "UAGIw55wqzjwzLKLuVVHvwtt";
			List<Mensaje> mensajes;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();

				mensajes = JsonConvert.DeserializeObject<List<Mensaje>>(jsonRespuesta);
				var db = new MensajesDB();
				var Mensajes = db.GetMensajesConIDConversacion(grupo.id);
				foreach(Mensaje m in mensajes){
					if (!Mensajes.Contains(m))
						db.AddMensaje(m);
				}
				sr.Close ();

			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}

		}

		public void EnviarMensaje(string text){
			string URL = "http://guasapuc.herokuapp.com/api/v2/conversations/send_message";
			string token = "UAGIw55wqzjwzLKLuVVHvwtt";
			List<Mensaje> mensajes;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);
				var sw = new StreamWriter(request.GetRequestStream());

				var m = new Mensaje(){content = text, sender = Yo, conversation_id = grupo.id};
				Console.WriteLine(m);
				string json = JsonConvert.SerializeObject(m);
				Console.WriteLine(json);
				sw.Write(json);
				//sw.Flush();
				sw.Close();

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();
				Console.WriteLine(jsonRespuesta);
				var mensaje = JsonConvert.DeserializeObject<List<Mensaje>>(jsonRespuesta)[0];
				Console.WriteLine(mensaje);
				var db = new MensajesDB();
				db.AddMensaje(mensaje);
				sr.Close ();

			}
			catch(Exception e)
			{
				UIAlertView _error = new UIAlertView ("Error", e.Message, null, "Ok", null);
				_error.Show ();
			}
		}

		//public override NSAttributedString GetCellTopLabelAttributedText (MessagesCollectionView collectionView, NSIndexPath indexPath)
		//{
		//	var db = new MensajesDB ();
		//	var mensajes = db.GetMensajesConIDConversacion (grupo.id);
		//	var mensaje = mensajes [indexPath.Row];
		//	return mensaje.sender;
		//	//return base.GetCellTopLabelAttributedText (collectionView, indexPath);
		//}

		async Task SimulateDelayedMessageReceived ()
		{
			ShowTypingIndicator = true;

			ScrollToBottom (true);

			var delay = System.Threading.Tasks.Task.Delay (1500);
			//var message = new Message(friend.phone_number, friend.name,NSDate.Now, "Hola");
			await delay;

			//messages.Add (message);

			ScrollToBottom (true);

			SystemSoundPlayer.PlayMessageReceivedSound ();

			FinishReceivingMessage (true);
		}

	}
}
