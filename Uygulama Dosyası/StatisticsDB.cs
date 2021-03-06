using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VeriTabaniProje
{

    class StatisticsDB
    {
        //****************************************************************************      İNDEXLER      *******************************************************************************
        public SqlConnection dbConnection = new SqlConnection(@"Data Source=.;Initial Catalog=MainDB;Integrated Security=True");

        public void CreateBasicTable()
        {
            dbConnection.Open();

            string cmd3_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='museumsTbl' and xtype='U') begin " +
                                 "Create Table museumsTbl(id int NOT NULL identity(1,1), museumName nvarchar(50) NOT NULL); " +
                                 "Create clustered Index museumsTbl on museumsTbl(id asc) " +
                                 "insert into museumsTbl values ('Topkapı Sarayı') " +
                                 "insert into museumsTbl values ('İstanbul Arkeoloji Muzesi') " +
                                 "insert into museumsTbl values ('Buyuk Saray Mozaikleri Muzesi') " +
                                 "insert into museumsTbl values ('Beylerbeyi Sarayı') " +
                                 "insert into museumsTbl values ('Dolmabahce Sarayı') " +
                                 "insert into museumsTbl values ('Deniz Muzesi') " +
                                 "insert into museumsTbl values ('Havacilik Muzesi') " +
                                 "insert into museumsTbl values ('Panorama 1453 Fetih Muzesi') " +
                                 "insert into museumsTbl values ('Yerebatan Sarnici') " +
                                 "insert into museumsTbl values ('Yildiz Sarayi') end";
            SqlCommand cmd = new SqlCommand(cmd3_query1, dbConnection);
            cmd.ExecuteNonQuery();

            string cmd4_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='citizenshipStateTbl' and xtype='U') begin ";
            string cmd4_query2 = "CREATE TABLE [dbo].[citizenshipStateTbl]([id] [bit] NULL,[isCitizen] [nchar](10) NULL) ON [PRIMARY] ";
            string cmd4_query3 = "insert into citizenshipStateTbl (id,isCitizen) values (0,'turist');";
            string cmd4_query4 = "insert into citizenshipStateTbl (id,isCitizen) values (1,'vatandaş'); end";
            SqlCommand cmd2 = new SqlCommand(cmd4_query1 + cmd4_query2 + cmd4_query3 + cmd4_query4, dbConnection);
            cmd2.ExecuteNonQuery();


            dbConnection.Close();

            try { cmd.ExecuteNonQuery(); } catch (Exception ex) { }
        }
        public void IndexOlusturucu()
        {

            SqlCommand cmd = new SqlCommand("if not EXISTS(select name From sys.indexes where name= 'IX_weekcrd' )\n" +
                                            "begin\n" +
                                            "    Create Index  IX_weekcrd on weekCardTbl (age asc)\n" +
                                            "end;\n" +
                                            "if not EXISTS(select name From sys.indexes where name= 'IX_nonclustered_yearcrd' )\n" +
                                            "begin\n" +
                                            "    Create nonclustered Index  IX_nonclustered_yearcrd \n" +
                                            "    on yearCardTbl (age asc)\n" +
                                            "end;\n" +
                                            "if not EXISTS(select name From sys.indexes where name= 'IX_unique_museums' )\n" +
                                            "begin\n" +
                                            "    create unique index  IX_unique_museums\n" +
                                            "    on museumsTbl (museumName asc);\n" +
                                            "end;", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }

        /**********************************************************************************        VİEWLAR       ******************************************************************************/
        public void UyeDagilimiView()
        {

            SqlCommand cmd = new SqlCommand("Create or alter View GenelUyeDagilimi\n" +
                                            "as\n" +
                                            "SELECT citizenshipStateTbl.isCitizen,cardTypeTbl.typeName,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                                    "JOIN cardTypeTbl ON weekCardTbl.cardType=cardTypeTbl.id\n" +
                                                    "JOIN citizenshipStateTbl ON weekCardTbl.isTurkishCitizen=citizenshipStateTbl.id GROUP BY typeName,isCitizen \n" +
                                                    "union\n" +
                                                    "SELECT citizenshipStateTbl.isCitizen,cardTypeTbl.typeName,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                                    "JOIN cardTypeTbl ON monthCardTbl.cardType=cardTypeTbl.id\n" +
                                                    "JOIN citizenshipStateTbl ON monthCardTbl.isTurkishCitizen=citizenshipStateTbl.id GROUP BY typeName,isCitizen\n" +
                                                    "union\n" +
                                                    "SELECT citizenshipStateTbl.isCitizen,cardTypeTbl.typeName,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                                    "JOIN cardTypeTbl ON yearCardTbl.cardType=cardTypeTbl.id\n" +
                                                    "JOIN citizenshipStateTbl ON yearCardTbl.isTurkishCitizen=citizenshipStateTbl.id GROUP BY typeName,isCitizen", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void TuristUlkeDagilimView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View TuristUlkeDagilimi\n" +
                                           "as\n" +
                                           "SELECT country,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                               "where isTurkishCitizen =0 group by country\n" +
                                               "union\n" +
                                               "SELECT country,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                               "where isTurkishCitizen =0 group by country\n" +
                                               "union\n" +
                                               "SELECT country,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                               "where isTurkishCitizen =0 group by country", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void UyeYasDagilimiView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View UyeYasDagilimi\n" +
                                                "as\n" +
                                                "SELECT isTurkishCitizen,age,SUM(totalMember) as totalMember from(\n" +
                                                    "SELECT isTurkishCitizen,age,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl GROUP BY age,isTurkishCitizen\n" +
                                                    "union\n" +
                                                    "SELECT isTurkishCitizen,age,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl GROUP BY age,isTurkishCitizen\n" +
                                                    "union\n" +
                                                    "SELECT isTurkishCitizen,age,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl GROUP BY age,isTurkishCitizen\n" +
                                                    ")t\n" +
                                                    "group by age,isTurkishCitizen", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void VatandasilDagilimiView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View VatandasIlDagilimi\n" +
                                            "as\n" +
                                            "SELECT city,sum(totalMember) as totalMember from\n" +
                                            "(\n" +
                                            "    SELECT city,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                            "    where isTurkishCitizen =1 group by city\n" +
                                            "    union\n" +
                                            "    SELECT city,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                            "    where isTurkishCitizen =1 group by city\n" +
                                            "    union\n" +
                                           "     SELECT city,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                           "     where isTurkishCitizen =1 group by city\n" +
                                            ")t\n" +
                                               " GROUP BY city ", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void MuzeUyeSayilariView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View MuzeUyeSayilari\n" +
                                            "as\n" +
                                               " SELECT museumName,sum(totalMember) as totalMember from(\n" +
                                                    "SELECT museumName, COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                                    "JOIN museumsTbl ON weekCardTbl.registeredMuseum = museumsTbl.id\n" +
                                                    "group by museumName\n" +
                                                    "union\n" +
                                                    "SELECT museumName, COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                                    "JOIN museumsTbl ON monthCardTbl.registeredMuseum = museumsTbl.id\n" +
                                                    "group by museumName\n" +
                                                    "union\n" +
                                                    "SELECT museumName, COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                                    "JOIN museumsTbl ON yearCardTbl.registeredMuseum = museumsTbl.id\n" +
                                                    "group by museumName\n" +
                                                    ")t\n" +
                                                    "GROUP BY museumName", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void BirHaftalikKayitView()
        {
            SqlCommand cmd = new SqlCommand("CREATE or alter View BirHaftalikKayit\n" +
                                                "as\n" +
                                                   " SELECT isTurkishCitizen,cardStartDate,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                                    "group by cardStartDate,isTurkishCitizen\n" +
                                                    "union\n" +
                                                    "SELECT isTurkishCitizen,cardStartDate,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                                    "group by cardStartDate,isTurkishCitizen\n" +
                                                    "union\n" +
                                                    "SELECT isTurkishCitizen,cardStartDate,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                                    "group by cardStartDate,isTurkishCitizen", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void VatandasUyeCinsiyetDagilimiView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View VatandasUyeCinsiyetDagilimi\n" +
                                                "as\n" +
                                                "SELECT cardTypeTbl.typeName,gender,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                                    "JOIN cardTypeTbl ON weekCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=1 \n" +
                                                    "GROUP BY typeName,gender \n" +
                                                    "union\n" +
                                                   " SELECT cardTypeTbl.typeName,gender,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                                   " JOIN cardTypeTbl ON monthCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=1\n" +
                                                   " GROUP BY typeName,gender	\n" +
                                                   " union\n" +
                                                   " SELECT cardTypeTbl.typeName,gender,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                                   " JOIN cardTypeTbl ON yearCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=1 \n" +
                                                    "GROUP BY typeName,gender", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }
        public void TuristUyeCinsiyetDagilimiView()
        {
            SqlCommand cmd = new SqlCommand("Create or alter View TuristUyeCinsiyetDagilimi\n" +
                                            "as\n" +
                                            "SELECT cardTypeTbl.typeName,gender,COUNT(weekCardTbl.id) as totalMember FROM weekCardTbl\n" +
                                                "JOIN cardTypeTbl ON weekCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=0\n" +
                                                "GROUP BY typeName,gender \n" +
                                                "union\n" +
                                                "SELECT cardTypeTbl.typeName,gender,COUNT(monthCardTbl.id) as totalMember FROM monthCardTbl\n" +
                                                "JOIN cardTypeTbl ON monthCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=0\n" +
                                                "GROUP BY typeName,gender	\n" +
                                                "union\n" +
                                                "SELECT cardTypeTbl.typeName,gender,COUNT(yearCardTbl.id) as totalMember FROM yearCardTbl\n" +
                                                "JOIN cardTypeTbl ON yearCardTbl.cardType=cardTypeTbl.id where isTurkishCitizen=0\n" +
                                                "GROUP BY typeName,gender", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();
        }

        public DataTable GenelUyeGrafigi()
        {


            SqlDataAdapter da = new SqlDataAdapter("SELECT *from(\n" +
                                                            "Select *from GenelUyeDagilimi\n" +
                                                        ")t\n" +
                                                        "order by case when typeName = 'haftalık' then 1\n" +
                                                                      "when typeName = 'aylık' then 2\n" +
                                                                     " when typeName = 'yıllık' then 3\n" +
                                                                 "end", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable YabancıUyeUlkeDağılımı()
        {

            SqlDataAdapter da = new SqlDataAdapter("SELECT country,sum(totalMember) as totalMember from(\n" +
                                                            "Select * from TuristUlkeDagilimi\n" +
                                                        ")t\n" +
                                                        "GROUP BY country\n" +
                                                        "ORDER BY totalMember desc", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable UyeYasDagilimi()
        {

            SqlDataAdapter da = new SqlDataAdapter("Select *from UyeYasDagilimi", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable VatandasUyeIlDagilimi()
        {

            SqlDataAdapter da = new SqlDataAdapter("  Select TOP (9) *from VatandasIlDagilimi order by totalMember desc", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable MuzeUyeSayilari()
        {

            SqlDataAdapter da = new SqlDataAdapter("Select *from MuzeUyeSayilari ORDER BY totalMember desc", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable HaftaKayitYogunlugu()
        {

            SqlDataAdapter da = new SqlDataAdapter("SELECT isTurkishCitizen,cardStartDate,sum(totalMember) as totalMember from(\n" +
                                                        "SELECT * from BirHaftalikKayit\n" +
                                                    ")t\n" +
                                                    "GROUP BY cardStartDate, isTurkishCitizen\n" +
                                                    "ORDER BY cardStartDate ", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable VatandasUyeCinsiyetDagilimi()
        {

            SqlDataAdapter da = new SqlDataAdapter("	SELECT *from(\n" +
                                                                "SELECT * from VatandasUyeCinsiyetDagilimi\n" +
                                                           " )t\n" +
                                                           " order by case when typeName = 'haftalık' then 1\n" +
                                                                         " when typeName = 'aylık' then 2\n" +
                                                                         " when typeName = 'yıllık' then 3\n" +
                                                                     "end; ", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable TuristUyeCinsiyetDagilimi()
        {

            SqlDataAdapter da = new SqlDataAdapter("	SELECT *from(\n" +
                                                                "SELECT * from TuristUyeCinsiyetDagilimi\n" +
                                                            ")t\n" +
                                                            "order by case when typeName = 'haftalık' then 1\n" +
                                                                          "when typeName = 'aylık' then 2\n" +
                                                                          "when typeName = 'yıllık' then 3\n" +
                                                                     "end; ", dbConnection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        /**********************************************************************INLINE AND MULTI STATEMENTS***************************************************************/

        public void Inlinestmnt_anabilgiweekcrd()
        {

            SqlCommand cmd = new SqlCommand("create or alter function fn_inlinestmnt_anabilgiweekcrd()\n" +
                                            "returns table\n" +
                                            "as\n" +
                                            "return (Select id,Upper(Left(firstName,1))+SUBSTRING(firstName,2,LEN(firstName)) as firstName,Upper(lastName) as lastName from weekCardTbl)", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void Inlinestmnt_anabilgimonthcrd()
        {

            SqlCommand cmd = new SqlCommand("create or alter function fn_inlinestmnt_anabilgimonthcrd()\n" +
                                            "returns table\n" +
                                            "as\n" +
                                            "return (Select id,Upper(Left(firstName,1))+SUBSTRING(firstName,2,LEN(firstName)) as firstName,Upper(lastName) as lastName from monthCardTbl)", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void Inlinestmnt_anabilgiyearcrd()
        {

            SqlCommand cmd = new SqlCommand("create or alter function fn_inlinestmnt_anabilgiyearcrd()\n" +
                                            "returns table\n" +
                                            "as\n" +
                                            "return (Select id, Upper(Left(firstName, 1)) + SUBSTRING(firstName, 2, LEN(firstName)) as firstName, Upper(lastName) as lastName from yearCardTbl)", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void Multistmnt_kartbilgiweekcrd()
        {

            SqlCommand cmd = new SqlCommand("Create or alter Function fn_multistmnt_kartbilgweekcrd()\n" +
                                            "returns @Table Table (id int,firstName nvarchar(50),lastName nvarchar(50),cardType nvarchar(50))\n" +
                                            "as\n" +
                                            "Begin\n" +
                                            "insert into @Table\n" +
                                            "Select weekCardTbl.id,firstName,lastName,typeName from weekCardTbl\n" +
                                            "join cardTypeTbl on weekCardTbl.cardType=cardTypeTbl.id\n" +

                                            "return\n" +
                                            "end", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void Multistmnt_kartbilgimonthcrd()
        {

            SqlCommand cmd = new SqlCommand("Create or alter Function fn_multistmnt_kartbilgmonthcrd()\n" +
                                            "returns @Table Table(id int, firstName nvarchar(50), lastName nvarchar(50), cardType nvarchar(50))\n" +
                                            "as\n" +
                                            "Begin\n" +
                                            "insert into @Table\n" +
                                            "Select monthCardTbl.id, firstName, lastName, typeName from monthCardTbl\n" +
                                            "join cardTypeTbl on monthCardTbl.cardType = cardTypeTbl.id\n" +

                                            "return\n" +
                                            "end", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }
        public void Multistmnt_kartbilgiyearcrd()
        {

            SqlCommand cmd = new SqlCommand("Create or alter Function fn_multistmnt_kartbilgyearcrd()\n" +
                                            "returns @Table Table(id int, firstName nvarchar(50), lastName nvarchar(50), cardType nvarchar(50))\n" +
                                            "as\n" +
                                            "Begin\n" +
                                            "insert into @Table\n" +
                                            "Select monthCardTbl.id, firstName, lastName, typeName from monthCardTbl\n" +
                                            "join cardTypeTbl on monthCardTbl.cardType = cardTypeTbl.id\n" +

                                            "return\n" +
                                            "end", dbConnection);
            dbConnection.Open();
            cmd.ExecuteNonQuery();
            dbConnection.Close();

        }

    }

}
