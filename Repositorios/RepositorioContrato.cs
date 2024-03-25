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
				var sql =
					@$"INSERT INTO {nameof(Contrato)}
						({nameof(Contrato.IdInquilino)}, 
						{nameof(Contrato.IdInmueble)}, 
						{nameof(Contrato.Precio)}, 
						{nameof(Contrato.AlquilerDesde)}, 
						{nameof(Contrato.AlquilerHasta)},
						{nameof(Contrato.AlquilerHastaOriginal)},
						{nameof(Contrato.Estado)}) 
					VALUES 
					(@IdInquilino, 
					@IdInmueble, 
					@Precio, 
					@AlquilerDesde, 
					@AlquilerHasta, 
					@AlquilerHastaOriginal, 
					1);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@Precio", contrato.Precio);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHastaOriginal", contrato.AlquilerHastaOriginal.ToDateTime(TimeOnly.MinValue));
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
				var sql = @$"DELETE FROM {nameof(Contrato)} WHERE {nameof(Contrato.IdContrato)} = @id";
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
				var sql = @$"UPDATE {nameof(Contrato)} SET {nameof(Contrato.Estado)} = 1 WHERE {nameof(Contrato.IdContrato)} = @id";
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
				var sql = @$"UPDATE {nameof(Contrato)} SET {nameof(Contrato.Estado)} = 0 WHERE {nameof(Contrato.IdContrato)} = @id";
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
				var sql =
					@$"UPDATE {nameof(Contrato)} SET 
						{nameof(Contrato.IdInquilino)} = @IdInquilino,
						{nameof(Contrato.IdInmueble)} = @IdInmueble,
						{nameof(Contrato.Precio)} = @Precio,
						{nameof(Contrato.AlquilerDesde)} = @AlquilerDesde,
						{nameof(Contrato.AlquilerHasta)} = @AlquilerHasta,
						{nameof(Contrato.AlquilerHastaOriginal)} = @AlquilerHastaOriginal,
						{nameof(Contrato.Estado)} = @Estado,
					WHERE 
						{nameof(Contrato.IdContrato)} = @IdContrato";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde);
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta);
					cmd.Parameters.AddWithValue("@AlquilerHastaOriginal", contrato.AlquilerHastaOriginal);
					cmd.Parameters.AddWithValue("@Estado", contrato.Estado);
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
				string sql =
							@$"SELECT 
								c.{nameof(Contrato.IdContrato)}, 
								c.{nameof(Contrato.Precio)}, 
								{nameof(Contrato.AlquilerDesde)}, 
								{nameof(Contrato.AlquilerHasta)},
								{nameof(Contrato.AlquilerHastaOriginal)},
								c.{nameof(Contrato.Estado)},
								inq.{nameof(Inquilino.Nombre)},
								inq.{nameof(Inquilino.Apellido)},
								inm.{nameof(Inmueble.Direccion)}
							FROM {nameof(Contrato)} AS c
							INNER JOIN {nameof(Inquilino)} AS inq
							ON c.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
							INNER JOIN {nameof(Inmueble)} AS inm
							ON c.{nameof(Contrato.IdInmueble)} = inm.{nameof(Inmueble.IdInmueble)}";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							res.Add(new Contrato
							{
								IdContrato = reader.GetInt32("IdContrato"),
								Precio = reader.GetDecimal("Precio"),
								AlquilerDesde = DateOnly.FromDateTime(reader.GetDateTime("AlquilerDesde")),
								AlquilerHasta = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHasta")),
								AlquilerHastaOriginal = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHastaOriginal")),
								Estado = reader.GetBoolean("Estado"),
								Inquilino = new Inquilino()
								{
									Nombre = reader.GetString("Nombre"),
									Apellido = reader.GetString("Apellido"),
								},
								Inmueble = new Inmueble()
								{
									Direccion = reader.GetString("Direccion")
								}
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
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								c.{nameof(Contrato.IdContrato)}, 
								c.{nameof(Contrato.Precio)}, 
								{nameof(Contrato.AlquilerDesde)}, 
								{nameof(Contrato.AlquilerHasta)},
								{nameof(Contrato.AlquilerHastaOriginal)},
								c.{nameof(Contrato.Estado)},
								inq.{nameof(Inquilino.Nombre)},
								inq.{nameof(Inquilino.Apellido)},
								inm.{nameof(Inmueble.Direccion)}
							FROM {nameof(Contrato)} AS c
							INNER JOIN {nameof(Inquilino)} AS inq
							ON c.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
							INNER JOIN {nameof(Inmueble)} AS inm
							ON c.{nameof(Contrato.IdInmueble)} = inm.{nameof(Inmueble.IdInmueble)}
							WHERE c.{nameof(Contrato.IdContrato)} = @id";
				Contrato contrato = new Contrato();
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							contrato = new Contrato
							{
								IdContrato = reader.GetInt32("IdContrato"),
								Precio = reader.GetDecimal("Precio"),
								AlquilerDesde = DateOnly.FromDateTime(reader.GetDateTime("AlquilerDesde")),
								AlquilerHasta = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHasta")),
								AlquilerHastaOriginal = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHastaOriginal")),
								Estado = reader.GetBoolean("Estado"),
								Inquilino = new Inquilino()
								{
									Nombre = reader.GetString("Nombre"),
									Apellido = reader.GetString("Apellido"),
								},
								Inmueble = new Inmueble()
								{
									Direccion = reader.GetString("Direccion")
								}
							};
						}
					}
					conn.Close();
				}
				Console.WriteLine(contrato.Inquilino.Apellido);
				return contrato;
			}
		}
	}
}