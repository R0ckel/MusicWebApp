using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using System.Data.SqlClient;

namespace MusicWebApp.DataAccess.Accessors
{
    public class AlbumDAO
    {
        public string ConnectionString { get; }

        public AlbumDAO(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnection") ?? "";
        }

        public Album? Read(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, year, dbo.GetAlbumAuthorList(id) from albums where id = @id";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@id", id));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Album album = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Year = reader.GetInt32(2)
                    };
                    album.SetAuthorsFromString(reader.GetString(3));
                    return album;
                }
                Console.WriteLine("No such album found!");
                return null;
            }
        }

        //public Album? ReadByName(string name)
        //{
        //    using (SqlConnection connection = new(ConnectionString))
        //    {
        //        string sql = "select id, name from ReadAlbumByName(@name)";
        //        connection.Open();
        //        SqlCommand sqlCommand = new(sql, connection);
        //        sqlCommand.Parameters.Add(new("@name", name));
        //        SqlDataReader reader = sqlCommand.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            Album album = new()
        //            {
        //                Id = reader.GetInt32(0),
        //                Name = reader.GetString(1),
        //            };
        //            return album;
        //        }
        //        return null;
        //    }
        //}

        public IEnumerable<Album> ReadByReader(SqlDataReader reader)
        {
            List<Album> albums = new();
            while (reader.Read())
            {
                Album album = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Year = reader.GetInt32(2)
                };
                album.SetAuthorsFromString(reader.GetString(3));
                albums.Add(album);
            }
            return albums;
        }

        public IEnumerable<Album> ReadAll()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, year, dbo.GetAlbumAuthorList(id) from albums";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public IEnumerable<Album> ReadPage(int page, int pageSize, AlbumFilter filter)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name, year, authors from GetAlbumsFilteredPage" +
                    "(@name, @minY, @maxY, @author, @offset, @pageSize)";
                int offset = pageSize * (page - 1);
                if (offset < 0) offset = 0;

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", filter.Name));
                cmd.Parameters.Add(new("@minY", filter.MinYear));
                cmd.Parameters.Add(new("@maxY", filter.MaxYear));
                cmd.Parameters.Add(new("@author", filter.Author));
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
                string sql = "select count(id) from albums";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int CountFiltered(AlbumFilter filter)
        {
            if (filter == null) return Count();
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from GetAlbumsFiltered(@name, @minY, @maxY, @author)";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", filter.Name));
                cmd.Parameters.Add(new("@minY", filter.MinYear));
                cmd.Parameters.Add(new("@maxY", filter.MaxYear));
                cmd.Parameters.Add(new("@author", filter.Author));

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Create(Album entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "CreateAlbum"; //@name, @year, @authors_id (int list, separated by ';')
                connection.Open();
                SqlCommand cmd = new(sql, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new("@name", entity.Name));
                cmd.Parameters.Add(new("@year", entity.Year));
                cmd.Parameters.Add(new("@authors_id", entity.GetAuthorsIdAsString()));
                int result = cmd.ExecuteNonQuery();
                if (result == 0) Console.WriteLine("Album was not inserted - error occured");
            }
        }

        public void Update(Album entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "UpdateAlbum"; //@id, @name, @year, @authors_id (int list, separated by ';')
                connection.Open();
                SqlCommand cmd = new(sql, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new("@id", entity.Id));
                cmd.Parameters.Add(new("@name", entity.Name));
                cmd.Parameters.Add(new("@year", entity.Year));
                cmd.Parameters.Add(new("@authors_id", entity.GetAuthorsIdAsString()));
                int result = cmd.ExecuteNonQuery();
                if (result == 0) Console.WriteLine("Album was not inserted - error occured");
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "delete from albums where id = @id";
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
                    Console.WriteLine($"Was try to delete {deleted} rows in 1 action (TABLE : Albums). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }
    }
}
