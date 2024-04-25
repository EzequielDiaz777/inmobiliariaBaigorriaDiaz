using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class RepositorioPago
	{
		protected readonly string connectionString;
		public RepositorioPago()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Pago pago)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
					@$"INSERT INTO {nameof(Pago)}
						({nameof(Pago.IdContrato)},  
						{nameof(Pago.Monto)}, 
						{nameof(Pago.MesDePago)},
						{nameof(Pago.Fecha)},
						{nameof(Pago.Estado)})
					VALUES 
						(@IdContrato, 
						@Monto, 
						@MesDePago,
						@Fecha,
						1);
					SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
					cmd.Parameters.AddWithValue("@MesDePago", pago.MesDePago);
					cmd.Parameters.AddWithValue("@Monto", pago.Monto);
					cmd.Parameters.AddWithValue("@Fecha", pago.Fecha.ToDateTime(TimeOnly.MinValue));
					conn.Open();
					cmd.ExecuteNonQuery();

					cmd.CommandText = "SELECT LAST_INSERT_ID()";
					res = Convert.ToInt32(cmd.ExecuteScalar());

					pago.NumeroDePago = res;
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
				var sql = @$"DELETE FROM {nameof(Pago)} WHERE {nameof(Pago.NumeroDePago)} = @id";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					baja = cmd.ExecuteNonQuery() != 0;
					conn.Close();
					if (!baja)
					{
						throw new InvalidOperationException($"No se encontró ningún pago con el ID {id} para eliminar.");
					}
				}
			}
			return baja;
		}

		public bool AltaLogica(int id)
		{
			bool alta;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Pago)} SET {nameof(Pago.Estado)} = 1 WHERE {nameof(Pago.NumeroDePago)} = @id";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					alta = cmd.ExecuteNonQuery() != 0;
					conn.Close();
				}
				if (!alta)
				{
					throw new InvalidOperationException($"No se encontró ningún pago con el ID {id} para dar de alta.");
				}
			}
			return alta;
		}

		public bool BajaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Pago)} SET {nameof(Pago.Estado)} = 0 WHERE {nameof(Pago.NumeroDePago)} = @id";
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

		public int ModificarPago(Pago pago)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
					@$"UPDATE {nameof(Pago)} SET 
						{nameof(Pago.MesDePago)} = @MesDePago
					WHERE 
						{nameof(Pago.NumeroDePago)} = @NumeroDePago";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@MesDePago", pago.MesDePago);
					cmd.Parameters.AddWithValue("@NumeroDePago", pago.NumeroDePago);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				Console.WriteLine(res);
				return res;
			}
		}

		public int ObtenerCantidadDeFilas()
		{
			var res = 0;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT COUNT({nameof(Pago.NumeroDePago)})
							FROM {nameof(Pago)};";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							res = reader.GetInt32("COUNT(NumeroDePago)");
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public List<Pago> ObtenerPagos(int limite, int paginado)
		{
			var res = new List<Pago>();
			int offset = (paginado - 1) * limite;

			if (offset < 0)
			{
				offset = 0;
			}
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								pag.{nameof(Pago.NumeroDePago)}, 
								pag.{nameof(Pago.MesDePago)},
								pag.{nameof(Pago.Monto)}, 
								pag.{nameof(Pago.Fecha)},
								pag.{nameof(Pago.Estado)},
								cont.{nameof(Contrato.IdContrato)},
								inq.{nameof(Inquilino.Apellido)} AS InquilinoApellido,
								prop.{nameof(Propietario.Apellido)} AS PropietarioApellido
							FROM {nameof(Pago)} AS pag
							INNER JOIN {nameof(Contrato)} AS cont 
							ON pag.{nameof(Pago.IdContrato)} = cont.{nameof(Contrato.IdContrato)}
							INNER JOIN {nameof(Inquilino)} AS inq 
							ON cont.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
							INNER JOIN {nameof(Inmueble)} AS inm 
							ON cont.{nameof(Contrato.IdInmueble)} = inm.{nameof(Inmueble.IdInmueble)}
							INNER JOIN {nameof(Propietario)} AS prop 
							ON inm.{nameof(Inmueble.IdPropietario)} = prop.{nameof(Propietario.IdPropietario)}
							LIMIT {limite} OFFSET {offset}";

				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Pago
							{
								NumeroDePago = reader.GetInt32("NumeroDePago"),
								IdContrato = reader.GetInt32("IdContrato"),
								MesDePago = reader.GetString("MesDePago"),
								Monto = reader.GetDecimal("Monto"),
								Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")),
								Estado = reader.GetBoolean("Estado"),
								Contrato = new Contrato
								{
									Inquilino = new Inquilino
									{
										Apellido = reader.GetString("InquilinoApellido")
									},
									Inmueble = new Inmueble
									{
										Duenio = new Propietario
										{
											Apellido = reader.GetString("PropietarioApellido")
										}
									}
								}
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public List<Pago> ObtenerPagos()
		{
			var res = new List<Pago>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								pag.{nameof(Pago.NumeroDePago)}, 
								pag.{nameof(Pago.MesDePago)},
								pag.{nameof(Pago.Monto)}, 
								pag.{nameof(Pago.Fecha)},
								pag.{nameof(Pago.Estado)},
								cont.{nameof(Contrato.IdContrato)},
								inq.{nameof(Inquilino.Apellido)} AS InquilinoApellido,
								prop.{nameof(Propietario.Apellido)} AS PropietarioApellido
							FROM {nameof(Pago)} AS pag
							INNER JOIN {nameof(Contrato)} AS cont 
							ON pag.{nameof(Pago.IdContrato)} = cont.{nameof(Contrato.IdContrato)}
							INNER JOIN {nameof(Inquilino)} AS inq 
							ON cont.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
							INNER JOIN {nameof(Inmueble)} AS inm 
							ON cont.{nameof(Contrato.IdInmueble)} = inm.{nameof(Inmueble.IdInmueble)}
							INNER JOIN {nameof(Propietario)} AS prop 
							ON inm.{nameof(Inmueble.IdPropietario)} = prop.{nameof(Propietario.IdPropietario)}";

				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Pago
							{
								NumeroDePago = reader.GetInt32("NumeroDePago"),
								IdContrato = reader.GetInt32("IdContrato"),
								MesDePago = reader.GetString("MesDePago"),
								Monto = reader.GetDecimal("Monto"),
								Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")),
								Estado = reader.GetBoolean("Estado"),
								Contrato = new Contrato
								{
									Inquilino = new Inquilino
									{
										Apellido = reader.GetString("InquilinoApellido")
									},
									Inmueble = new Inmueble
									{
										Duenio = new Propietario
										{
											Apellido = reader.GetString("PropietarioApellido")
										}
									}
								}
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPagoPorId(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								{nameof(Pago.NumeroDePago)}, 
								{nameof(Pago.MesDePago)},
								{nameof(Pago.Monto)}, 
								{nameof(Pago.Fecha)}, 
								p.{nameof(Pago.Estado)},
								cont.{nameof(Contrato.IdContrato)}
							FROM {nameof(Pago)} AS p
							INNER JOIN {nameof(Contrato)} AS cont
							ON p.{nameof(Pago.IdContrato)} = cont.{nameof(Pago.IdContrato)}
							WHERE p.{nameof(Pago.NumeroDePago)} = @id";
				Pago pago = new Pago();
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							pago = new Pago
							{
								NumeroDePago = reader.GetInt32("NumeroDePago"),
								IdContrato = reader.GetInt32("IdContrato"),
								MesDePago = reader.GetString("MesDePago"),
								Monto = reader.GetDecimal("Monto"),
								Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")),
								Estado = reader.GetBoolean("Estado"),
							};
						}
					}
					conn.Close();
				}
				return pago;
			}
		}
	}
}