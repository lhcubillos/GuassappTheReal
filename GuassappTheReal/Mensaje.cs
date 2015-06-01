using System;
using SQLite;
using Newtonsoft.Json;

namespace GuassappTheReal
{
	public class Mensaje
	{
		[PrimaryKey,AutoIncrement]
		int key{ get; set;}

		public string sender{ get; set;}
		public string id {get;set;}
		public string conversation_id{ get; set; }
		public string content{ get; set;}
		public string created_at{ get; set;}

		public Mensaje ()
		{
		}

		public override string ToString ()
		{
			return string.Format ("[Mensaje: sender={0},  content={1}, created_at={2},id = {3}, conversation_id={4}]", sender, content, created_at,id, conversation_id);
		}
		public override bool Equals(object obj)
		{
			var other = obj as Mensaje;
			if (other == null)
				return false;
			return (this.sender == other.sender && this.content == other.content && other.created_at == this.created_at && this.conversation_id == other.conversation_id);
		}

		public override int GetHashCode()
		{
			return sender.GetHashCode() ^ conversation_id.GetHashCode() ^ content.GetHashCode() ^ created_at.GetHashCode();
		}

	}
}

