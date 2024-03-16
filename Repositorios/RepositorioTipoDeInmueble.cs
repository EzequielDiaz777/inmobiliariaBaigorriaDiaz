using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioTipoDeInmueble
	{
		protected readonly string connectionString;

		public RepositorioTipoDeInmueble()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(TipoDeInmueble tipoDeInmueble)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"INSERT INTO tipodeinmueble(Tipo, Estado) VALUES (@Tipo, @Estado);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@Tipo", tipoDeInmueble.Tipo);
					cmd.Parameters.AddWithValue("@Estado", tipoDeInmueble.Estado);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					tipoDeInmueble.IdTipoDeInmueble = res;
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
				var sql = @"UPDATE tipodeinmueble SET Estado = 1 FROM tipodeinmueble WHERE IdTipoDeInmueble = @id";
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
				var sql = @"UPDATE tipodeinmueble SET 
                            Estado = 0 
                            WHERE IdTipoDeInmueble = @id";
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

		public int Modificacion(TipoDeInmueble tipoDeInmueble)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"UPDATE tipodeinmueble SET 
                            IdTipoDeInmueble = @IdTipoDeInmueble,
							Tipo = @Tipo,
                            Estado = @Estado
							WHERE IdTipoDeInmueble = @IdTipoDeInmueble;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdTipoDeInmueble", tipoDeInmueble.IdTipoDeInmueble);
					cmd.Parameters.AddWithValue("@Tipo", tipoDeInmueble.Tipo);
					cmd.Parameters.AddWithValue("@Estado", tipoDeInmueble.Estado);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			return res;
		}

		public List<TipoDeInmueble> ObtenerTiposDeInmuebles()
		{
			var res = new List<TipoDeInmueble>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
						{nameof(TipoDeInmueble.IdTipoDeInmueble)}, 
						{nameof(TipoDeInmueble.Tipo)},
						{nameof(TipoDeInmueble.Estado)}				
					FROM tipodeinmueble";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new TipoDeInmueble
							{
								IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble"),
								Tipo = reader.GetString("Tipo"),
								Estado = reader.GetBoolean("Estado"),
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public TipoDeInmueble ObtenerTipoDeInmueblePorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
							{nameof(TipoDeInmueble.IdTipoDeInmueble)}, 
							{nameof(TipoDeInmueble.Tipo)}, 
							{nameof(TipoDeInmueble.Estado)}
						FROM tipodeinmueble 
						WHERE IdTipoDeInmueble = @id";
			TipoDeInmueble tipoDeInmueble = new TipoDeInmueble();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						tipoDeInmueble = new TipoDeInmueble
						{
							IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble"),
							Tipo = reader.GetString("Tipo"),
							Estado = reader.GetBoolean("Estado"),
						};
					}
				}
				conn.Close();
			}
			return tipoDeInmueble;
		}
	}
}
