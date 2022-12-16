using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using System.Data.SqlClient;

namespace MusicWebApp.DataAccess.Accessors
{
    public class UserDAO
    {
        public string ConnectionString { get; }

        public UserDAO(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnection") ?? "";
        }

        public User? Read(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, login, password, role_id from users where id = @id";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                cmd.Parameters.Add(new("@id", id));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User user = new()
                    {
                        Id = reader.GetInt32(0),
                        Login = reader.GetString(1),
                        Password = reader.GetString(2),
                        UserRole = (Role)reader.GetInt32(3)
                    };
                    return user;
                }
                Console.WriteLine("No such user found!");
                return null;
            }
        }

        public IEnumerable<User> ReadByReader(SqlDataReader reader)
        {
            List<User> users = new();
            while (reader.Read())
            {
                User user = new()
                {
                    Id = reader.GetInt32(0),
                    Login = reader.GetString(1),
                    Password = reader.GetString(2),
                    UserRole = (Role)reader.GetInt32(3)
                };
                users.Add(user);
            }
            return users;
        }

        public IEnumerable<User> ReadAll()
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, login, password, role_id from users";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                return ReadByReader(reader);
            }
        }

        public User? ReadByLogin(string login)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select top 1 id, login, password, role_id from users where login = @login";
                connection.Open();
                SqlCommand sqlCommand = new(sql, connection);
                sqlCommand.Parameters.Add(new("@login", login));
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    User user = new()
                    {
                        Id = reader.GetInt32(0),
                        Login = reader.GetString(1),
                        Password = reader.GetString(2),
                        UserRole = (Role)reader.GetInt32(3)
                    };
                    return user;
                }
                return null;
            }
        }

        public IEnumerable<User> ReadPage(int page, int pageSize, UserFilter filter)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select id, login, password, role_id from users " +
                    "where login like @login ";
                if (filter.Role != 0)
                {
                    sql += "and role_id = @role ";
                }
                int offset = pageSize * (page - 1);
                if (offset < 0) offset = 0;
                sql += $"order by login offset @offset rows fetch next @pageSize rows only";

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                if (filter.Role != 0)
                {
                    cmd.Parameters.Add(new("@role", filter.Role));
                }
                cmd.Parameters.Add(new("@login", $"%{filter.Login}%"));
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
                string sql = "select count(id) from users";
                connection.Open();
                SqlCommand cmd = new(sql, connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int CountFiltered(UserFilter filter)
        {
            if (filter == null ) return Count();
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "select count(id) from users " +
                    "where login like @login ";
                if (filter.Role != 0)
                {
                    sql += "and role_id = @role";
                }

                connection.Open();
                SqlCommand cmd = new(sql, connection);
                SqlParameter loginParam = new("@login", $"%{filter.Login}%");
                if (filter.Role != 0)
                {
                    cmd.Parameters.Add(new("@role", filter.Role));
                }
                cmd.Parameters.Add(loginParam);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Create(User entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "insert into users(login, password, role_id) " +
                    "values(@login, @password, @role)";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@login", entity.Login));
                cmd.Parameters.Add(new("@password", entity.Password));
                cmd.Parameters.Add(new("@role", entity.UserRole == 0 ? 1 : entity.UserRole));
                int inserted = cmd.ExecuteNonQuery();
                if (inserted != 1)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to insert {inserted} rows in 1 action (TABLE : Users). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Update(User entity)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "update users set login = @login, password = @password, role_id = @role " +
                    "where id = @id";
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand cmd = new(sql, connection)
                {
                    Transaction = transaction
                };
                cmd.Parameters.Add(new("@login", entity.Login));
                cmd.Parameters.Add(new("@password", entity.Password));
                cmd.Parameters.Add(new("@role", entity.UserRole == 0 ? 1 : entity.UserRole));
                cmd.Parameters.Add(new("@id", entity.Id));
                int updated = cmd.ExecuteNonQuery();
                if (updated != 1)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Was try to update {updated} rows in 1 action (TABLE : Users). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new(ConnectionString))
            {
                string sql = "delete from users where id = @id";
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
                    Console.WriteLine($"Was try to delete {deleted} rows in 1 action (TABLE : Users). Transaction rollback!");
                    return;
                }
                transaction.Commit();
            }
        }
    }
}
