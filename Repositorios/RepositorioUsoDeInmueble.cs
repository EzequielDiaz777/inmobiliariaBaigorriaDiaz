using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioUsoDeInmueble
	{
		protected readonly string connectionString;

		public RepositorioUsoDeInmueble()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(UsoDeInmueble UsoDeInmueble)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"INSERT INTO {nameof(UsoDeInmueble)}({nameof(UsoDeInmueble.Nombre)}, {nameof(UsoDeInmueble.Estado)}) VALUES (@Nombre, 1);
				SELECT LAST_INSERT_ID();";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@Nombre", UsoDeInmueble.Nombre);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					UsoDeInmueble.IdUsoDeInmueble = res;
					conn.Close();
				}
			}
			return res;
		}

		public bool AltaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(UsoDeInmueble)} SET {nameof(UsoDeInmueble.Estado)} = 1 WHERE {nameof(UsoDeInmueble.IdUsoDeInmueble)} = @id;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					baja = cmd.ExecuteNonQuery() != 0;
					conn.Close();
				}
			}
			return baja;
		}

		public bool BajaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(UsoDeInmueble)} SET 
                            {nameof(UsoDeInmueble.Estado)} = 0 
                            WHERE {nameof(UsoDeInmueble.IdUsoDeInmueble)} = @id";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					baja = cmd.ExecuteNonQuery() != 0;
					conn.Close();
				}
			}
			return baja;
		}

		public int Modificacion(UsoDeInmueble UsoDeInmueble)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(UsoDeInmueble)} SET 
                            {nameof(UsoDeInmueble.IdUsoDeInmueble)} = @IdUsoDeInmueble,
							{nameof(UsoDeInmueble.Nombre)} = @Nombre
							WHERE {nameof(UsoDeInmueble.IdUsoDeInmueble)} = @IdUsoDeInmueble;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsoDeInmueble", UsoDeInmueble.IdUsoDeInmueble);
					cmd.Parameters.AddWithValue("@Nombre", UsoDeInmueble.Nombre);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			return res;
		}

		public List<UsoDeInmueble> ObtenerUsosDeInmuebles()
		{
			var res = new List<UsoDeInmueble>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
						{nameof(UsoDeInmueble.IdUsoDeInmueble)}, 
						{nameof(UsoDeInmueble.Nombre)},
						{nameof(UsoDeInmueble.Estado)}				
					FROM {nameof(UsoDeInmueble)}";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new UsoDeInmueble
							{
								IdUsoDeInmueble = reader.GetInt32("IdUsoDeInmueble"),
								Nombre = reader.GetString("Nombre"),
								Estado = reader.GetBoolean("Estado"),
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public UsoDeInmueble? ObtenerUsoDeInmueblePorID(int id)
		{
			//UsoDeInmueble? UsoDeInmueble = null;
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
							{nameof(UsoDeInmueble.IdUsoDeInmueble)}, 
							{nameof(UsoDeInmueble.Nombre)}, 
							{nameof(UsoDeInmueble.Estado)}
						FROM {nameof(UsoDeInmueble)} 
						WHERE {nameof(UsoDeInmueble.IdUsoDeInmueble)} = @id";
			UsoDeInmueble? usoDeInmueble = new UsoDeInmueble();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						usoDeInmueble = new UsoDeInmueble
						{
							IdUsoDeInmueble = reader.GetInt32("IdUsoDeInmueble"),
							Nombre = reader.GetString("Nombre"),
							Estado = reader.GetBoolean("Estado"),
						};
					}
				}
				conn.Close();
			}
			return usoDeInmueble;
		}
	}
}
