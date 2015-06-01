using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace GuassappTheReal
{
	public class GruposDB
	{
		private SQLiteConnection _connection;
		private const string yo = "56983362592";


		public GruposDB ()
		{
			_connection = GetConnection ();
			_connection.CreateTable<Grupo> ();
		}

		private SQLiteConnection GetConnection(){
			var fileName = "Grupos.db3";
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			var path = Path.Combine (libraryPath, fileName);

			var connection = new SQLiteConnection (path);

			return connection;
		}

		public Grupo AddGrupo(Grupo m){

			_connection.Insert (m);
			return m;
		}

		public List<Grupo> GetGrupos(){
			return (from t in _connection.Table<Grupo>()
				select t).ToList ();
		}
			

		public void DeleteGrupo(int key){
			_connection.Delete<Grupo> (key);
		}

		public void DeleteAll(){
			_connection.DeleteAll<Grupo> ();
		}
		public int Largo(){
			return _connection.Table<Grupo> ().Count();
		}

		//public string[] GetIntegrantesGrupo(string id){
		//	return (_connection.Get<Grupo> (g => g.id = id).Integrantes).ToArray();
		//}
	
	}
}

