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
        private async Task<string> ConnectionString()
        {
            JSONReader dbConfig = new JSONReader();
            await dbConfig.ReadDBConfigJSONAsync();

            string connectionString = $"Host={dbConfig.Host};Username={dbConfig.Username};Password={dbConfig.Password};Database={dbConfig.Database};";
            return connectionString;
        }

        private async Task<long> GetRecordsCountAsync()
        {
            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM data.reportticket;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt64(userCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public async Task<bool> isEmptyAsync()
        {
            var count = await GetRecordsCountAsync();

            if (count <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> StoreReportAsync(DBReportTicket ticket)
        {
            var ticketNo = await GetRecordsCountAsync();

            if (ticketNo == -1)
            {
                throw new Exception();
            }
            else
            {
                ticketNo += 1;
            }

            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO data.reportticket (ticketno, username, plantiff, defendant, date, details, screenshot)" +
                                  $"VALUES ('{ticketNo}', '{ticket.UserName}', '{ticket.Plantiff}', '{ticket.Defendant}', '{ticket.Date}', '{ticket.Details}','{ticket.Screenshots}');";

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

        public async Task<(bool, DBReportTicket)> GetReportTicketAsync(int ticketNo)
        {
            DBReportTicket ticket;

            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT t.ticketno, t.username, t.plantiff, t.defendant, t. date, t.details, t.screenshot " +
                                   "FROM data.reportticket t " +
                                  $"WHERE ticketno = '{ticketNo}';";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();

                        ticket = new DBReportTicket
                        {
                            UserName = reader.GetString(1),
                            Plantiff = reader.GetString(2),
                            Defendant = reader.GetString(3),
                            Date = reader.GetString(4),
                            Details = reader.GetString(5),
                            Screenshots = reader.GetString(6),
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

        public async Task<(bool, List<DBReportTicket>)> GetAllReportTicketsAsync()
        {
            List<DBReportTicket> tickets = new List<DBReportTicket>();
            DBReportTicket ticket;

            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT t.ticketno, t.plantiff, t.defendant " +
                                   "FROM data.reportticket t;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var reader = await cmd.ExecuteReaderAsync();

                        while(await reader.ReadAsync())
                        {
                            ticket = new DBReportTicket
                            {
                                TicketNo = reader.GetInt16(0),
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

        public async Task<bool> DeleteReportTicketAsync(int ticketNo)
        {
            try
            {
                string connectionString = await ConnectionString();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM data.reportticket " +
                                   $"WHERE ticketno = '{ticketNo}'";

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
