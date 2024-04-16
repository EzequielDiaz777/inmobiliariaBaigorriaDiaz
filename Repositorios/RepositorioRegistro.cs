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
			var res = -1;
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql =
						@$"INSERT INTO {nameof(Registro)}
							({nameof(Registro.IdUsuario)},
							{nameof(Registro.IdFila)},
							{nameof(Registro.NombreDeTabla)},
							{nameof(Registro.TipoDeAccion)},
							{nameof(Registro.FechaDeAccion)}
						VALUES
							(@IdUsuario, 
							@IdFila, 
							@NombreDeTabla, 
							@TipoDeAccion, 
							@FechaDeAccion);
				SELECT LAST_INSERT_ID()";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsuario", registro.IdUsuario);
					cmd.Parameters.AddWithValue("@IdFila", registro.IdFila);
					cmd.Parameters.AddWithValue("@NombreDeTabla", registro.NombreDeTabla);
					cmd.Parameters.AddWithValue("@TipoDeAccion", registro.TipoDeAccion);
					cmd.Parameters.AddWithValue("@FechaDeAccion", registro.FechaDeAccion.ToDateTime(TimeOnly.MinValue));
					conn.Open();
					res = Convert.ToInt32(cmd.ExecuteScalar());
					registro.IdRegistro = res;
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
				var sql = @$"DELETE FROM {nameof(Registro)} WHERE {nameof(Registro.IdRegistro)} = @id";
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
								{nameof(Registro.FechaDeAccion)} = @FechaDeAccion
							WHERE {nameof(Registro.IdRegistro)} = @IdRegistro";
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@IdUsuario", registro.IdUsuario);
					cmd.Parameters.AddWithValue("@IdFila", registro.IdFila);
					cmd.Parameters.AddWithValue("@NombreDeTabla", registro.NombreDeTabla);
					cmd.Parameters.AddWithValue("@TipoDeAccion", registro.TipoDeAccion);
					cmd.Parameters.AddWithValue("@FechaDeAccion", registro.FechaDeAccion.ToDateTime(TimeOnly.MinValue));
					conn.Open();
					res = cmd.ExecuteNonQuery();
					conn.Close();
				}
				return res;
			}
		}

		public List<Registro> ObtenerRegistros()
		{
			var res = new List<Registro>();
			using (MySqlConnection conn = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT 
							{nameof(Registro.IdUsuario)},
							{nameof(Registro.IdFila)},
							{nameof(Registro.NombreDeTabla)},
							{nameof(Registro.TipoDeAccion)},
							{nameof(Registro.FechaDeAccion)}
						FROM {nameof(Registro)}";
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
								FechaDeAccion = DateOnly.FromDateTime(reader.GetDateTime("FechaDeAccion"))
							});
						}
					}
					conn.Close();
				}
			}
			return res;
		}

		public Registro ObtenerPropietarioPorID(int id)
		{
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			using MySqlConnection conn = mySqlConnection;
			var sql = @$"SELECT 
							{nameof(Registro.IdUsuario)},
							{nameof(Registro.IdFila)},
							{nameof(Registro.NombreDeTabla)},
							{nameof(Registro.TipoDeAccion)},
							{nameof(Registro.FechaDeAccion)}
						FROM {nameof(Registro)}
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
							FechaDeAccion = DateOnly.FromDateTime(reader.GetDateTime("FechaDeAccion"))
						};
					}
				}
				conn.Close();
			}
			return registro;
		}
	}
}
