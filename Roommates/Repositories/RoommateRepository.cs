using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories;

internal class RoommateRepository : BaseRepository
{
    public RoommateRepository(string connectionString) : base(connectionString) { }

    public List<Roommate> GetAll()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select Id, FirstName, LastName from Roommate";
                SqlDataReader reader = cmd.ExecuteReader();

                List<Roommate> roomates = new List<Roommate>();

                while (reader.Read())
                {
                    Roommate roommate = new Roommate
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    };

                    roomates.Add(roommate);
                }

                reader.Close();
                return roomates;
            }
        }
    }

    public Roommate GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select
                                        Roommate.Id,
                                        FirstName,
                                        LastName,
                                        RentPortion,
                                        Room.Id as RoomId,
                                        Room.Name as RoomName
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
                        Room = new Room()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                            Name = reader.GetString(reader.GetOrdinal("RoomName"))
                        }
                    };
                }

                reader.Close();
                return roommate;
            }
        }
    }
}
