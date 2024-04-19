using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{

	public class RepositorioUsuario
	{

		protected readonly string connectionString;
		public RepositorioUsuario()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Usuario usuario)
		{
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
						@$"INSERT INTO {nameof(Usuario)}
						(
							{nameof(Usuario.Nombre)},
							{nameof(Usuario.Apellido)},
							{nameof(Usuario.Email)},
							{nameof(Usuario.Clave)},
							{nameof(Usuario.Rol)},
							{nameof(Usuario.AvatarURL)},
							{nameof(Usuario.Estado)}
						) VALUES (
							@Nombre, 
							@Apellido, 
							@Email, 
							@Clave,
							@Rol,
							@AvatarURL,
							1);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
					cmd.Parameters.AddWithValue("@Email", usuario.Email);
					cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
					cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
					cmd.Parameters.AddWithValue("@AvatarURL", string.IsNullOrEmpty(usuario.AvatarURL) ? (object)DBNull.Value : usuario.AvatarURL);
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					usuario.IdUsuario = res;
					conn.Close();
				}
			}
			Console.WriteLine("Sali de alta");
			return res;
		}

		public bool AltaLogica(int id)
		{
			bool baja;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Usuario)} SET {nameof(Usuario.Estado)} = 1 WHERE {nameof(Usuario.IdUsuario)} = @id";
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
				var sql = @$"DELETE FROM {nameof(Usuario)} WHERE {nameof(Usuario.IdUsuario)} = @id";
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
				var sql = @$"UPDATE {nameof(Usuario)} SET {nameof(Usuario.Estado)} = 0 WHERE {nameof(Usuario.IdUsuario)} = @id";
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

		public int Modificacion(Usuario usuario)
		{
			Console.WriteLine("Estoy en usuario: " + usuario.AvatarURL);
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Usuario)} SET 
							{nameof(Usuario.Nombre)} = @Nombre, 
							{nameof(Usuario.Apellido)} = @Apellido, 
							{nameof(Usuario.Email)} = @Email, 
							{nameof(Usuario.Clave)} = @Clave,
							{nameof(Usuario.Rol)} = @Rol,
							{nameof(Usuario.AvatarURL)} = @AvatarURL
							WHERE {nameof(Usuario.IdUsuario)} = @IdUsuario";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
					cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
					cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
					cmd.Parameters.AddWithValue("@Email", usuario.Email);
					cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
					cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
					cmd.Parameters.AddWithValue("@AvatarURL", string.IsNullOrEmpty(usuario.AvatarURL) ? (object)DBNull.Value : usuario.AvatarURL);
					conn.Open();
					res = cmd.ExecuteNonQuery();
					Console.WriteLine(usuario.IdUsuario);
					conn.Close();
				}
				return res;
			}
		}

		public List<Usuario> ObtenerUsuarios()
		{
			var res = new List<Usuario>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
						{nameof(Usuario.IdUsuario)},
						{nameof(Usuario.Nombre)}, 
						{nameof(Usuario.Apellido)}, 
						{nameof(Usuario.Email)},
						{nameof(Usuario.Clave)}, 
						{nameof(Usuario.Rol)},
						{nameof(Usuario.AvatarURL)},
						{nameof(Usuario.Estado)}
					FROM {nameof(Usuario)}";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Usuario
							{
								IdUsuario = reader.GetInt32("IdUsuario"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
								Email = reader.GetString("Email"),
								Clave = reader.GetString("Clave"),
								Rol = reader.GetInt32("Rol"),
								AvatarURL = reader["AvatarURL"] != DBNull.Value ? reader.GetString("AvatarURL") : "",
								Estado = reader.GetBoolean("Estado")
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerUsuarioPorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Usuario.IdUsuario)}, 
						{nameof(Usuario.Nombre)}, 
						{nameof(Usuario.Apellido)}, 
						{nameof(Usuario.Email)}, 
						{nameof(Usuario.Clave)}, 
						{nameof(Usuario.Rol)},
						{nameof(Usuario.AvatarURL)},
						{nameof(Usuario.Estado)}
					FROM {nameof(Usuario)} 
					WHERE {nameof(Usuario.IdUsuario)} = @id";
			Usuario usuario = new Usuario();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						usuario = new Usuario
						{
							IdUsuario = reader.GetInt32("IdUsuario"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Email = reader["Email"] != DBNull.Value ? reader.GetString("Email") : "",
							Clave = reader.GetString("Clave"),
							AvatarURL = reader["AvatarURL"] != DBNull.Value ? reader.GetString("AvatarURL") : "",
							Rol = reader.GetInt32("Rol"),
							Estado = reader.GetBoolean("Estado"),
						};
					}
				}
				conn.Close();
			}
			return usuario;
		}

		public Usuario? ObtenerUsuarioPorEmail(string Email)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
						{nameof(Usuario.IdUsuario)}, 
						{nameof(Usuario.Nombre)}, 
						{nameof(Usuario.Apellido)}, 
						{nameof(Usuario.Email)}, 
						{nameof(Usuario.Clave)},
						{nameof(Usuario.Rol)},
						{nameof(Usuario.AvatarURL)},
						{nameof(Usuario.Estado)}
					FROM {nameof(Usuario)} 
					WHERE {nameof(Usuario.Email)} = @Email";
			Usuario? usuario = null;
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@Email", Email);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						usuario = new Usuario
						{
							IdUsuario = reader.GetInt32("IdUsuario"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
							AvatarURL = reader["AvatarURL"] != DBNull.Value ? reader.GetString("AvatarURL") : "",
							Estado = reader.GetBoolean("Estado"),
						};
					}
				}
				conn.Close();
			}
			return usuario;
		}
	}
}
