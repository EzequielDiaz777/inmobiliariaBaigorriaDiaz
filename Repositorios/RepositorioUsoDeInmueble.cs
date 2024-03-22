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
				var sql = @"INSERT INTO UsoDeInmueble(Uso, Estado) VALUES (@Uso, 1);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@Uso", UsoDeInmueble.Uso);
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
				var sql = @"UPDATE UsoDeInmueble SET Estado = 1 FROM UsoDeInmueble WHERE IdUsoDeInmueble = @id";
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
				var sql = @"UPDATE UsoDeInmueble SET 
                            Estado = 0 
                            WHERE IdUsoDeInmueble = @id";
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
				var sql = @"UPDATE UsoDeInmueble SET 
                            IdUsoDeInmueble = @IdUsoDeInmueble,
							Uso = @Uso,
                            Estado = @Estado
							WHERE IdUsoDeInmueble = @IdUsoDeInmueble;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsoDeInmueble", UsoDeInmueble.IdUsoDeInmueble);
					cmd.Parameters.AddWithValue("@Uso", UsoDeInmueble.Uso);
					cmd.Parameters.AddWithValue("@Estado", UsoDeInmueble.Estado);
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
						{nameof(UsoDeInmueble.Uso)},
						{nameof(UsoDeInmueble.Estado)}				
					FROM UsoDeInmueble";
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
								Uso = reader.GetString("Uso"),
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
							{nameof(UsoDeInmueble.Uso)}, 
							{nameof(UsoDeInmueble.Estado)}
						FROM UsoDeInmueble 
						WHERE IdUsoDeInmueble = @id";
			UsoDeInmueble usoDeInmueble = new UsoDeInmueble();
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
							Uso = reader.GetString("Uso"),
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
