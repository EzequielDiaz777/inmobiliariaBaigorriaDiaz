using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{

	public class RepositorioInquilino
	{

		protected readonly string connectionString;
		public RepositorioInquilino()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Inquilino inquilino)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = 
						@$"INSERT INTO {nameof(Inquilino)}
							({nameof(Inquilino.Nombre)},
							{nameof(Inquilino.Apellido)},
							{nameof(Inquilino.Telefono)},
							{nameof(Inquilino.Email)},
							{nameof(Inquilino.DNI)},
							{nameof(Inquilino.Estado)})
						VALUES
							(@Nombre, 
							@Apellido, 
							@Telefono, 
							@Email, 
							@DNI, 
							1);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
					cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(inquilino.Email) ? "" : inquilino.Email);
					cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(inquilino.Telefono) ? "" : inquilino.Telefono);
					cmd.Parameters.AddWithValue("@DNI", inquilino.DNI);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					inquilino.IdInquilino = res;
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
				var sql = @$"UPDATE {nameof(Inquilino)} SET {nameof(Inquilino.Estado)} = 1 WHERE {nameof(Inquilino.IdInquilino)} = @$id";
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
				var sql = @$"DELETE FROM {nameof(Inquilino)} WHERE {nameof(Inquilino.IdInquilino)} = @id";
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
				var sql = @$"UPDATE {nameof(Inquilino)} SET {nameof(Inquilino.Estado)} = 0 WHERE {nameof(Inquilino.IdInquilino)} = @id";
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

		public int Modificacion(Inquilino inquilino)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Inquilino)} SET 
							{nameof(Inquilino.Nombre)} = @Nombre, 
							{nameof(Inquilino.Apellido)} = @Apellido, 
							{nameof(Inquilino.Telefono)} = @Telefono, 
							{nameof(Inquilino.Email)} = @Email, 
							{nameof(Inquilino.DNI)} = @DNI
							WHERE {nameof(Inquilino.IdInquilino)} = @IdInquilino";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdInquilino", inquilino.IdInquilino);
					cmd.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
					cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(inquilino.Email) ? "" : inquilino.Email);
					cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(inquilino.Telefono) ? "" : inquilino.Telefono);
					cmd.Parameters.AddWithValue("@DNI", inquilino.DNI);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public List<Inquilino> ObtenerInquilinos()
		{
			var res = new List<Inquilino>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
						{nameof(Inquilino.IdInquilino)}, 
						{nameof(Inquilino.Nombre)}, 
						{nameof(Inquilino.Apellido)}, 
						{nameof(Inquilino.Telefono)}, 
						{nameof(Inquilino.Email)}, 
						{nameof(Inquilino.DNI)}, 
						{nameof(Inquilino.Estado)} 
					FROM {nameof(Inquilino)}";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Inquilino
							{
								IdInquilino = reader.GetInt32("IdInquilino"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
								Telefono = reader["Telefono"] != DBNull.Value ? reader.GetString("Telefono") : "",
								Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
								DNI = reader.GetString("DNI"),
								Estado = reader.GetBoolean("Estado")
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Inquilino ObtenerInquilinoPorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Inquilino.IdInquilino)}, 
						{nameof(Inquilino.Nombre)}, 
						{nameof(Inquilino.Apellido)}, 
						{nameof(Inquilino.Telefono)}, 
						{nameof(Inquilino.Email)}, 
						{nameof(Inquilino.DNI)}, 
						{nameof(Inquilino.Estado)} 
					FROM {nameof(Inquilino)} 
					WHERE {nameof(Inquilino.IdInquilino)} = @id";
			Inquilino inquilino = new Inquilino();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						inquilino = new Inquilino
						{
							IdInquilino = reader.GetInt32("IdInquilino"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Telefono = reader["Telefono"] != DBNull.Value ? reader.GetString("Telefono") : "",
							Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
							DNI = reader.GetString("DNI"),
							Estado = reader.GetBoolean("Estado")
						};
					}
				}
				conn.Close();
			}
			return inquilino;
		}

		public Inquilino? ObtenerInquilinoPorDNI(int dni)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Inquilino.IdInquilino)}, 
						{nameof(Inquilino.Nombre)}, 
						{nameof(Inquilino.Apellido)}, 
						{nameof(Inquilino.Telefono)}, 
						{nameof(Inquilino.Email)}, 
						{nameof(Inquilino.DNI)}, 
						{nameof(Inquilino.Estado)} 
					FROM {nameof(Inquilino)} 
					WHERE {nameof(Inquilino.DNI)} = @dni";
			Inquilino? inquilino = null;
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@dni", dni);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						inquilino = new Inquilino
						{
							IdInquilino = reader.GetInt32("IdInquilino"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Telefono = reader["Telefono"] != DBNull.Value ? reader.GetString("Telefono") : "",
							Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
							DNI = reader.GetString("DNI"),
							Estado = reader.GetBoolean("Estado")
						};
					}
				}
				conn.Close();
			}
			return inquilino;
		}
	}
}
