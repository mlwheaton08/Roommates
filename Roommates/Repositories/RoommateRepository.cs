using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories;

internal class RoommateRepository : BaseRepository
{
    public RoommateRepository(string connectionString) : base(connectionString) { }

    public Roommate GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select Roommate.Id, FirstName, LastName, RentPortion, Name
                                    from Roommate
                                    join Room
                                    on Roommate.RoomId = Room.Id
                                    where Roommate.Id = @id;";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                Roommate roommate = null;

                if (reader.Read())
                {
                    roommate = new Roommate
                    {
                        Id = id,
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                        Room = new Room() { Name = reader.GetString(reader.GetOrdinal("Name")) }
                    };
                }

                reader.Close();
                return roommate;
            }
        }
    }
}
