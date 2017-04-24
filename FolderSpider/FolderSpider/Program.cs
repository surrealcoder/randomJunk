using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace FolderSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            string sessionName = "Adoptive EVO";
            int sessionID = 0;
            string sessionPath = "V:\\";

            string mySQL = "insert into sessions (name,path) values (@name,@path) select @@identity";
            using (SqlConnection conn = new SqlConnection(gConfig.dbWriter))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(mySQL, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("name", sessionName));
                    cmd.Parameters.Add(new SqlParameter("path", sessionPath));

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sessionID = int.Parse(reader[0].ToString());
                        }
                    }
                }
            }

            Spider spidy = new Spider();
            spidy.run(sessionPath, sessionID, 1);

            Console.WriteLine("Hit Enter to Exist");
            Console.ReadLine();
        }
    }
}
