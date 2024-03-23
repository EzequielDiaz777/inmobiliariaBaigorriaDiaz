using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaBaigorriaDiaz.Models;

public class RepositorioInmueble
{
    protected readonly string connectionString;
    public RepositorioInmueble()
    {
        connectionString = "Server=localhost;User=root;Password=;Database=inmobiliariabaigorriadiaz;";
    }

    public int Alta(Inmueble inmueble)
    {
        var res = -1;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"INSERT INTO ({nameof(Inmueble)}
                        {nameof(Inmueble.IdPropietario)}
                        {nameof(Inmueble.IdTipoDeInmueble)},
                        {nameof(Inmueble.IdUsoDeInmueble)},
                        {nameof(Inmueble.Direccion)},
                        {nameof(Inmueble.Ambientes)},
                        {nameof(Inmueble.Superficie)},
                        {nameof(Inmueble.Longitud)},
                        {nameof(Inmueble.Latitud)},
                        {nameof(Inmueble.Precio)},
                        {nameof(Inmueble.Estado)})
                    VALUES(
                        @IdPropietario,
                        @IdTipoDeInmueble,
                        @IdUsoDeInmueble,
                        @Direccion, 
                        @Ambientes, 
                        @Superficie, 
                        @Longitud, 
                        @Latitud, 
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
                res = Convert.ToInt32(cmd.ExecuteScalar());
                inmueble.IdInmueble = res;
                conn.Close();
            }
        }
        return res;
    }

    public void EliminarInmueble(int id)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @$"DELETE FROM {nameof(Inmueble)} WHERE {nameof(Inmueble.IdInmueble)} = @Id";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); // Obtiene la cantidad de filas afectadas por la eliminación
                conn.Close();

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"No se encontró ningún Inmueble con el ID {id} para eliminar.");
                }
            }
        }
    }

    public void ModificarInmueble(Inmueble inmueble)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql =
                    @$"UPDATE {nameof(Inmueble)} SET
                        {nameof(Inmueble.IdPropietario)} =@IdPropietario,
                        {nameof(Inmueble.IdTipoDeInmueble)} = @IdTipoDeInmueble,
                        {nameof(Inmueble.IdUsoDeInmueble)} = @IdUsoDeInmueble.
                        {nameof(Inmueble.Direccion)} = @Direccion,
                        {nameof(Inmueble.Ambientes)} = @Ambientes,
                        {nameof(Inmueble.Superficie)} = @Superficie,
                        {nameof(Inmueble.Longitud)} = @Longitud,
                        {nameof(Inmueble.Latitud)} = @Latitud,
                        {nameof(Inmueble.Precio)} = @Precio,
                        {nameof(Inmueble.Estado)} = @Estado,
                    WHERE {nameof(Inmueble)} = @IdInmueble";
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
                cmd.Parameters.AddWithValue("@Estado", inmueble.Estado);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
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
                        u.{nameof(UsoDeInmueble.Nombre)},
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
                                Nombre = reader.GetString("Nombre"),
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
                            i.{nameof(Inmueble.Direccion)},
                            i.{nameof(Inmueble.Ambientes)},
                            i.{nameof(Inmueble.Superficie)},
                            i.{nameof(Inmueble.Longitud)},
                            i.{nameof(Inmueble.Latitud)},
                            i.{nameof(Inmueble.Precio)},
                            u.{nameof(UsoDeInmueble.Nombre)},
                            i.{nameof(Inmueble.Estado)},
                            t.{nameof(TipoDeInmueble.Nombre)} AS TipoDeInmuebleNombre,
                            p.{nameof(Propietario.Nombre)} AS PropietarioNombre,
                            p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                        FROM {nameof(Inmueble)} i 
                        INNER JOIN {nameof(Propietario)} p 
                        ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                        INNER JOIN {nameof(TipoDeInmueble)} t
                        ON i.{nameof(Inmueble.IdTipoDeInmueble)} = t.{nameof(TipoDeInmueble.IdTipoDeInmueble)}
                        INNER JOIN usodeinmueble u
                        ON i.{nameof(Inmueble.IdUsoDeInmueble)} = u.{nameof(UsoDeInmueble.IdUsoDeInmueble)}
                        WHERE i.IdInmueble = @Id;";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
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
                            Nombre = reader.GetString("Nombre"),
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

    public List<Inmueble> ObtenerInmueblesPorPropietario(int id)
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
                        u.{nameof(UsoDeInmueble.Nombre)},
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
                    WHERE i.{nameof(Inmueble.IdPropietario)} = @Id;";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
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
                                Nombre = reader.GetString("Nombre"),
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


}