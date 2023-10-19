using Leaf_Village_Bot.Config;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.DBUtil.ReportTicket
{
    public class DBUtil_ReportTicket
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
                                   "FROM data.reportticket " +
                                   $"WHERE serverid = {serverid};";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var count = await cmd.ExecuteScalarAsync();
                        var convert = Convert.ToInt16(count);

                        if(convert <= 0)
                        {
                            return true;
                        } else
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

        public async Task<bool> StoreReportAsync(DBReportTicket ticket)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO data.reportticket (ticketid, username, memberid, serverid, plantiff, defendant, date, details) " +
                                  $"VALUES ({ticket.TicketID}, '{ticket.UserName}', {ticket.MemberID}, {ticket.ServerID}, '{ticket.Plantiff}', '{ticket.Defendant}', '{ticket.Date}', '{ticket.Details}');";

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

        public async Task<(bool, DBReportTicket)> GetReportTicketAsync(ulong TicketID, ulong ServerID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                DBReportTicket ticket;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT t.ticketid, t.memberid, t.username, t.plantiff, t.defendant, t. date, t.details " +
                                   "FROM data.reportticket t " +
                                  $"WHERE ticketid = {TicketID} AND serverid = {ServerID};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        ticket = new DBReportTicket
                        {
                            TicketID = (ulong)reader.GetInt64(0),
                            MemberID = (ulong)reader.GetInt64(1),
                            UserName = reader.GetString(2),
                            Plantiff = reader.GetString(3),
                            Defendant = reader.GetString(4),
                            Date = reader.GetString(5),
                            Details = reader.GetString(6)
                        };
                    }
                }
                return (true, ticket);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }

        public async Task<(bool, List<DBReportTicket>)> GetAllReportTicketsAsync(ulong ServerID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                List<DBReportTicket> tickets = new List<DBReportTicket>();
                DBReportTicket ticket;

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT t.ticketid, t.plantiff, t.defendant " +
                                   "FROM data.reportticket t " +
                                  $"WHERE serverid = {ServerID};";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while(await reader.ReadAsync())
                        {
                            ticket = new DBReportTicket
                            {
                                TicketID = (ulong)reader.GetInt64(0),
                                Plantiff = reader.GetString(1),
                                Defendant = reader.GetString(2)
                            };
                            tickets.Add(ticket);
                        }
                    }
                }
                return (true, tickets);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
            
        }

        public async Task<bool> DeleteReportTicketAsync(ulong TicketID)
        {
            try
            {
                string connectionString = await ConnectionStringAsync();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM data.reportticket " +
                                   $"WHERE ticketid = {TicketID}";

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
    }
}
