using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioContrato
	{
		protected readonly string connectionString;
		private RepositorioInmueble rInm = new RepositorioInmueble();
		private RepositorioInquilino rInq = new RepositorioInquilino();
		public RepositorioContrato()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Contrato contrato)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"INSERT INTO contrato(IdInquilino, IdInmueble, PrecioDeContrato, AlquilerDesde, AlquilerHasta, Estado) 
				VALUES (@IdInquilino, @IdInmueble, @PrecioDeContrato, @AlquilerDesde, @AlquilerHasta, @Estado);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@PrecioDeContrato", contrato.PrecioDeContrato);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@Estado", 1);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					Console.WriteLine(res);
					contrato.IdContrato = res;
					conn.Close();
					}
			}
			return res;
		}

		public bool BajaFisica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"DELETE FROM contrato WHERE IdContrato = @id";
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

		public bool AltaLogica(int id)
		{
			bool alta;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE contrato SET {nameof(Contrato.Estado)} = 1 WHERE IdContrato = @id";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					alta = cmd.ExecuteNonQuery() != 0;
					conn.Close();
				}
			}
			return alta;
		}

		public bool BajaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE contrato SET {nameof(Contrato.Estado)} = 0 WHERE IdContrato = @id";
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

		public int Modificacion(Contrato contrato)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @"UPDATE contrato SET 
				IdInquilino = @IdInquilino,
				IdInmueble = @IdInmueble,
				PrecioDeContrato = @PrecioDeContrato,
				AlquilerDesde = @AlquilerDesde,
				AlquilerHasta = @AlquilerHasta,
				Estado = @Estado,
				WHERE IdContrato = @IdContrato";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde);
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public List<Contrato> ObtenerContratos()
		{
			var res = new List<Contrato>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql = @$"SELECT 
								{nameof(Contrato.IdContrato)}, 
								{nameof(Contrato.IdInmueble)}, 
								{nameof(Contrato.PrecioDeContrato)},
								{nameof(Contrato.IdInquilino)}, 
								{nameof(Contrato.AlquilerDesde)}, 
								{nameof(Contrato.AlquilerHasta)},
								{nameof(Contrato.Estado)}
							FROM contrato;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							int IdInmueble = reader.GetInt32("IdInmueble");
							int IdInquilino = reader.GetInt32("IdInquilino");
							Console.WriteLine("OC IdInmueble: " + IdInmueble);
							Console.WriteLine("OC IdInquilino: "+IdInquilino);
							var inmueble = rInm.ObtenerInmueblePorID(IdInmueble);
							var inquilino = rInq.ObtenerInquilinoPorID(IdInquilino);
							res.Add(new Contrato
							{
								IdContrato = reader.GetInt32("IdContrato"),
								PrecioDeContrato = reader.GetDecimal("PrecioDeContrato"),
								AlquilerDesde = DateOnly.FromDateTime(reader.GetDateTime("AlquilerDesde")),
								AlquilerHasta = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHasta")),
								Estado = reader.GetBoolean("Estado"),
								Inmueble = inmueble,
								Inquilino = inquilino
							});
						}
					conn.Close();
				}
			}
			return res;
		}

		public Contrato ObtenerContratoById(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Contrato.IdContrato)}, 
						{nameof(Contrato.IdInmueble)}, 
						{nameof(Contrato.IdInquilino)}, 
						{nameof(Contrato.PrecioDeContrato)},
						{nameof(Contrato.AlquilerDesde)}, 
						{nameof(Contrato.AlquilerHasta)},
						{nameof(Contrato.Estado)}
					FROM contrato
					WHERE IdContrato = @id;";
			Contrato contrato = new Contrato();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						int IdInmueble = reader.GetInt32("IdInmueble");
						int IdInquilino = reader.GetInt32("IdInquilino");
						var inmueble = rInm.ObtenerInmueblePorID(IdInmueble);
						var inquilino = rInq.ObtenerInquilinoPorID(IdInquilino);
						contrato = new Contrato
						{
							IdContrato = reader.GetInt32("IdContrato"),
							AlquilerDesde = DateOnly.FromDateTime(reader.GetDateTime("AlquilerDesde")),
							AlquilerHasta = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHasta")),
							Inmueble = inmueble,
							Inquilino = inquilino
						};
					}
				}
				conn.Close();
			}
			return contrato;
		}
	}
}