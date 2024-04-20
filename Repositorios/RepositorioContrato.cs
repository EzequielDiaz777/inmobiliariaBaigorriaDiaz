using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioContrato 
	{
		protected readonly string connectionString;
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
						{nameof(Contrato.AlquilerHastaOriginal)}
					VALUES 
						(@IdInquilino, 
						@IdInmueble, 
						@Precio, 
						@AlquilerDesde, 
						@AlquilerHasta, 
						@AlquilerHastaOriginal
						);
					SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@Precio", contrato.Precio);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHastaOriginal", contrato.AlquilerHasta.ToDateTime(TimeOnly.MinValue));
					conn.Open();
					cmd.ExecuteNonQuery();

					cmd.CommandText = "SELECT LAST_INSERT_ID()";
					res = Convert.ToInt32(cmd.ExecuteScalar());

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
					if (!baja)
					{
						throw new InvalidOperationException($"No se encontró ningún contrato con el ID {id} para eliminar.");
					}
				}
			}
			return baja;
		}

		public int ModificarContrato(Contrato contrato)
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
						{nameof(Contrato.AlquilerHastaOriginal)} = @AlquilerHastaOriginal
					WHERE 
						{nameof(Contrato.IdContrato)} = @IdContrato";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdContrato", contrato.IdContrato);
					cmd.Parameters.AddWithValue("@IdInquilino", contrato.IdInquilino);
					cmd.Parameters.AddWithValue("@IdInmueble", contrato.IdInmueble);
					cmd.Parameters.AddWithValue("@Precio", contrato.Precio);
					cmd.Parameters.AddWithValue("@AlquilerDesde", contrato.AlquilerDesde.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHasta", contrato.AlquilerHasta.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@AlquilerHastaOriginal", contrato.AlquilerHastaOriginal.ToDateTime(TimeOnly.MinValue));
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
								inq.{nameof(Inquilino.Nombre)},
								inq.{nameof(Inquilino.Apellido)},
								inm.{nameof(Inmueble.Direccion)}
							FROM {nameof(Contrato)} AS c
								INNER JOIN {nameof(Inquilino)} AS inq
									ON c.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
								INNER JOIN {nameof(Inmueble)} AS inm
									ON c.{nameof(Contrato.IdInmueble)} = inm.{nameof(Inmueble.IdInmueble)}
							ORDER BY c.AlquilerDesde DESC;";
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
								c.{nameof(Contrato.IdInquilino)},
								c.{nameof(Contrato.IdInmueble)},
								{nameof(Contrato.AlquilerDesde)}, 
								{nameof(Contrato.AlquilerHasta)},
								{nameof(Contrato.AlquilerHastaOriginal)},
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
								IdInmueble = reader.GetInt32("IdInmueble"),
								IdInquilino = reader.GetInt32("IdInquilino"),
								Precio = reader.GetDecimal("Precio"),
								AlquilerDesde = DateOnly.FromDateTime(reader.GetDateTime("AlquilerDesde")),
								AlquilerHasta = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHasta")),
								AlquilerHastaOriginal = DateOnly.FromDateTime(reader.GetDateTime("AlquilerHastaOriginal")),
								Inquilino = new Inquilino
								{
									IdInquilino = reader.GetInt32("IdInquilino"),
									Nombre = reader.GetString("Nombre"),
									Apellido = reader.GetString("Apellido"),
								},
								Inmueble = new Inmueble
								{
									IdInmueble = reader.GetInt32("IdInmueble"),
									Direccion = reader.GetString("Direccion"),
								}
							};
						}
					}
					conn.Close();
				}
				return contrato;
			}
		}

		public List<Contrato> ObtenerContratosPorDias(string dias)
		{
			var res = new List<Contrato>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								c.IdContrato,
								c.Precio,
								c.AlquilerDesde,
								c.AlquilerHasta,
								c.AlquilerHastaOriginal,
								inq.Nombre,
								inq.Apellido,
								inm.Direccion
							FROM Contrato AS c
								INNER JOIN Inquilino AS inq 
									ON c.IdInquilino = inq.IdInquilino
								INNER JOIN Inmueble AS inm 
									ON c.IdInmueble = inm.IdInmueble
							WHERE c.AlquilerHasta < DATE_ADD(NOW(), INTERVAL @dias DAY)
							ORDER BY c.AlquilerDesde DESC;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@dias", dias);
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

		public List<Contrato> ObtenerContratosPorFechas(string desde, string hasta)
		{
			var res = new List<Contrato>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								c.IdContrato,
								c.Precio,
								c.AlquilerDesde,
								c.AlquilerHasta,
								c.AlquilerHastaOriginal,
								inq.Nombre,
								inq.Apellido,
								inm.Direccion
							FROM Contrato AS c
								INNER JOIN Inquilino AS inq 
									ON c.IdInquilino = inq.IdInquilino
								INNER JOIN Inmueble AS inm 
									ON c.IdInmueble = inm.IdInmueble
							WHERE 
								c.AlquilerDesde >= @desde
								AND c.AlquilerHasta <= @hasta
							ORDER BY c.AlquilerDesde DESC;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@desde", desde);
					cmd.Parameters.AddWithValue("@hasta", hasta);
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

		public int ObtenerMesesTotales(int id){
			var res = 1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"
							SELECT 
								TIMESTAMPDIFF(MONTH, AlquilerDesde, AlquilerHasta) AS mesesTotales
							FROM contrato
							WHERE IdContrato = @id;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						if (reader.Read())
						{
							res += reader.GetInt32("mesesTotales");
						}
					conn.Close();
				}
			}
			return res;
		}

		public int ObtenerMesesPagados(int id)
		{
			var res = 0;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"
							SELECT 
								COUNT(p.IdContrato) AS cantidadDePagos
							FROM contrato AS c
								INNER JOIN pago AS p
								ON c.IdContrato = p.IdContrato
							WHERE c.IdContrato = @id
								AND p.Estado = 1;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						if (reader.Read())
						{
							res = reader.GetInt32("cantidadDePagos");
						}
					conn.Close();
				}
			}
			return res;
		}

		public int ObtenerMesesAdeudados(int id){
			var res = 1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"
							SELECT 
								TIMESTAMPDIFF(MONTH, c.AlquilerDesde, NOW()) AS mesesTotales,
								COUNT(p.IdContrato) AS cantidadDePagos
							FROM contrato AS c
								INNER JOIN pago AS p
								ON c.IdContrato = p.IdContrato
							WHERE c.IdContrato = @id
								AND p.Estado = 1;";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						if(reader.Read())
						{
							res += reader.GetInt32("mesesTotales");
							res -= reader.GetInt32("cantidadDePagos");
						}
					conn.Close();
				}
			}
			return res;
		}

		public List<Contrato> ObtenerContratosVigentes()
		{
			var res = new List<Contrato>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								c.IdContrato,
								c.Precio,
								c.AlquilerDesde,
								c.AlquilerHasta,
								c.AlquilerHastaOriginal,
								inq.Nombre,
								inq.Apellido,
								inm.Direccion
							FROM Contrato AS c
								INNER JOIN Inquilino AS inq 
									ON c.IdInquilino = inq.IdInquilino
								INNER JOIN Inmueble AS inm 
									ON c.IdInmueble = inm.IdInmueble
							WHERE c.AlquilerHasta > NOW()
							ORDER BY c.AlquilerDesde DESC;";
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

		public List<Contrato> ObtenerContratosNoVigentes()
		{
			var res = new List<Contrato>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								c.IdContrato,
								c.Precio,
								c.AlquilerDesde,
								c.AlquilerHasta,
								c.AlquilerHastaOriginal,
								inq.Nombre,
								inq.Apellido,
								inm.Direccion
							FROM Contrato AS c
								INNER JOIN Inquilino AS inq ON c.IdInquilino = inq.IdInquilino
								INNER JOIN Inmueble AS inm ON c.IdInmueble = inm.IdInmueble
							WHERE c.AlquilerHasta < NOW()
							ORDER BY c.AlquilerDesde DESC;";
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
	}
}