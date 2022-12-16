using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using System.Data.SqlClient;

namespace MusicWebApp.DataAccess.Accessors
{
    public class AuthorDAO
    {
        public string ConnectionString { get; }

        public AuthorDAO(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnection") ?? "";
        }

        public Author? Read(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name from authors where id = @id";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@id", id));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Author author = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    };
                    return author;
                }
                Console.WriteLine("No such author found!");
                return null;
            }
        }
        public Author? ReadByName(string name)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name from ReadAuthorByName(@name)";
                connection.Open();
                SqlCommand sqlCommand = new(sql, connection);
                sqlCommand.Parameters.Add(new("@name", name));
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    Author author = new()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    };
                    return author;
                }
                return null;
            }
        }

        public IEnumerable<Author> ReadByReader(SqlDataReader reader)
        {
            List<Author> authors = new();
            while (reader.Read())
            {
                Author author = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
                authors.Add(author);
            }
            return authors;
        }

        public IEnumerable<Author> ReadAll()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name from authors";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public IEnumerable<Author> ReadPage(int page, int pageSize, AuthorFilter filter)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, name from authors where name like @name " +
                    "order by name offset @offset rows fetch next @pageSize rows only";
                int offset = pageSize * (page - 1);
                if (offset < 0) offset = 0;

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", $"%{filter.Name}%"));
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
                string sql = "select count(id) from authors";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int CountFiltered(AuthorFilter filter)
        {
            if (filter == null) return Count();
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from authors where name like @name";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@name", $"%{filter.Name}%"));

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Create(Author entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "insert into authors(name) values(@name)";
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
                    Console.WriteLine($"Was try to insert {inserted} rows in 1 action (TABLE : Authors). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Update(Author entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "update authors set name = @name where id = @id";
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
                    Console.WriteLine($"Was try to update {updated} rows in 1 action (TABLE : Authors). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "delete from authors where id = @id";
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
                    Console.WriteLine($"Was try to delete {deleted} rows in 1 action (TABLE : Authors). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }
    }
}
