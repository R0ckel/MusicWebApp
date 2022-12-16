using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using System.Data.SqlClient;

namespace MusicWebApp.DataAccess.Accessors
{
    public class TrackDAO
    {
        public string ConnectionString { get; }

        public TrackDAO(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnection") ?? "";
        }

        public Track? Read(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, track, year, genre, genre_id, album, dbo.GetTrackAuthorList(id) from all_info where id = @id";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@id", id));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Track track = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Year = reader.GetInt32(2),
                        TrackGenre = new()
                        {
                            Name = reader.GetString(3),
                            Id = reader.GetInt32(4),
                        },
                        TrackAlbum = new()
                        {
                            Name = reader.GetString(5)
                        }
                    };
                    track.SetAuthorsFromString(reader.GetString(6));
                    return track;
                }
                Console.WriteLine("No such track found!");
                return null;
            }
        }

        //public Track? ReadByName(string name)
        //{
        //    using (SqlConnection connection = new(ConnectionString))
        //    {
        //        string sql = "select id, name from ReadTrackByName(@name)";
        //        connection.Open();
        //        SqlCommand sqlCommand = new(sql, connection);
        //        sqlCommand.Parameters.Add(new("@name", name));
        //        SqlDataReader reader = sqlCommand.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            Track track = new()
        //            {
        //                Id = reader.GetInt32(0),
        //                Name = reader.GetString(1),
        //            };
        //            return track;
        //        }
        //        return null;
        //    }
        //}

        public IEnumerable<Track> ReadByReader(SqlDataReader reader)
        {
            List<Track> tracks = new();
            while (reader.Read())
            {
                Track track = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Year = reader.GetInt32(2),
                    TrackGenre = new()
                    {
                        Name = reader.GetString(3),
                        Id = reader.GetInt32(4),
                    },
                    TrackAlbum = new()
                    {
                        Name = reader.GetString(5)
                    }
                };
                track.SetAuthorsFromString(reader.GetString(6));
                tracks.Add(track);
            }
            return tracks;
        }

        public IEnumerable<Track> ReadAll()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, track, year, genre, genre_id, album, dbo.GetTrackAuthorList(id) from all_info";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public IEnumerable<Track> ReadPage(int page, int pageSize, TrackFilter filter)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, track, year, genre, genre_id, album, authors from GetTracksFilteredPage" +
                    "(@name, @minY, @maxY, @author, @album, @genre, @offset, @pageSize)";
                int offset = pageSize * (page - 1);
                if (offset < 0) offset = 0;

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", filter.Name));
                cmd.Parameters.Add(new("@minY", filter.MinYear));
                cmd.Parameters.Add(new("@maxY", filter.MaxYear));
                cmd.Parameters.Add(new("@author", filter.Author));
                cmd.Parameters.Add(new("@album", filter.Album));
                cmd.Parameters.Add(new("@offset", offset));
                cmd.Parameters.Add(new("@pageSize", pageSize));
                cmd.Parameters.Add(new("@genre", filter.Genre));

                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public int Count()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from all_info";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int CountFiltered(TrackFilter filter)
        {
            if (filter == null) return Count();
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from GetTracksFiltered(@name, @minY, @maxY, @author, @album, @genre)";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", filter.Name));
                cmd.Parameters.Add(new("@minY", filter.MinYear));
                cmd.Parameters.Add(new("@maxY", filter.MaxYear));
                cmd.Parameters.Add(new("@author", filter.Author));
                cmd.Parameters.Add(new("@album", filter.Album));
                cmd.Parameters.Add(new("@genre", filter.Genre));

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Create(Track entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "CreateTrack";
                //  @name varchar(50), @year int, @authors_id varchar(50) (int list, separated by ';'),
                //  @genre_id int, @album_id int,
                connection.Open();
                SqlCommand cmd = new(sql, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new("@name", entity.Name));
                cmd.Parameters.Add(new("@year", entity.Year));
                cmd.Parameters.Add(new("@authors_id", entity.GetAuthorsIdAsString()));
                cmd.Parameters.Add(new("@album_id", entity.TrackAlbum.Id));
                cmd.Parameters.Add(new("@genre_id", entity.TrackGenre.Id));

                int result = cmd.ExecuteNonQuery();
                if (result == 0) Console.WriteLine("Track was not inserted - error occured");
            }
        }

        public void Update(Track entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "UpdateTrack";
                //  @id int, @name varchar(50), @year int, @authors_id varchar(50) (int list, separated by ';'),
                //  @genre_id int, @album_id int,
                connection.Open();
                SqlCommand cmd = new(sql, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new("@id", entity.Id));
                cmd.Parameters.Add(new("@name", entity.Name));
                cmd.Parameters.Add(new("@year", entity.Year));
                cmd.Parameters.Add(new("@authors_id", entity.GetAuthorsIdAsString()));
                cmd.Parameters.Add(new("@album_id", entity.TrackAlbum.Id));
                cmd.Parameters.Add(new("@genre_id", entity.TrackGenre.Id));
                int result = cmd.ExecuteNonQuery();
                if (result == 0) Console.WriteLine("Track was not updated - error occured");
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "delete from tracks where id = @id";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@id", id));
                int deleted = cmd.ExecuteNonQuery();
                if (deleted < 1 || deleted > 2)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to delete {deleted} rows in 1 action (TABLE : Tracks). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

    }
}
