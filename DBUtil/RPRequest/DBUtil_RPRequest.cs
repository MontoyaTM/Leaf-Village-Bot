using Leaf_Village_Bot.Config;
using Leaf_Village_Bot.DBUtil.Profile;
using Leaf_Village_Bot.DBUtil.ReportTicket;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.RPRequest
{
    public class DBUtil_RPRequest
    {
        private async Task<string> ConnectionStringAsync()
        {
            JSONReader dbConfig = new JSONReader();
            await dbConfig.ReadDBConfigJSONAsync();

            string connectionString = $"Host={dbConfig.Host};Username={dbConfig.Username};Password={dbConfig.Password};Database={dbConfig.Database};";
            return connectionString;
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
                                   "FROM data.rprequest " +
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

        public async Task<bool> StoreRequestAsync(DBRPRequest request)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO data.rprequest (requestid, username, memberid, serverid, ingamename, rpmission, attendees, timezone)" +
                                  $"VALUES ({request.RequestID}, '{request.UserName}', {request.MemberID}, '{request.ServerID}', '{request.IngameName}', '{request.RPMission}', '{request.Attendees}', '{request.Timezone}');";

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

        public async Task<(bool, DBRPRequest)> GetRequestAsync(ulong requestID, ulong serverid)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                DBRPRequest request;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT r.requestid, r.memberid, r.username, r.ingamename, r.rpmission, r. attendees, r.timezone " +
                                   "FROM data.rprequest r " +
                                  $"WHERE requestid = {requestID} AND serverid = {serverid};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        request = new DBRPRequest
                        {
                            RequestID = (ulong)reader.GetInt64(0),
                            MemberID = (ulong)reader.GetInt64(1),
                            UserName = reader.GetString(2),
                            IngameName = reader.GetString(3),
                            RPMission = reader.GetString(4),
                            Attendees = reader.GetString(5),
                            Timezone = reader.GetString(6)
                        };
                    }
                }
                return (true, request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }


        public async Task<(bool, List<DBRPRequest>)> GetAllReportTicketsAsync(ulong serverid)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                List<DBRPRequest> requests = new List<DBRPRequest>();
                DBRPRequest request;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT r.requestid, r.ingamename, r.rpmission " +
                                   "FROM data.rprequest r " +
                                  $"WHERE serverid = {serverid};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            request = new DBRPRequest
                            {
                                RequestID = (ulong)reader.GetInt64(0),
                                IngameName = reader.GetString(1),
                                RPMission = reader.GetString(2)
                            };
                            requests.Add(request);
                        }
                    }
                }
                return (true, requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }

        }

        public async Task<bool> DeleteRequestAsync(ulong requestID, ulong serverID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM data.rprequest " +
                                   $"WHERE requestid = {requestID} AND serverid = {serverID}";

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

        public async Task<bool> UpdateProctoredMissionsAsync(ulong memberid)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "UPDATE data.profiledata " +
                                  $"SET proctoredmissions = proctoredmissions + 1 " +
                                  $"WHERE memberid = {memberid};";

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

    }
}
