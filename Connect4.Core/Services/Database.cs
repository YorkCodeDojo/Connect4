using Connect4.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Connect4.Core.Services
{
    public class Database
    {
        private readonly Settings _settings;

        public Database(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }
        public async Task<Player?> LoadPlayer(Guid playerID)
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_LoadPlayer";
                    cmd.Parameters.AddWithValue("ID", playerID);
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync()) return null;

                        return new Player((string)dr["TeamName"], playerID)
                        {
                            CurrentGameID = dr["CurrentGameID"] as Guid?,
                            SystemBot = (bool)dr["SystemBot"],
                            Password = (string)dr["Password"],
                            WebHook = dr["WebHook"] as string,
                        };

                    }
                }
            }
        }

        internal async Task<IEnumerable<Player>> GetAllPlayers()
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_GetAllPlayers";
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        var result = new List<Player>();
                        while (await dr.ReadAsync())
                        {
                            result.Add(new Player((string)dr["TeamName"], (Guid)dr["ID"])
                            {
                                CurrentGameID = dr["CurrentGameID"] as Guid?,
                                SystemBot = (bool)dr["SystemBot"],
                                Password = (string)dr["Password"],
                                WebHook = dr["WebHook"] as string,
                            });
                        }
                        return result;
                    }
                }
            }
        }
        public async Task<List<Game>> GetAllGames()
        {
            var result = new List<Game>();
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_GetAllGames";
                    using (var dr = await cmd.ExecuteReaderAsync())

                    {
                        while (await dr.ReadAsync())
                        {
                            var game = Game.LoadFromState((string)dr["State"]);
                            result.Add(game);
                        }
                    }
                }
            }
            return result;
        }
        public async Task<Game> LoadGame(Guid gameID)
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_LoadGame";
                    cmd.Parameters.AddWithValue("ID", gameID);
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync()) return null;
                        return Game.LoadFromState((string)dr["State"]);
                    }
                }
            }
        }

        public async Task SavePlayer(Player player, string password)
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_SavePlayer";
                    cmd.Parameters.AddWithValue("TeamName", player.Name);
                    cmd.Parameters.AddWithValue("Password", password);
                    player.ID = (Guid)await cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task SaveGame(Game game)
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_SaveGame";
                    cmd.Parameters.AddWithValue("ID", game.ID);
                    cmd.Parameters.AddWithValue("RedPlayerID", game.RedPlayerID);
                    cmd.Parameters.AddWithValue("YellowPlayerID", game.YellowPlayerID);
                    cmd.Parameters.AddWithValue("State", game.AsState());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task DeleteGame(Guid gameID)
        {
            using (var cn = new SqlConnection(_settings.ConnectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "usp_DeleteGame";
                    cmd.Parameters.AddWithValue("ID", gameID);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
