using MySql.Data.MySqlClient;

namespace inmobiliariaBaigorriaDiaz.Models
{

	public class RepositorioRegistro
	{

		protected readonly string connectionString;
		public RepositorioRegistro()
		{
			connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;SslMode=none";
		}

		public int AltaFisica(Registro registro)
		{
			Console.WriteLine("Estoy en AltaFisica");
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
						@$"INSERT INTO {nameof(Registro)}
							({nameof(Registro.IdUsuario)},
							{nameof(Registro.IdFila)},
							{nameof(Registro.NombreDeTabla)},
							{nameof(Registro.TipoDeAccion)},
							{nameof(Registro.FechaDeAccion)},
							{nameof(Registro.HoraDeAccion)})
						VALUES
							(@IdUsuario, 
							@IdFila, 
							@NombreDeTabla, 
							@TipoDeAccion, 
							@FechaDeAccion,
							@HoraDeAccion);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsuario", registro.IdUsuario);
					cmd.Parameters.AddWithValue("@IdFila", registro.IdFila);
					cmd.Parameters.AddWithValue("@NombreDeTabla", registro.NombreDeTabla);
					cmd.Parameters.AddWithValue("@TipoDeAccion", registro.TipoDeAccion);
					cmd.Parameters.AddWithValue("@FechaDeAccion", registro.FechaDeAccion.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@HoraDeAccion", registro.HoraDeAccion.ToString());
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					registro.IdRegistro = res;
					conn.Close();
				}
			}
			return res;
		}

		public int Modificacion(Registro registro)
		{
			int res;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"UPDATE {nameof(Registro)} SET 
								{nameof(Registro.IdUsuario)} = @IdUsuario,
								{nameof(Registro.IdFila)} = @IdFila,
								{nameof(Registro.NombreDeTabla)} = @NombreDeTabla,
								{nameof(Registro.TipoDeAccion)} = @TipoDeAccion,
								{nameof(Registro.FechaDeAccion)} = @FechaDeAccion,
								{nameof(Registro.HoraDeAccion)} = @HoraDeAccion
							WHERE {nameof(Registro.IdRegistro)} = @IdRegistro";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsuario", registro.IdUsuario);
					cmd.Parameters.AddWithValue("@IdFila", registro.IdFila);
					cmd.Parameters.AddWithValue("@NombreDeTabla", registro.NombreDeTabla);
					cmd.Parameters.AddWithValue("@TipoDeAccion", registro.TipoDeAccion);
					cmd.Parameters.AddWithValue("@FechaDeAccion", registro.FechaDeAccion.ToDateTime(TimeOnly.MinValue));
					cmd.Parameters.AddWithValue("@HoraDeAccion", registro.HoraDeAccion.ToString());
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public int ObtenerCantidadDeFilas()
		{
			var res = 0;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT COUNT({nameof(Registro.IdRegistro)})
							FROM {nameof(Registro)};";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							res = reader.GetInt32("COUNT(IdRegistro)");
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public List<Registro> ObtenerRegistros(int limite, int paginado)
		{
			var res = new List<Registro>();
			int offset = (paginado - 1) * limite;

			if (offset < 0)
			{
				offset = 0;
			}
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
							r.{nameof(Registro.IdRegistro)},
							r.{nameof(Registro.IdUsuario)},
							r.{nameof(Registro.IdFila)},
							r.{nameof(Registro.NombreDeTabla)},
							r.{nameof(Registro.TipoDeAccion)},
							r.{nameof(Registro.FechaDeAccion)},
							r.{nameof(Registro.HoraDeAccion)},
							u.{nameof(Usuario.Nombre)},
							u.{nameof(Usuario.Apellido)}
						FROM {nameof(Registro)} r
						INNER JOIN usuario u
							ON u.{nameof(Usuario.IdUsuario)} = r.{nameof(Registro.IdUsuario)}
							LIMIT {limite} OFFSET {offset};";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Registro
							{
								IdRegistro = reader.GetInt32("IdRegistro"),
								IdUsuario = reader.GetInt32("IdUsuario"),
								IdFila = reader.GetInt32("IdFila"),
								NombreDeTabla = reader.GetString("NombreDeTabla"),
								TipoDeAccion = reader.GetString("TipoDeAccion"),
								FechaDeAccion = DateOnly.FromDateTime(reader.GetDateTime("FechaDeAccion")),
								HoraDeAccion = reader.GetTimeSpan("HoraDeAccion"),
								Usuario = new Usuario()
								{
									Nombre = reader.GetString("Nombre"),
									Apellido = reader.GetString("Apellido")
								}
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public List<Registro> ObtenerRegistros()
		{
			var res = new List<Registro>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
							r.{nameof(Registro.IdRegistro)},
							r.{nameof(Registro.IdUsuario)},
							r.{nameof(Registro.IdFila)},
							r.{nameof(Registro.NombreDeTabla)},
							r.{nameof(Registro.TipoDeAccion)},
							r.{nameof(Registro.FechaDeAccion)},
							r.{nameof(Registro.HoraDeAccion)},
							u.{nameof(Usuario.Nombre)},
							u.{nameof(Usuario.Apellido)}
						FROM {nameof(Registro)} r
						INNER JOIN usuario u
							ON u.{nameof(Usuario.IdUsuario)} = r.{nameof(Registro.IdUsuario)};";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					conn.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Registro
							{
								IdRegistro = reader.GetInt32("IdRegistro"),
								IdUsuario = reader.GetInt32("IdUsuario"),
								IdFila = reader.GetInt32("IdFila"),
								NombreDeTabla = reader.GetString("NombreDeTabla"),
								TipoDeAccion = reader.GetString("TipoDeAccion"),
								FechaDeAccion = DateOnly.FromDateTime(reader.GetDateTime("FechaDeAccion")),
								HoraDeAccion = reader.GetTimeSpan("HoraDeAccion"),
								Usuario = new Usuario()
								{
									Nombre = reader.GetString("Nombre"),
									Apellido = reader.GetString("Apellido")
								}
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Registro ObtenerRegistroPorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
							r.{nameof(Registro.IdRegistro)},
							r.{nameof(Registro.IdUsuario)},
							r.{nameof(Registro.IdFila)},
							r.{nameof(Registro.NombreDeTabla)},
							r.{nameof(Registro.TipoDeAccion)},
							r.{nameof(Registro.FechaDeAccion)},
							r.{nameof(Registro.HoraDeAccion)},
							u.{nameof(Usuario.Nombre)},
							u.{nameof(Usuario.Apellido)}
						FROM {nameof(Registro)} r
						INNER JOIN usuario u
							ON u.{nameof(Usuario.IdUsuario)} = r.{nameof(Registro.IdUsuario)}
						WHERE {nameof(Registro.IdRegistro)} = @id";
			Registro registro = new Registro();
			using (MySqlCommand cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@id", id);
				conn.Open();
				using (MySqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						registro = new Registro
						{
							IdRegistro = reader.GetInt32("IdRegistro"),
							IdUsuario = reader.GetInt32("IdUsuario"),
							IdFila = reader.GetInt32("IdFila"),
							NombreDeTabla = reader.GetString("NombreDeTabla"),
							TipoDeAccion = reader.GetString("TipoDeAccion"),
							FechaDeAccion = DateOnly.FromDateTime(reader.GetDateTime("FechaDeAccion")),
							HoraDeAccion = reader.GetTimeSpan("HoraDeAccion"),
							Usuario = new Usuario() {
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido")
							}
						};
					}
				}
				conn.Close();
			}
			return registro;
		}
	}
}
