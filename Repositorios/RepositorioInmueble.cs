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
            var sql = @"INSERT INTO inmueble(IdTipoDeInmueble,Direccion,Ambientes,Superficie,Longitud,Latitud,Precio,Uso,Estado,IdPropietario) 
            VALUES(@IdTipoDeInmueble,@Direccion,@Ambientes,@Superficie,@Longitud,@Latitud,@Precio,@Uso,1 ,@IdPropietario);
            SELECT LAST_INSERT_ID()";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdTipoDeInmueble", inmueble.IdTipoDeInmueble);
                cmd.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                cmd.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                cmd.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                cmd.Parameters.AddWithValue("@Longitud", inmueble.Longitud.HasValue ? inmueble.Longitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Latitud", inmueble.Latitud.HasValue ? inmueble.Latitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", inmueble.Precio);
                cmd.Parameters.AddWithValue("@Uso", inmueble.Uso);
                cmd.Parameters.AddWithValue("@IdPropietario", inmueble.IdPropietario);
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
            var sql = "DELETE FROM inmueble WHERE IdInmueble=@Id";
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

    public void ModificarInmueble(Inmueble i)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @"UPDATE inmueble SET
                     IdTipoDeInmueble=@IdTipoDeInmueble,
                     Direccion=@Direccion,
                     Ambientes=@Ambientes,
                     Superficie=@Superficie,
                     Longitud=@Longitud,
                     Latitud=@Latitud,
                     Precio=@Precio,
                     Uso=@Uso,
                     Estado=@Estado,
                     IdPropietario=@IdPropietario
                     WHERE IdInmueble=@IdInmueble";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
                cmd.Parameters.AddWithValue("@IdTipoDeInmueble", i.IdTipoDeInmueble);
                cmd.Parameters.AddWithValue("@Direccion", i.Direccion);
                cmd.Parameters.AddWithValue("@Ambientes", i.Ambientes);
                cmd.Parameters.AddWithValue("@Superficie", i.Superficie);
                cmd.Parameters.AddWithValue("@Longitud", i.Longitud.HasValue ? i.Longitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Latitud", i.Latitud.HasValue ? i.Latitud.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", i.Precio);
                cmd.Parameters.AddWithValue("@Uso", i.Uso);
                cmd.Parameters.AddWithValue("@Estado", i.Estado);
                cmd.Parameters.AddWithValue("@IdPropietario", i.IdPropietario);
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
            var sql = @"SELECT i.IdInmueble, 
                            i.Direccion, 
                            i.Ambientes, 
                            i.Superficie, 
                            i.Longitud, 
                            i.Latitud, 
                            i.Precio, 
                            i.Uso, 
                            i.Estado,
                            t.Nombre AS TipoDeInmuebleNombre,
                            p.Nombre as PropietarioNombre, 
                            p.Apellido as PropietarioApellido
                        FROM inmueble i 
                        INNER JOIN propietario p 
                        ON i.IdPropietario = p.IdPropietario
                        INNER JOIN tipodeinmueble t
                        ON i.IdTipoDeInmueble = t.IdTipoDeInmueble;";
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
                            Uso = reader.GetString("Uso"),
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


    public Inmueble ObtenerInmueblePorID(int id)
    {
        Inmueble res = null;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            var sql = @"SELECT i.IdInmueble, 
                            i.IdPropietario,
                            i.IdTipoDeInmueble,
                            i.Direccion, 
                            i.Ambientes, 
                            i.Superficie, 
                            i.Longitud, 
                            i.Latitud, 
                            i.Precio, 
                            i.Uso, 
                            i.Estado,
                            t.Nombre AS TipoDeInmuebleNombre,
                            p.Nombre as PropietarioNombre, 
                            p.Apellido as PropietarioApellido
                        FROM inmueble i 
                        INNER JOIN propietario p 
                        ON i.IdPropietario = p.IdPropietario
                        INNER JOIN tipodeinmueble t
                        ON i.IdTipoDeInmueble = t.IdTipoDeInmueble
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
                        res.IdPropietario = reader.GetInt32("IdPropietario");
                        res.IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble");
                        res.Tipo = new TipoDeInmueble
                        {
                            Nombre = reader.GetString("TipoDeInmuebleNombre"),
                        };
                        res.Direccion = reader.GetString("Direccion");
                        res.Ambientes = reader.GetInt32("Ambientes");
                        res.Superficie = reader.GetDecimal("Superficie");
                        res.Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud");
                        res.Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud");
                        res.Precio = reader.GetDecimal("Precio");
                        res.Uso = reader.GetString("Uso");
                        res.Estado = reader.GetBoolean("Estado");
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
            var sql = @"SELECT i.IdInmueble, 
                            i.Direccion, 
                            i.Ambientes, 
                            i.Superficie, 
                            i.Longitud, 
                            i.Latitud, 
                            i.Precio, 
                            i.Uso, 
                            i.Estado,
                            t.Nombre AS TipoDeInmuebleNombre,
                            p.Nombre as PropietarioNombre, 
                            p.Apellido as PropietarioApellido
                        FROM inmueble i 
                        INNER JOIN propietario p 
                        ON i.IdPropietario = p.IdPropietario
                        INNER JOIN tipodeinmueble t
                        ON i.IdTipoDeInmueble = t.IdTipoDeInmueble
                        WHERE i.IdPropietario = @Id;";
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
                            IdTipoDeInmueble = reader.GetInt32("IdTipoDeInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Superficie = reader.GetDecimal("Superficie"),
                            Longitud = reader.IsDBNull(reader.GetOrdinal("Longitud")) ? (decimal?)null : reader.GetDecimal("Longitud"),
                            Latitud = reader.IsDBNull(reader.GetOrdinal("Latitud")) ? (decimal?)null : reader.GetDecimal("Latitud"),
                            Precio = reader.GetDecimal("Precio"),
                            Uso = reader.GetString("Uso"),
                            Estado = reader.GetBoolean("Estado"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
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