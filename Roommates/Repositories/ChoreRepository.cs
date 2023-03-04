using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories;

internal class ChoreRepository : BaseRepository
{
    public ChoreRepository(string connectionString) : base(connectionString) { }

    public List<Chore> GetAll()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Name FROM Chore";
                SqlDataReader reader = cmd.ExecuteReader();
                List<Chore> chores = new List<Chore>();

                while (reader.Read())
                {
                    int idColumnPosition = reader.GetOrdinal("Id");
                    int idValue = reader.GetInt32(idColumnPosition);
                    int nameColumnPosition = reader.GetOrdinal("Name");
                    string nameValue = reader.GetString(nameColumnPosition);

                    Chore chore = new Chore
                    {
                        Id = idValue,
                        Name = nameValue,
                    };
                    chores.Add(chore);
                }

                reader.Close();
                return chores;
            }
        }
    }

    public Chore GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                Chore chore = null;

                if (reader.Read())
                {
                    chore = new Chore
                    {
                        Id = id,
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                    };
                }

                reader.Close();
                return chore;
            }
        }
    }

    public List<Chore> GetUassignedChores()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select c.Id, c.Name
                                    from chore c
                                    left join RoommateChore rmc
                                    on c.Id = rmc.ChoreId
                                    where rmc.ChoreId is null;";
                SqlDataReader reader = cmd.ExecuteReader();
                List<Chore> chores = new List<Chore>();

                while (reader.Read())
                {
                    int idColumnPosition = reader.GetOrdinal("Id");
                    int IdValue = reader.GetInt32(idColumnPosition);
                    int nameColumnPosition = reader.GetOrdinal("Name");
                    string nameValue = reader.GetString(nameColumnPosition);

                    Chore chore = new Chore
                    {
                        Id = IdValue,
                        Name = nameValue
                    };

                    chores.Add(chore);
                }

                reader.Close();
                return chores;
            }
        }
    }

    public void Insert(Chore chore)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Chore (Name)
                                    OUTPUT INSERTED.Id
                                    VALUES (@name)";
                cmd.Parameters.AddWithValue("@name", chore.Name);
                int id = (int)cmd.ExecuteScalar();

                chore.Id = id;
            }
        }
    }

    public void AssignChore(int roommateId, int choreId)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"insert into RoommateChore (RoommateId, ChoreId)
                                    values (@roommateId, @choreId)";
                cmd.Parameters.AddWithValue("@roommateId", roommateId);
                cmd.Parameters.AddWithValue("@choreId", choreId);

                cmd.ExecuteNonQuery();
            }
            
        }
    }

    public void Update(Chore chore)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"update Chore
                                    set Name = @name
                                    where Id = @id";
                cmd.Parameters.AddWithValue("@name", chore.Name);
                cmd.Parameters.AddWithValue("@id", chore.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void Delete(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "delete from Chore where Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}