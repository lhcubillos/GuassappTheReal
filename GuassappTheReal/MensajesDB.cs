using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GuassappTheReal
{
	public class MensajesDB
	{
		private SQLiteConnection _connection;
		private const string yo = "56983362592";


		public MensajesDB ()
		{
			_connection = GetConnection ();
			_connection.CreateTable<Mensaje> ();
		}

		private SQLiteConnection GetConnection(){
			var fileName = "Mensajes.db3";
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			var path = Path.Combine (libraryPath, fileName);

			var connection = new SQLiteConnection (path);

			return connection;
		}

		public Mensaje AddMensaje(Mensaje m){
			
			_connection.Insert (m);
			return m;
		}

		public List<Mensaje> GetMensajes(){
			return (from t in _connection.Table<Mensaje>()
			        select t).ToList ();
		}

		public List<Mensaje> GetMensajesConIDConversacion(string id){
			return (from t in _connection.Table<Mensaje> () where (t.conversation_id == id)
				select t).ToList ();
		}

		public void DeleteMensaje(int key){
			_connection.Delete<Mensaje> (key);
		}

		public bool InDB (Mensaje m){
			return (_connection.Table<Mensaje> ().Contains (m));

		}

		public void DeleteAll(){
			_connection.DeleteAll<Mensaje> ();
		}
	}
}

