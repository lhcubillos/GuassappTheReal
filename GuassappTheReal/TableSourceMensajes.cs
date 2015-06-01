using System;
using UIKit;
using System.Collections.Generic;
using AddressBook;

namespace GuassappTheReal
{
	public class TableSourceMensajes: UITableViewSource
	{
		const string Identificador = "Celda Mensajes";

		readonly string yo;
		readonly Grupo grupo;

		public TableSourceMensajes (string YO,Grupo g)
		{
			//Mensajes = mensajes;
			this.grupo = g;
			yo = YO;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			var db = new MensajesDB ();
			List<Mensaje> Mensajes = db.GetMensajesConIDConversacion (grupo.id);
			return Mensajes.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (Identificador);
			if (cell == null) 
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, Identificador);
			//var m = Mensajes [indexPath.Row];

			//Arreglar para que tome todos los mensajes desde la base de datos.
			var db = new MensajesDB();
			//List<Mensaje> mensajes = db.GetMensajesCon(otro);
			var m = db.GetMensajes() [indexPath.Row];
			cell.DetailTextLabel.Text = m.created_at;
			if (m.sender == yo) {
				cell.BackgroundColor = UIColor.Yellow;
				cell.TextLabel.Text = "Yo: "+ m.content;
			} else {
				cell.BackgroundColor = UIColor.Orange;
				cell.TextLabel.Text = m.sender+": "+ m.content;
			}

			return cell;
		}
	}
}

