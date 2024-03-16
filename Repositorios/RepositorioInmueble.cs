using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioInmueble
	{
		protected readonly string connectionString;
		private RepositorioPropietario rp = new RepositorioPropietario();
		private RepositorioTipoDeInmueble rtdi = new RepositorioTipoDeInmueble();

		public RepositorioInmueble()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Inmueble inmueble)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"INSERT INTO inmueble(IdPropietario, IdTipoDeInmueble, Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, Estado) 
				VALUES (@IdPropietario, @IdTipoDeInmueble, @Direccion, @Ambientes, @Superficie, @Latitud, @Longitud, @Precio, @Estado);
				SELECT LAST_INSERT_ID();";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
					cmd.Parameters.AddWithValue("@IdTipoDeInmueble", inmueble.IdTipoDeInmueble);
					cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
					cmd.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
					cmd.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
					cmd.Parameters.AddWithValue("@Latitud", string.IsNullOrEmpty(string.Format("inmueble.Latitud")) ? 0 : inmueble.Latitud);
					cmd.Parameters.AddWithValue("@Longitud", string.IsNullOrEmpty(string.Format("inmueble.Longitud")) ? 0 : inmueble.Longitud);
					cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
					cmd.Parameters.AddWithValue("@Estado", inmueble.Estado);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					inmueble.IdInmueble = res;
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
				var sql = @"UPDATE inmueble SET Estado = 1 FROM inmueble WHERE IdInmueble = @id";
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

		public bool BajaFisica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"DELETE FROM inmueble WHERE IdInmueble = @id";
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

		public bool BajaFisicaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"UPDATE inmueble SET Estado = 0 FROM inmueble WHERE IdInmueble = @id";
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

		public int Modificacion(Inmueble inmueble)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"UPDATE inmueble SET 
							IdTipoDeInmueble = @IdTipoDeInmueble,
							Direccion = @Direccion,
							Ambientes = @Ambientes,
							Superficie = @Superficie,
							Latitud = @Latitud,
							Longitud = @Longitud,
							Precio = @Precio,
							Estado = @Estado,
							IdPropietario = @IdPropietario
							WHERE IdInmueble = @IdInmueble";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
					cmd.Parameters.AddWithValue("@IdTipoDeInmueble", inmueble.IdTipoDeInmueble);
					cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
					cmd.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
					cmd.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
					cmd.Parameters.AddWithValue("@Latitud", string.IsNullOrEmpty(string.Format("inmueble.Latitud")) ? 0 : inmueble.Latitud);
					cmd.Parameters.AddWithValue("@Longitud", string.IsNullOrEmpty(string.Format("inmueble.Longitud")) ? 0 : inmueble.Longitud);
					cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
					cmd.Parameters.AddWithValue("@Estado", inmueble.Estado);
					cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
					Console.WriteLine(inmueble.Estado);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			return res;
		}


		public List<Inmueble> ObtenerInmuebles()
		{
			var res = new List<Inmueble>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
							IdInmueble, 
							{nameof(Inmueble.IdTipoDeInmueble)}, 
							{nameof(Inmueble.Direccion)}, 
							{nameof(Inmueble.Ambientes)}, 
							{nameof(Inmueble.Superficie)}, 
							{nameof(Inmueble.Latitud)}, 
							{nameof(Inmueble.Longitud)}, 
							{nameof(Inmueble.Precio)}, 
							{nameof(Inmueble.Estado)}, 
							{nameof(Inmueble.IdPropietario)} 
							FROM inmueble;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							int IdPropietario = reader.GetInt32("IdPropietario");
							int IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble");
							var tipoDeInmueble = rtdi.ObtenerTipoDeInmueblePorID(IdTipoDeInmueble);
							var propietario = rp.ObtenerPropietarioPorID(IdPropietario);
							res.Add(new Inmueble
							{
								IdInmueble = reader.GetInt32("IdInmueble"),
								IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble"),
								TipoDeInmueble = tipoDeInmueble,
								Direccion = reader.GetString("Direccion"),
								Ambientes = reader.GetInt32("Ambientes"),
								Superficie = reader.GetDecimal("Superficie"),
								Latitud = reader["Latitud"] != DBNull.Value ? reader.GetString("Latitud") : "",
								Longitud = reader["Longitud"] != DBNull.Value ? reader.GetString("Longitud") : "",
								Precio = reader.GetDecimal("Precio"),
								Estado = reader.GetBoolean("Estado"),
								IdPropietario = reader.GetInt32("IdPropietario"),
								Duenio = propietario
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerInmueblePorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Inmueble.IdTipoDeInmueble)}, 
						{nameof(Inmueble.Direccion)}, 
						{nameof(Inmueble.Ambientes)}, 
						{nameof(Inmueble.Superficie)}, 
						{nameof(Inmueble.Latitud)}, 
						{nameof(Inmueble.Longitud)}, 
						{nameof(Inmueble.Precio)}, 
						{nameof(Inmueble.Estado)}, 
						{nameof(Inmueble.IdPropietario)} 
					FROM inmueble 
					WHERE IdInmueble = @id";
			Inmueble inmueble = new Inmueble();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						int IdPropietario = reader.GetInt32("IdPropietario");
						int IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble");
						var tipoDeInmueble = rtdi.ObtenerTipoDeInmueblePorID(IdTipoDeInmueble);
						var propietario = new Propietario();
						propietario = rp.ObtenerPropietarioPorID(IdPropietario);
						inmueble = new Inmueble
						{
							IdInmueble = reader.GetInt32("IdInmueble"),
							TipoDeInmueble = tipoDeInmueble,
							Direccion = reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetDecimal("Superficie"),
							Latitud = reader["Latitud"] != DBNull.Value ? reader.GetString("Latitud") : "",
							Longitud = reader["Longitud"] != DBNull.Value ? reader.GetString("Longitud") : "",
							Precio = reader.GetDecimal("Precio"),
							Estado = reader.GetBoolean("Estado"),
							IdPropietario = reader.GetInt32("IdPropietario"),
							Duenio = propietario
						};
					}
				}
				conn.Close();
			}
			return inmueble;
		}
	}
}