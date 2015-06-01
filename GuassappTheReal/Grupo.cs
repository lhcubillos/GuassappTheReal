using System;
using SQLite;
using System.Collections.Generic;

namespace GuassappTheReal
{
	public class Grupo
	{
		
		[PrimaryKey,AutoIncrement]
		int Key{ get; set;}

		public string id{ get; set;}
		public string admin{ get; set;}
		public bool group{ get; set; }
		public string title{ get; set;}

		public Grupo ()//string id, string admin, bool group, string title)
		{
			//this.id = id;
			//this.admin = admin;
			//this.group = group;
			//this.title = title;
		}
			
		public override bool Equals(object obj)
		{
			var other = obj as Grupo;
			if (other == null)
				return false;
			return (this.title == other.title);
		}

		public override int GetHashCode()
		{
			return title.GetHashCode();
		}
		public override string ToString ()
		{
			return string.Format ("[Grupo: admin={0}, title={1}, group={2}]", admin, title,group);
		}
	}
}

