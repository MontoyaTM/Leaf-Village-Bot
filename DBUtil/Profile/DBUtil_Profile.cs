using Leaf_Village_Bot.Config;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.Profile
{
    public class DBUtil_Profile
    {
        private async Task<string> ConnectionString()
        {
            JSONReader dbConfig = new JSONReader();
            await dbConfig.ReadDBConfigJSONAsync();

            string connectionString = $"Host={dbConfig.Host};Username={dbConfig.Username};Password={dbConfig.Password};Database={dbConfig.Database};";
            return connectionString;
        }

        private async Task<long> GetProfileCountAsync()
        {
            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM data.userinfo;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt64(userCount);
                    };
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        private async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                var connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM data.userinfo " +
                                  $"WHERE username = '{username}';";
                    using (var cmd = new NpgsqlCommand(query,conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        var result = Convert.ToInt32(userCount);

                        if(result <= 0)
                        {
                            return false;
                        } else
                        {
                            return true;
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> isEmptyAsync()
        {
            var count = await GetProfileCountAsync();
            
            if(count <= 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<bool> StoreVillagerApplicationAsync(DBProfile profile)
        {
            var userExists = await UserExistsAsync(profile.UserName);

            if(userExists)
            {
                Console.WriteLine($"User: {profile.UserName} already exists! Please update your profile!");
                return false;
            }

            var userNo = await GetProfileCountAsync();

            if(userNo == -1)
            {
                throw new Exception();
            } else
            {
                userNo += 1;
            }

            try
            {
                var connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var masteryArray = profile.Masteries.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    var masteries = string.Join(",", masteryArray);
                    var queryMasteries = String.Join(",", profile.Masteries.Split(",").Select(x => string.Format("'{0}'", x)));

                    var altsArray = profile.Alts.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    var alts = string.Join(",", altsArray);
                    var queryAlts = String.Join(",", profile.Alts.Split(',').Select(x => string.Format("'{0}'", x)));

                    string query = "INSERT INTO data.userinfo (userno, username, ingamename, level, clan, masteries , alts) " +
                                  $"VALUES ('{userNo}', '{profile.UserName}', '{profile.InGameName}', '{profile.Level}', '{profile.Clan}', ARRAY[{queryMasteries}], ARRAY[{queryAlts}]);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteVillagerApplication(string username)
        {
            try
            {
                var connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM data.userinfo " +
                                 $"WHERE username = '{username}';";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<(bool, DBProfile)> GetApplicationFromIngameNameAsync(string ingameName)
        {
            DBProfile profile = null;

            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT u.ingamename, u.level, ARRAY_TO_STRING(u.masteries, ','), u.clan, ARRAY_TO_STRING(u.alts, ',') " +
                                   "FROM data.userinfo u " +
                                  $"WHERE ingamename = '{ingameName}'";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while(await reader.ReadAsync())
                        {
                            profile = new DBProfile
                            {
                                InGameName = reader.GetString(0),
                                Level = reader.GetInt32(1),
                                Masteries = reader.GetString(2),
                                Clan = reader.GetString(3),
                                Alts = reader.GetString(4),
                            };
                        }
                    }
                }
                return (true, profile);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }

        public async Task<(bool, string)> GetUserNameFromIngameNameAsync(string ingameName)
        {
            string username;

            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT username " +
                                   "FROM data.userinfo " +
                                  $"WHERE ingamename = '{ingameName}';";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        username = reader.GetString(0);
                    }
                }

                return (true, username);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, string.Empty);
            }
        }
    }
}
