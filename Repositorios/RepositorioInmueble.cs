using MySql.Data.MySqlClient;
using System.Data;

namespace inmobiliariaBaigorriaDiaz.Models;

public class RepositorioInmueble
{
    protected readonly string connectionString;
    public RepositorioInmueble()
    {
        connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;";
    }

    public int AltaFisica(Inmueble inmueble)
    {
        var res = -1;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"INSERT INTO {nameof(Inmueble)}
                        ({nameof(Inmueble.IdPropietario)},
                        {nameof(Inmueble.IdTipoDeInmueble)},
                        {nameof(Inmueble.IdUsoDeInmueble)},
                        {nameof(Inmueble.Direccion)},
                        {nameof(Inmueble.Ambientes)},
                        {nameof(Inmueble.Superficie)},
                        {nameof(Inmueble.Latitud)},
                        {nameof(Inmueble.Longitud)},
                        {nameof(Inmueble.Precio)},
                        {nameof(Inmueble.Estado)})
                    VALUES(
                        @IdPropietario,
                        @IdTipoDeInmueble,
                        @IdUsoDeInmueble,
                        @Direccion, 
                        @Ambientes, 
                        @Superficie,  
                        @Latitud,
                        @Longitud, 
                        @Precio,
                        1
                    );
                SELECT LAST_INSERT_ID()";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                cmd.Parameters.AddWithValue("@IdTipoDeInmueble", inmueble.IdTipoDeInmueble);
                cmd.Parameters.AddWithValue("@IdUsoDeInmueble", inmueble.IdUsoDeInmueble);
                cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                cmd.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                cmd.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud.HasValue ? inmueble.Longitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud.HasValue ? inmueble.Latitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                conn.Open();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                res = Convert.ToInt32(cmd.ExecuteScalar());

                inmueble.IdInmueble = res;
                conn.Close();
            }
        }
        return res;
    }

    public bool AltaLogica(int id)
    {
        bool alta;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @$"UPDATE {nameof(Inmueble)} SET {nameof(Inmueble.Estado)} = 1 WHERE {nameof(Inmueble.IdInmueble)} = @id";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                alta = cmd.ExecuteNonQuery() != 0;
                conn.Close();
                if (!alta)
                {
                    throw new InvalidOperationException($"No se encontró ningún Inmueble con el ID {id} para dar de alta.");
                }
            }
        }
        return alta;
    }

    public bool BajaLogica(int id)
    {
        bool baja;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @$"UPDATE {nameof(Inmueble)} SET {nameof(Inmueble.Estado)} = 0 WHERE {nameof(Inmueble.IdInmueble)} = @id";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                baja = cmd.ExecuteNonQuery() != 0;
                conn.Close();
                if (!baja)
                {
                    throw new InvalidOperationException($"No se encontró ningún Inmueble con el ID {id} para dar de baja.");
                }
            }
        }
        return baja;
    }

    public void ModificarInmueble(Inmueble inmueble)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"UPDATE {nameof(Inmueble)} SET
                        {nameof(Inmueble.IdPropietario)} = @IdPropietario,
                        {nameof(Inmueble.IdTipoDeInmueble)} = @IdTipoDeInmueble,
                        {nameof(Inmueble.IdUsoDeInmueble)} = @IdUsoDeInmueble,
                        {nameof(Inmueble.Direccion)} = @Direccion,
                        {nameof(Inmueble.Ambientes)} = @Ambientes,
                        {nameof(Inmueble.Superficie)} = @Superficie,
                        {nameof(Inmueble.Longitud)} = @Longitud,
                        {nameof(Inmueble.Latitud)} = @Latitud,
                        {nameof(Inmueble.Precio)} = @Precio
                    WHERE {nameof(Inmueble.IdInmueble)} = @IdInmueble;";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdInmueble", inmueble.IdInmueble);
                cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
                cmd.Parameters.AddWithValue("@IdTipoDeInmueble", inmueble.IdTipoDeInmueble);
                cmd.Parameters.AddWithValue("@IdUsoDeInmueble", inmueble.IdUsoDeInmueble);
                cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                cmd.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                cmd.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud.HasValue ? inmueble.Longitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud.HasValue ? inmueble.Latitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }

    public int ObtenerCantidadDeFilas()
    {
        var res = 0;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @$"SELECT COUNT({nameof(Inmueble.IdInmueble)})
							FROM {nameof(Inmueble)};";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = reader.GetInt32("COUNT(IdInmueble)");
                    }
                }
                conn.Close();
            }
        }
        return res;
    }

    public List<Inmueble> ObtenerInmuebles(int limite, int paginado)
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        int offset = (paginado - 1) * limite;

        if (offset < 0)
        {
            offset = 0;
        }
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.{nameof(Inmueble.IdInmueble)},
                        i.{nameof(Inmueble.Direccion)},
                        i.{nameof(Inmueble.Ambientes)},
                        i.{nameof(Inmueble.Superficie)},
                        i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.Latitud)},
                        i.{nameof(Inmueble.Precio)},
                        u.{nameof(UsoDeInmueble.Nombre)} AS UsoDeInmuebleNombre,
                        i.{nameof(Inmueble.Estado)},
                        t.{nameof(TipoDeInmueble.Nombre)} AS TipoDeInmuebleNombre,
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre,
                        p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                    FROM {nameof(Inmueble)} i 
                    INNER JOIN {nameof(Propietario)} p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN {nameof(TipoDeInmueble)} t
                    ON i.{nameof(Inmueble.IdTipoDeInmueble)} = t.{nameof(TipoDeInmueble.IdTipoDeInmueble)}
                    INNER JOIN {nameof(UsoDeInmueble)} u
                    ON i.{nameof(Inmueble.IdUsoDeInmueble)} = u.{nameof(UsoDeInmueble.IdUsoDeInmueble)}
                    LIMIT {limite} OFFSET {offset};";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Superficie = reader.GetDecimal("Superficie"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Estado = reader.GetBoolean("Estado"),
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmuebles;
    }

    public List<Inmueble> ObtenerInmueblesPorPropietario(int id, int limite, int paginado)
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        int offset = (paginado - 1) * limite;

        if (offset < 0)
        {
            offset = 0;
        }
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.{nameof(Inmueble.IdInmueble)},
                        i.{nameof(Inmueble.Direccion)},
                        i.{nameof(Inmueble.Ambientes)},
                        i.{nameof(Inmueble.Superficie)},
                        i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.Latitud)},
                        i.{nameof(Inmueble.Precio)},
                        u.{nameof(UsoDeInmueble.Nombre)} AS UsoDeInmuebleNombre,
                        i.{nameof(Inmueble.Estado)},
                        t.{nameof(TipoDeInmueble.Nombre)} AS TipoDeInmuebleNombre,
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre,
                        p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                    FROM {nameof(Inmueble)} i 
                    INNER JOIN {nameof(Propietario)} p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN {nameof(TipoDeInmueble)} t
                    ON i.{nameof(Inmueble.IdTipoDeInmueble)} = t.{nameof(TipoDeInmueble.IdTipoDeInmueble)}
                    INNER JOIN {nameof(UsoDeInmueble)} u
                    ON i.{nameof(Inmueble.IdUsoDeInmueble)} = u.{nameof(UsoDeInmueble.IdUsoDeInmueble)}
                    WHERE i.{nameof(Inmueble.IdPropietario)} = @id
                    LIMIT {limite} OFFSET {offset};";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Superficie = reader.GetDecimal("Superficie"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Estado = reader.GetBoolean("Estado"),
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmuebles;
    }

    public List<Inmueble> ObtenerInmuebles()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.{nameof(Inmueble.IdInmueble)},
                        i.{nameof(Inmueble.Direccion)},
                        i.{nameof(Inmueble.Ambientes)},
                        i.{nameof(Inmueble.Superficie)},
                        i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.Latitud)},
                        i.{nameof(Inmueble.Precio)},
                        u.{nameof(UsoDeInmueble.Nombre)} AS UsoDeInmuebleNombre,
                        i.{nameof(Inmueble.Estado)},
                        t.{nameof(TipoDeInmueble.Nombre)} AS TipoDeInmuebleNombre,
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre,
                        p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                    FROM {nameof(Inmueble)} i 
                    INNER JOIN {nameof(Propietario)} p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN {nameof(TipoDeInmueble)} t
                    ON i.{nameof(Inmueble.IdTipoDeInmueble)} = t.{nameof(TipoDeInmueble.IdTipoDeInmueble)}
                    INNER JOIN {nameof(UsoDeInmueble)} u
                    ON i.{nameof(Inmueble.IdUsoDeInmueble)} = u.{nameof(UsoDeInmueble.IdUsoDeInmueble)};";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Superficie = reader.GetDecimal("Superficie"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Estado = reader.GetBoolean("Estado"),
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmuebles;
    }

    public Inmueble? ObtenerInmueblePorID(int id)
    {
        Inmueble? res = null;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.{nameof(Inmueble.IdInmueble)},
                        i.{nameof(Inmueble.IdTipoDeInmueble)},
                        i.{nameof(Inmueble.IdUsoDeInmueble)},
                        i.{nameof(Inmueble.IdPropietario)},
                        i.{nameof(Inmueble.Direccion)},
                        i.{nameof(Inmueble.Ambientes)},
                        i.{nameof(Inmueble.Superficie)},
                        i.{nameof(Inmueble.Longitud)},
                        i.{nameof(Inmueble.Latitud)},
                        i.{nameof(Inmueble.Precio)},
                        i.{nameof(Inmueble.Estado)},
                        u.{nameof(UsoDeInmueble.Nombre)} AS UsoDeInmuebleNombre,
                        t.{nameof(TipoDeInmueble.Nombre)} AS TipoDeInmuebleNombre,
                        p.{nameof(Propietario.Nombre)} AS PropietarioNombre,
                        p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                    FROM {nameof(Inmueble)} AS i 
                    INNER JOIN {nameof(Propietario)} AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                    INNER JOIN {nameof(TipoDeInmueble)} AS t
                    ON i.{nameof(Inmueble.IdTipoDeInmueble)} = t.{nameof(TipoDeInmueble.IdTipoDeInmueble)}
                    INNER JOIN {nameof(UsoDeInmueble)} AS u
                    ON i.{nameof(Inmueble.IdUsoDeInmueble)} = u.{nameof(UsoDeInmueble.IdUsoDeInmueble)}
                    WHERE i.{nameof(Inmueble.IdInmueble)} = @Id;";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    res = new Inmueble();
                    if (reader.Read())
                    {

                        res.IdInmueble = reader.GetInt32("IdInmueble");
                        res.Direccion = reader.GetString("Direccion");
                        res.Ambientes = reader.GetInt32("Ambientes");
                        res.Superficie = reader.GetDecimal("Superficie");
                        res.Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud");
                        res.Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud");
                        res.Precio = reader.GetDecimal("Precio");
                        res.Uso = new UsoDeInmueble
                        {
                            Nombre = reader.GetString("UsoDeInmuebleNombre"),
                        };
                        res.Estado = reader.GetBoolean("Estado");
                        res.Tipo = new TipoDeInmueble
                        {
                            Nombre = reader.GetString("TipoDeInmuebleNombre"),
                        };
                        res.Duenio = new Propietario
                        {
                            Nombre = reader.GetString("PropietarioNombre"),
                            Apellido = reader.GetString("PropietarioApellido"),
                        };
                    }

                }
                conn.Close();
            }
        }
        if (res == null)
        {
            throw new Exception("No se encontró ningún Inmueble con el ID especificado.");
        }

        return res;
    }

    public List<Inmueble> ObtenerInmueblesPorUso(int id)
    {
        Console.WriteLine(id);
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.IdInmueble,
                        i.Direccion,
                        i.Precio,
                        u.Nombre AS UsoDeInmuebleNombre,
                        t.Nombre AS TipoDeInmuebleNombre,
                        p.Nombre AS PropietarioNombre,
                        p.Apellido AS PropietarioApellido
                    FROM inmueble i 
                        INNER JOIN propietario p 
                            ON i.IdPropietario = p.IdPropietario
                        INNER JOIN tipodeinmueble t
                            ON i.IdTipoDeInmueble = t.IdTipoDeInmueble
                        INNER JOIN usodeinmueble u
                            ON i.IdUsoDeInmueble = u.IdUsoDeInmueble
                    WHERE i.IdUsoDeInmueble = @id
                    AND i.Estado = 1
                        AND NOT EXISTS (
                            SELECT 1 FROM Contrato c
                            WHERE c.IdInmueble = i.IdInmueble);";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmuebles;
    }

    public List<Inmueble> ObtenerInmueblesPorTipo(int id)
    {
        Console.WriteLine(id);
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"SELECT 
                        i.IdInmueble,
                        i.Direccion,
                        i.Precio,
                        u.Nombre AS UsoDeInmuebleNombre,
                        t.Nombre AS TipoDeInmuebleNombre,
                        p.Nombre AS PropietarioNombre,
                        p.Apellido AS PropietarioApellido
                    FROM inmueble i 
                        INNER JOIN propietario p 
                            ON i.IdPropietario = p.IdPropietario
                        INNER JOIN tipodeinmueble t
                            ON i.IdTipoDeInmueble = t.IdTipoDeInmueble
                        INNER JOIN usodeinmueble u
                            ON i.IdUsoDeInmueble = u.IdUsoDeInmueble
                    WHERE i.IdTipoDeInmueble = @id
                    AND i.Estado = 1
                        AND NOT EXISTS (
                            SELECT 1 FROM Contrato c
                            WHERE c.IdInmueble = i.IdInmueble);";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmuebles;
    }

    public List<Inmueble> BuscarInmuebles(int? idUsoDeInmueble, int? idTipoDeInmueble, int? ambientes, decimal? precioDesde, decimal? precioHasta, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        List<Inmueble> inmueblesEncontrados = new List<Inmueble>();
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @"
            SELECT 
                inmueble.IdInmueble,
                inmueble.Direccion,
                inmueble.Precio,
                usodeinmueble.Nombre AS UsoDeInmuebleNombre,
                tipodeinmueble.Nombre AS TipoDeInmuebleNombre,
                propietario.Nombre AS PropietarioNombre,
                propietario.Apellido AS PropietarioApellido
            FROM inmueble 
                INNER JOIN tipodeinmueble 
                    ON inmueble.IdTipoDeInmueble = tipodeinmueble.IdTipoDeInmueble
                INNER JOIN propietario 
                    ON inmueble.IdPropietario = propietario.IdPropietario
                INNER JOIN usodeinmueble 
                    ON inmueble.IdUsoDeInmueble = usodeinmueble.IdUsoDeInmueble
            WHERE inmueble.Estado = 1";
            if (idTipoDeInmueble.HasValue)
            {
                sql += " AND inmueble.IdTipoDeInmueble = @idTipoDeInmueble";
            }
            if (idUsoDeInmueble.HasValue)
            {
                sql += " AND inmueble.IdUsoDeInmueble = @idUsoDeInmueble";
            }
            if (ambientes.HasValue)
            {
                sql += " AND inmueble.Ambientes = @ambientes";
            }
            if (precioDesde.HasValue)
            {
                sql += " AND inmueble.Precio >= @precioDesde";
            }
            if (precioHasta.HasValue)
            {
                sql += " AND inmueble.Precio <= @precioHasta";
            }
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                sql += @"
                AND NOT EXISTS (
                    SELECT 1 FROM Contrato c
                    WHERE c.IdInmueble = inmueble.IdInmueble
                        AND c.AlquilerHasta >= @fechaDesde AND c.AlquilerDesde <= @fechaHasta
                )";
            }
            else if (fechaDesde.HasValue)
            {
                sql += @"
                AND NOT EXISTS (
                    SELECT 1 FROM Contrato c
                    WHERE c.IdInmueble = inmueble.IdInmueble
                        AND c.AlquilerHasta >= @fechaDesde
                )";
            }
            else if (fechaHasta.HasValue)
            {
                sql += @"
                AND NOT EXISTS (
                    SELECT 1 FROM Contrato c
                    WHERE c.IdInmueble = inmueble.IdInmueble
                        AND c.AlquilerDesde <= @fechaHasta
                )";
            }
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@idUsoDeInmueble", idUsoDeInmueble);
                cmd.Parameters.AddWithValue("@idTipoDeInmueble", idTipoDeInmueble);
                cmd.Parameters.AddWithValue("@ambientes", ambientes);
                if (precioDesde.HasValue)
                {
                    cmd.Parameters.AddWithValue("@precioDesde", precioDesde.Value);
                }
                if (precioHasta.HasValue)
                {
                    cmd.Parameters.AddWithValue("@precioHasta", precioHasta.Value);
                }
                if (fechaDesde.HasValue)
                {
                    cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value);
                }
                if (fechaHasta.HasValue)
                {
                    cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value);
                }
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = new UsoDeInmueble
                            {
                                Nombre = reader.GetString("UsoDeInmuebleNombre"),
                            },
                            Tipo = new TipoDeInmueble
                            {
                                Nombre = reader.GetString("TipoDeInmuebleNombre"),
                            },
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("PropietarioApellido"),
                            }
                        };
                        inmueblesEncontrados.Add(inmueble);
                    }
                }
                conn.Close();
            }
        }
        return inmueblesEncontrados;
    }
}