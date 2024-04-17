using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{

	public class RepositorioPropietario
	{

		protected readonly string connectionString;
		public RepositorioPropietario()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Propietario propietario)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
						@$"INSERT INTO {nameof(Propietario)}
							({nameof(Propietario.Nombre)},
							{nameof(Propietario.Apellido)},
							{nameof(Propietario.Telefono)},
							{nameof(Propietario.Email)},
							{nameof(Propietario.DNI)},
							{nameof(Propietario.Estado)})
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
					cmd.Parameters.AddWithValue("@Nombre", propietario.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", propietario.Apellido);
					cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(propietario.Email) ? "" : propietario.Email);
					cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(propietario.Telefono) ? "" : propietario.Telefono);
					cmd.Parameters.AddWithValue("@DNI", propietario.DNI);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					propietario.IdPropietario = res;
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
				var sql = @$"UPDATE {nameof(Propietario)} SET {nameof(Propietario.Estado)} = 1 WHERE {nameof(Propietario.IdPropietario)} = @id";
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
				var sql = @$"DELETE FROM {nameof(Propietario)} WHERE {nameof(Propietario.IdPropietario)} = @id";
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
				var sql = @$"UPDATE {nameof(Propietario)} SET {nameof(Propietario.Estado)} = 0 WHERE {nameof(Propietario.IdPropietario)} = @id";
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

		public int Modificacion(Propietario propietario)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Propietario)} SET 
							{nameof(Propietario.Nombre)} = @Nombre, 
							{nameof(Propietario.Apellido)} = @Apellido, 
							{nameof(Propietario.Telefono)} = @Telefono, 
							{nameof(Propietario.Email)} = @Email, 
							{nameof(Propietario.DNI)} = @DNI
							WHERE {nameof(Propietario.IdPropietario)} = @IdPropietario";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdPropietario", propietario.IdPropietario);
					cmd.Parameters.AddWithValue("@Nombre", propietario.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", propietario.Apellido);
					cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(propietario.Email) ? "" : propietario.Email);
					cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(propietario.Telefono) ? "" : propietario.Telefono);
					cmd.Parameters.AddWithValue("@DNI", propietario.DNI);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public List<Propietario> ObtenerPropietarios()
		{
			var res = new List<Propietario>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
						{nameof(Propietario.IdPropietario)}, 
						{nameof(Propietario.Nombre)}, 
						{nameof(Propietario.Apellido)}, 
						{nameof(Propietario.Telefono)}, 
						{nameof(Propietario.Email)}, 
						{nameof(Propietario.DNI)}, 
						{nameof(Propietario.Estado)} 
					FROM {nameof(Propietario)}";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Propietario
							{
								IdPropietario = reader.GetInt32("IdPropietario"),
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

		public Propietario ObtenerPropietarioPorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Propietario.IdPropietario)}, 
						{nameof(Propietario.Nombre)}, 
						{nameof(Propietario.Apellido)}, 
						{nameof(Propietario.Telefono)}, 
						{nameof(Propietario.Email)}, 
						{nameof(Propietario.DNI)}, 
						{nameof(Propietario.Estado)} 
					FROM {nameof(Propietario)} 
					WHERE {nameof(Propietario.IdPropietario)} = @id";
			Propietario propietario = new Propietario();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						propietario = new Propietario
						{
							IdPropietario = reader.GetInt32("IdPropietario"),
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
			return propietario;
		}
	}
}
