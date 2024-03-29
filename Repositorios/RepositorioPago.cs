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
						{nameof(Pago.Fecha)}
					VALUES 
						(@IdContrato, 
						@Monto, 
						@Fecha);
					SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
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
				var sql = @$"UPDATE {nameof(Pago)} 
				WHERE {nameof(Pago.NumeroDePago)} = @id";
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
				var sql = @$"UPDATE {nameof(Pago)} 
				WHERE {nameof(Pago.NumeroDePago)} = @id";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@id", id);
					conn.Open();
					baja = cmd.ExecuteNonQuery() != 0;
					conn.Close();
				}
				if (!baja)
				{
					throw new InvalidOperationException($"No se encontró ningún pago con el ID {id} para dar de baja.");
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
						{nameof(Pago.IdContrato)} = @IdContrato,
						{nameof(Pago.Monto)} = @Monto,
						{nameof(Pago.Fecha)} = @Fecha
					WHERE 
						{nameof(Pago.NumeroDePago)} = @NumeroDePago";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@NumeroDePago", pago.NumeroDePago);
					cmd.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
					cmd.Parameters.AddWithValue("@Monto", pago.Monto);
					cmd.Parameters.AddWithValue("@Fecha", pago.Fecha);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public List<Pago> ObtenerPagos()
		{
			var res = new List<Pago>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								{nameof(Pago.NumeroDePago)}, 
								{nameof(Pago.Monto)}, 
								{nameof(Pago.Fecha)},
								cont.{nameof(Contrato.IdContrato)}
							FROM {nameof(Pago)} AS pag
							INNER JOIN {nameof(Contrato)} AS cont
							ON pag.{nameof(Pago.IdContrato)} = cont.{nameof(Contrato.IdContrato)}";
	
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
						while (reader.Read())
						{
							res.Add(new Pago
							{
								NumeroDePago = reader.GetInt32("NumeroDePago"),
								Monto = reader.GetDecimal("Monto"),
								Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")),
								Contrato = new Contrato()
								{
									IdContrato = reader.GetInt32("IdContrato")
								}
							});
						}
					conn.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPagoById(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				string sql =
							@$"SELECT 
								{nameof(Pago.NumeroDePago)}, 
								{nameof(Pago.Monto)}, 
								{nameof(Pago.Fecha)}, 
								cont.{nameof(Contrato.IdContrato)},
							FROM {nameof(Pago)}
							INNER JOIN {nameof(Contrato)} AS cont
							ON {nameof(Pago.IdContrato)} = cont.{nameof(Pago.IdContrato)}
							WHERE {nameof(Pago.NumeroDePago)} = @id";
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
								Monto = reader.GetDecimal("Monto"),
								Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")),	
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