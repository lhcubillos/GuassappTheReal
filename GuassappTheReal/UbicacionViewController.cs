using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using MapKit;
using CoreLocation;
using System.Collections.Generic;
using System.Net;
using System.IO;
using SQLite;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace GuassappTheReal
{
	//http://developer.xamarin.com/recipes/ios/content_controls/map_view/add_an_annotation_to_a_map/
	public class BasicMapAnnotation : MKAnnotation{
		//public override CLLocationCoordinate2D Coordinate {get;set;}
		public CLLocationCoordinate2D coords{get;set;}
		public override CLLocationCoordinate2D Coordinate {get {return coords;}}
		string title, subtitle;
		public override string Title { get{ return title; }}
		public override string Subtitle { get{ return subtitle; }}
		public BasicMapAnnotation (CLLocationCoordinate2D coordinate,string title, string subtitle) { 
			coords = coordinate;
			this.title = title;
			this.subtitle = subtitle;
		}
	}
	public class Conexion 
	{
		public int id{ get; set; }
		public string ip{ get; set;}
		public string port{ get; set; }
		public List<User> usuarios{ get; set; }
		public string sender{ get; set; }
		//public string created_at{ get; set;}

		public override string ToString ()
		{
			return string.Format ("[Conexion: id={0}, ip={1}, port={2}, usuarios={3}]", id, ip, port, usuarios);
		}
	}

	public class MapDelegate2 : MKMapViewDelegate{
		protected string annotationIdentifier = "BasicAnnotation";
		UIButton detailButton; // avoid GC
		IMKAnnotation annotation;
		public MapDelegate2(IMKAnnotation annotation){
			this.annotation = annotation;
		}
		public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{

			// try and dequeue the annotation view
			MKAnnotationView annotationView = mapView.DequeueReusableAnnotation(annotationIdentifier);   
			// if we couldn't dequeue one, create a new one
			if (annotationView == null)
				annotationView = new MKPinAnnotationView(annotation, annotationIdentifier);
			else // if we did dequeue one for reuse, assign the annotation to it
				annotationView.Annotation = this.annotation;

			// configure our annotation view properties
			annotationView.CanShowCallout = true;
			(annotationView as MKPinAnnotationView).AnimatesDrop = true;
			(annotationView as MKPinAnnotationView).PinColor = MKPinAnnotationColor.Red;
			annotationView.Selected = true;

			//annotationView.Image = UIImage.FromFile ("Images/Logo.png");

			detailButton = UIButton.FromType(UIButtonType.DetailDisclosure);
			detailButton.TouchUpInside += (s, e) => {
				Console.WriteLine ("Clicked");
				new UIAlertView("Annotation Clicked", "You clicked on " +
					(annotation as MKAnnotation).Coordinate.Latitude.ToString() + ", " +
					(annotation as MKAnnotation).Coordinate.Longitude.ToString() , null, "OK", null).Show();
			};


			//annotationView.RightCalloutAccessoryView = detailButton;
			var venueview = new UIImageView (UIImage.FromFile("Images/Avatar.png"));
			//venueview.ContentMode = UIViewContentMode.ScaleToFill;
			venueview.Frame = new RectangleF (10,10,venueview.Image.CGImage.Width, venueview.Image.CGImage.Height);
			annotationView.Frame = new CoreGraphics.CGRect (0, 0, 100, 100); 
			annotationView.LeftCalloutAccessoryView = venueview;
			return annotationView;	
		}
		   

	}

	//public class MapDelegate : MKMapViewDelegate
	//{
	//	static string annotationId = "BasicMapAnnotation";
	//	UIImageView venueView;
	//	UIImage venueImage;
	//
	//	public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
	//	{
	//		MKAnnotationView annotationView = null;
	//
	//		if (annotation is MKUserLocation)
	//			return null; 
	//
	//		if (annotation is BasicMapAnnotation) {
	//
	//			// show conference annotation
	//			annotationView = mapView.DequeueReusableAnnotation (annotationId);
	//
	//			if (annotationView == null)
	//				annotationView = new MKAnnotationView (annotation, annotationId);
	//
	//			//annotationView.Image = UIImage.FromFile ("Images/Logo.png");
	//			//annotationView.CanShowCallout = false;
	//		} 
	//
	//		return annotationView;
	//	}
	//	public override void DidSelectAnnotationView (MKMapView mapView, MKAnnotationView view)
	//	{
	//		// show an image view when the conference annotation view is selected
	//		if (view.Annotation is BasicMapAnnotation) {
	//			
	//			venueView = new UIImageView ();
	//			venueView.ContentMode = UIViewContentMode.ScaleAspectFit;
	//			venueImage = UIImage.FromFile ("Images/Avatar2.png");
	//			venueView.Frame = new CoreGraphics.CGRect (venueView.Center.X, venueView.Center.Y, venueView.Image.CGImage.Width, venueView.Image.CGImage.Height);
	//			venueView.Image = venueImage;
	//			view.AddSubview (venueView);
	//
	//			//UIView.Animate (0.4, () => {
	//			//	venueView.Frame = new CoreGraphics.CGRect (-75, -75, 200, 200); });
	//		}
	//	}
	//
	//	public override void DidDeselectAnnotationView (MKMapView mapView, MKAnnotationView view)
	//	{
	//		// remove the image view when the conference annotation is deselected
	//		if (view.Annotation is BasicMapAnnotation) {
	//
	//			venueView.RemoveFromSuperview ();
	//			venueView.Dispose ();
	//			venueView = null;
	//		}
	//	}
	//}

	partial class UbicacionViewController : UIViewController
	{
		public const string Yo = "56983362592";
		private const string token = "UAGIw55wqzjwzLKLuVVHvwtt";

		public string phone_number;
		public IPAddress IP;
		public string port;
		public Socket cliente;
		int i = 0;

		//Tengo que definir las coords cuando llame este controlador.
		CLLocationCoordinate2D coords;
		public event Action<CLLocationCoordinate2D> CambioCoordenadas;
		MKMapView mapView;
		MapDelegate2 mpdelegate;

		public UbicacionViewController (IntPtr handle) : base (handle)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = phone_number;
			var thread = new Thread (SimularServidor);
			thread.IsBackground = true;
			thread.Start ();
			ObtenerCoordenadas ();
			var annotation = new BasicMapAnnotation (coords, phone_number, "");

			CambioCoordenadas += UbicacionViewController_CambioCoordenadas;
			mapView = new MKMapView (View.Bounds);
			mpdelegate = new MapDelegate2 (annotation);
			mapView.Delegate = mpdelegate;
			mapView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			View.AddSubview(mapView);

			var clm = new CLLocationManager ();
			clm.RequestAlwaysAuthorization ();
			//mapView.ShowsUserLocation = true;


			var region = MKCoordinateRegion.FromDistance (coords, 2000, 2000);
			mapView.Region = region;
			mapView.AddAnnotation (annotation);

		

		}


		void UbicacionViewController_CambioCoordenadas (CLLocationCoordinate2D newcoords)
		{
			i++;
			InvokeOnMainThread ( () => {
				// manipulate UI controls
				mapView.AddAnnotation(new BasicMapAnnotation(newcoords,i.ToString(),"Iteracion"));
				Console.WriteLine ("Ubicación cambió");
				coords = newcoords;
				mapView.Region = MKCoordinateRegion.FromDistance (coords, 2000, 2000);
			});

		}

		//<summary>Obtengo las coordenadas del usuario que me las está compartiendo. Si no me está compartiendo, retorna false.</summary>
		public bool ObtenerCoordenadas()
		{
			//tengo que primero hacer un request al servidor para ver si este weon me esta compartiendo la wea.
			//primero hago un Get, para obtener todas las conexiones disponibles, y luego reviso si el usuario en el que estoy me esta
			//compartiendo su ubicación.
			string URL = "http://guasapuc.herokuapp.com/api/v2/shared_locations?phone_number="+Yo;
			try 
			{
				var request = WebRequest.Create(URL) as HttpWebRequest;
				request.Method = "GET";
				request.ContentType = "application/json";
				request.Headers.Add("Authorization", "Token token="+token);

				HttpWebResponse Httpresponse = (HttpWebResponse)request.GetResponse();
				StreamReader sr = new StreamReader (Httpresponse.GetResponseStream ());
				string jsonRespuesta = sr.ReadToEnd ();
				//Console.WriteLine();
				var conexiones = JsonConvert.DeserializeObject<List<Conexion>>(jsonRespuesta);
				bool meEstaCompartiendo = false;
				foreach(Conexion c in conexiones){
					if (c.sender == phone_number){
						meEstaCompartiendo = true;
						IP = IPAddress.Parse(c.ip);
						port = c.port;
						break;
					}
				}

				//Ya tengo la IP y el Puerto, ahora creo un socket client para conectarme al server socket, e ir recibiendo la ubicacion.
				if (meEstaCompartiendo){
					var thread = new Thread(RecibirMensajesServidor);
					thread.IsBackground = true;
					thread.Start();
				}
				sr.Close ();


			}
			catch(Exception error)
			{
				UIAlertView _error = new UIAlertView ("Error", error.Message, null, "Ok", null);
				_error.Show ();
				return false;
			}
			return true;

		}

		public void RecibirMensajesServidor()
		{

			cliente = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var ep = new IPEndPoint (IP, int.Parse(port));
			try{
				cliente.Connect (ep);
				while (true) {
					//Aquí tengo que recibir los mensajes con las ubicaciones.
					byte[] dataBuffer = new byte[20000];
					int largo = cliente.Receive (dataBuffer);
					string coordenadas = Encoding.ASCII.GetString (dataBuffer, 0, largo);
					//tengo que parsear las ubicaciones.
					string[] Coordenadas = coordenadas.Split (new char[]{ ',' });
					try {
						coords = new CLLocationCoordinate2D (double.Parse (Coordenadas [0]), double.Parse (Coordenadas [1]));
						if (CambioCoordenadas != null)
							CambioCoordenadas (coords);
					} catch (Exception e){
						Console.WriteLine("Error en Coords: "+e.Message);
					}
				}
			}
			catch(Exception e){
				Console.WriteLine ("Error en conexión de Socket: "+e.Message);
			}


		}

		public void SimularServidor(){

			var servidor = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var clientes = new List<Socket> ();
			IPEndPoint Ep = new IPEndPoint (IPAddress.Parse("192.168.1.34"), 8000);
			servidor.Bind (Ep);
			servidor.Listen (Int32.MaxValue);
			var clm = new CLLocationManager ();

			while (true) {
				var cliente = servidor.Accept ();
				clientes.Add (cliente);
				var coords = clm.Location.Coordinate;
				//var coords  = new CLLocationCoordinate2D(-33.5000469,-70.6133657);
				string message = coords.Latitude.ToString () + "," + coords.Longitude.ToString ();
				var data = Encoding.ASCII.GetBytes (message);
				cliente.Send (data);
			}
		}

	}
}
