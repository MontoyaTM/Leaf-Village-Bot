using Leaf_Village_Bot.Config;
using Npgsql;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace Leaf_Village_Bot.DBUtil.Profile
{
    public class DBUtil_Profile
    {
        private async Task<string> ConnectionStringAsync()
        {
            JSONReader dbConfig = new JSONReader();
            await dbConfig.ReadDBConfigJSONAsync();

            string connectionString = $"Host={dbConfig.Host};Username={dbConfig.Username};Password={dbConfig.Password};Database={dbConfig.Database};";
            return connectionString;
        }

        private async Task<bool> UserExistsAsync(ulong MemberID)
        {
            try
            {
                var connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM data.profiledata " +
                                  $"WHERE memberid = '{MemberID}';";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        var result = Convert.ToInt32(userCount);

                        if (result <= 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> isEmptyAsync(ulong serverid)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM data.profiledata " +
                                   $"WHERE serverid = {serverid};";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var count = await cmd.ExecuteScalarAsync();
                        var convert = Convert.ToInt16(count);

                        if (convert <= 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }
        }

        public async Task<bool> StoreVillagerApplicationAsync(DBProfile profile)
        {
            var userExists = await UserExistsAsync(profile.MemberID);

            if (userExists)
            {
                Console.WriteLine($"MemberID: {profile.MemberID} for User: {profile.UserName} already exists!");
                return false;
            }

            try
            {
                var connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var masteryArray = profile.Masteries.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    var masteries = string.Join(",", masteryArray);
                    var queryMasteries = String.Join(",", profile.Masteries.Split(",").Select(x => string.Format("'{0}'", x)));

                    var altsArray = profile.Alts.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    var alts = string.Join(",", altsArray);
                    var queryAlts = String.Join(",", profile.Alts.Split(',').Select(x => string.Format("'{0}'", x)));

                    string query = "INSERT INTO data.profiledata (memberid, username, serverid, avatarurl, profileimage, ingamename, level, clan, masteries , alts, fame, raids, organization, orgrank, proctoredmissions) " +
                                  $"VALUES ('{profile.MemberID}', '{profile.UserName}', '{profile.ServerID}', '{profile.AvatarURL}', '{profile.ProfileImage}', '{profile.InGameName}', " +
                                  $"'{profile.Level}', '{profile.Clan}', ARRAY[{queryMasteries}], ARRAY[{queryAlts}], '{profile.Fame}', '{profile.Raids}', '{profile.Organization}', '{profile.OrgRank}', '{profile.ProctoredMissions}');";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteVillagerApplication(ulong MemberID)
        {
            try
            {
                var connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM data.profiledata " +
                                 $"WHERE memberid = '{MemberID}';";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        #region Retrieve Profile Data
        public async Task<(bool, DBProfile)> GetProfile(ulong MemberID)
        {
            try
            {
                DBProfile profile = null;

                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT p.ingamename, p.level, ARRAY_TO_STRING(p.masteries, ','), p.clan, p.organization, p.orgrank, p.raids, p.fame, p.avatarurl, p.profileimage, p.proctoredmissions " +
                                   "FROM data.profiledata p " +
                                  $"WHERE memberid = ' {MemberID}'";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            profile = new DBProfile
                            {
                                InGameName = reader.GetString(0),
                                Level = reader.GetInt32(1),
                                Masteries = reader.GetString(2),
                                Clan = reader.GetString(3),
                                Organization = reader.GetString(4),
                                OrgRank = reader.GetString(5),
                                Raids = reader.GetInt32(6),
                                Fame = reader.GetInt32(7),
                                AvatarURL = reader.GetString(8),
                                ProfileImage = reader.GetString(9),
                                ProctoredMissions = reader.GetInt32(10)
                            };
                        }

                    }
                }

                return (true, profile);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }

        public async Task<(bool, string)> GetProfileImageAsync(ulong MemberID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();
                string profileURL = String.Empty;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    
                    string query = "SELECT p.profileimage " +
                                   "FROM data.profiledata p " +
                                  $"WHERE memberid = {MemberID};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        profileURL = reader.GetString(0);
                    }
                }

                return (true, profileURL);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, String.Empty);
            }
        }
        #endregion

        #region Update Profile
        public async Task<bool> UpdateProfile(ulong MemberID, DBProfile profile)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE data.profiledata " +
                                  $"SET  ingamename = '{profile.InGameName}', level = {profile.Level}, masteries = ARRAY['{profile.Masteries}'], clan = '{profile.Clan}' " +
                                  $"WHERE memberid = {MemberID};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateProfileImage(ulong MemberID, string ImageURL)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE data.profiledata " +
                                  $"SET profileimage = '{ImageURL}' " +
                                  $"WHERE memberid = {MemberID};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateLevel(ulong MemberID, int Level)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE data.profiledata " +
                    $"SET  level = {Level} " +
                    $"WHERE memberid = {MemberID};";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateRaid(ulong MemberID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE data.profiledata " +
                                  $"SET raids = raids + 1 " +
                                  $"WHERE memberid = {MemberID};";
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


        public async Task<bool> UpdateFame(ulong MemberID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE data.profiledata " +
                                   "SET fame = fame + 1 " +
                                  $"WHERE memberid = {MemberID};";
                    
                    using (var cmd = new NpgsqlCommand(query,conn))
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
        #endregion
    }
}
