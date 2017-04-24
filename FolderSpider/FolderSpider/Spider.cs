using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ZetaLongPaths.Native.FileOperations;

namespace FolderSpider
{
    class Spider
    {
        // instantiate with nothing, multi-thread, use the folder path to index
        // 
        private string root = "";
        private int x = 0;
        private int sessionID = 0;
        private int maxIndex = 100;

        public Spider() { }

        public void run(string ROOT, int SESSIONID, int MAX = 100)
        {
            root = ROOT;
            sessionID = SESSIONID;
            maxIndex = MAX;
            indexer(root);
        }

        public void indexer(string path)
        {
            x++;
            Console.WriteLine(path);

            ZetaLongPaths.ZlpDirectoryInfo di = new ZetaLongPaths.ZlpDirectoryInfo(path);
            ZetaLongPaths.ZlpFileInfo[] flist = di.GetFiles();
            for (int i = 0; i < flist.Length; i++)
            {
                // insert each file into SQL
                persistInfo("file", flist[i].OriginalPath, flist[i].Name, flist[i].Length);
                // chechsum each file
                //string md5 = Md5(flist[i]);
            }
            ZetaLongPaths.ZlpDirectoryInfo[] dlist = di.GetDirectories();
  
            Console.WriteLine("recurse: " + dlist.Length);
            for (int i = 0; i < dlist.Length; i++)
            {
                // insert each directory into SQL
                persistInfo("dir", dlist[i].OriginalPath, "", 0); 
                // recursively index this folder
                indexer(dlist[i].OriginalPath);
                if (x > maxIndex && maxIndex > 0)
                {
                    break;
                }
            }


        }

        private string Md5(string FilePath)
        {
            return "";
        }
        private void persistInfo(string TYPE, string PATH, string NAME, long SIZE, string MD5 = "")
        {
            string mySQL = "insert into contents (sessionid,path,filename,filesize,md5,type) " 
                + " values (@sid,@path,@fn,@fs,@md5,@ty)";
            using (SqlConnection conn = new SqlConnection(gConfig.dbWriter))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(mySQL, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("sid", sessionID));
                    cmd.Parameters.Add(new SqlParameter("path", PATH));
                    cmd.Parameters.Add(new SqlParameter("fn", NAME));
                    cmd.Parameters.Add(new SqlParameter("fs", SIZE.ToString()));
                    cmd.Parameters.Add(new SqlParameter("md5", MD5));
                    cmd.Parameters.Add(new SqlParameter("ty", TYPE));

                    cmd.ExecuteNonQuery();
                }
            }


        }
    }

}
