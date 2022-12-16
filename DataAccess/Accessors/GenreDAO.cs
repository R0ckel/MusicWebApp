using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using System.Data.SqlClient;

namespace MusicWebApp.DataAccess.Accessors
{
    public class GenreDAO
    {
        public string ConnectionString { get; }

        public GenreDAO(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnection") ?? "";
        }

        public Genre? Read(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, track_count from genres where id = @id";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@id", id));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Genre genre = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        TrackCount = reader.GetInt32(2),
                    };
                    return genre;
                }
                Console.WriteLine("No such genre found!");
                return null;
            }
        }
        public Genre? ReadByName(string name)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, track_count from genres where name = @name";
                connection.Open();
                SqlCommand sqlCommand = new(sql, connection);
                sqlCommand.Parameters.Add(new("@name", name));
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    Genre genre = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        TrackCount= reader.GetInt32(2),
                    };
                    return genre;
                }
                return null;
            }
        }

        public IEnumerable<Genre> ReadByReader(SqlDataReader reader)
        {
            List<Genre> genres = new();
            while (reader.Read())
            {
                Genre genre = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TrackCount= reader.GetInt32(2),
                };
                genres.Add(genre);
            }
            return genres;
        }

        public IEnumerable<Genre> ReadAll()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, track_count from genres";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public IEnumerable<Genre> ReadPage(int page, int pageSize, GenreFilter filter)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, track_count from genres " +
                    "where name like @name and track_count >= @minc and track_count <= @maxc " +
                    "order by name offset @offset rows fetch next @pageSize rows only";
                int offset = pageSize * (page - 1);
                if (offset < 0) offset = 0;

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", $"%{filter.Name}%"));
                cmd.Parameters.Add(new("@minc", filter.MinTracks));
                cmd.Parameters.Add(new("@maxc", filter.MaxTracks));
                cmd.Parameters.Add(new("@offset", offset));
                cmd.Parameters.Add(new("@pageSize", pageSize));

                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public int Count()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from genres";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int CountFiltered(GenreFilter filter)
        {
            if (filter == null) return Count();
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from genres " +
                    "where name like @name and track_count >= @minc and track_count <= @maxc";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", $"%{filter.Name}%"));
                cmd.Parameters.Add(new("@minc", filter.MinTracks));
                cmd.Parameters.Add(new("@maxc", filter.MaxTracks));

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Create(Genre entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "insert into genres(name) values(@name)";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@name", entity.Name));
                int inserted = cmd.ExecuteNonQuery();
                if (inserted != 1)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to insert {inserted} rows in 1 action (TABLE : Genres). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Update(Genre entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "update genres set name = @name where id = @id";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@name", entity.Name));
                cmd.Parameters.Add(new("@id", entity.Id));
                int updated = cmd.ExecuteNonQuery();
                if (updated != 1)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to update {updated} rows in 1 action (TABLE : Genres). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "delete from genres where id = @id";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@id", id));
                int deleted = cmd.ExecuteNonQuery();
                if (deleted != 1)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to delete {deleted} rows in 1 action (TABLE : Genres). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }
    }
}
