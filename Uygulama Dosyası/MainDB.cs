using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace VeriTabaniProje
{
    class MainDB
    {
        public SqlConnection masterConnection = new SqlConnection("Data Source=.;Initial Catalog=master;Integrated Security=True");
        public SqlConnection mainConnection = new SqlConnection("Data Source=.;Initial Catalog=MainDB;Integrated Security=True");
        public SqlConnection loginConnection = new SqlConnection("Data Source=.;Initial Catalog=LoginDB;Integrated Security=True");

        public MainDB()
        {
            GetBasicSettings();
        }
        private void GetBasicSettings()
        {
            SqlConnection connection;
            connection = masterConnection;
            connection.Open();

            //LoginDB veritabanını oluşturma
            string cmd1_query1 = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MainDB') BEGIN " + 
                                 "CREATE DATABASE MainDB; END;";
            SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //use fonksiyonu
            string cmd_use = "use [MainDB]"; cmd1 = new SqlCommand(cmd_use, connection); cmd1.ExecuteNonQuery();

            //weekCardTbl tablosunu oluşturma 
            string cmd2_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='weekCardTbl' and xtype='U') begin " +
                                 "Create Table weekCardTbl(id int Identity(1,1) Primary Key,firstName VARCHAR(50) not null,lastName VARCHAR(50) not null,age INT not null,gender VARCHAR(50) not null,country VARCHAR(50) not null,city VARCHAR(50),isTurkishCitizen BIT not null,registeredMuseum INT not null,isCardActive BIT not null,cardType INT not null,cardStartDate DATE not null); " +
                                 "ALTER TABLE weekCardTbl ADD CONSTRAINT CK_weekCardTbl_Age CHECK (Age >= 15 AND Age <= 92) end";
            SqlCommand cmd2 = new SqlCommand(cmd2_query1, connection);
            try { cmd2.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunu oluşturma 
            string cmd3_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='monthCardTbl' and xtype='U') begin " +
                                 "Create Table monthCardTbl(id int Identity(1,1) Primary Key,firstName VARCHAR(50) not null,lastName VARCHAR(50) not null,age INT not null,gender VARCHAR(50) not null,country VARCHAR(50) not null,city VARCHAR(50),isTurkishCitizen BIT not null,registeredMuseum INT not null,isCardActive BIT not null,cardType INT not null,cardStartDate DATE not null); " +
                                 "ALTER TABLE monthCardTbl ADD CONSTRAINT CK_monthCardTbl_Age CHECK (Age >= 15 AND Age <= 92) end";
            SqlCommand cmd3 = new SqlCommand(cmd3_query1, connection);
            try { cmd3.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunu oluşturma 
            string cmd4_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='yearCardTbl' and xtype='U') begin " +
                                 "Create Table yearCardTbl(id int Identity(1,1) Primary Key,firstName VARCHAR(50) not null,lastName VARCHAR(50) not null,age INT not null,gender VARCHAR(50) not null,country VARCHAR(50) not null,city VARCHAR(50),isTurkishCitizen BIT not null,registeredMuseum INT not null,isCardActive BIT not null,cardType INT not null,cardStartDate DATE not null); " +
                                 "ALTER TABLE yearCardTbl ADD CONSTRAINT CK_yearCardTbl_Age CHECK (Age >= 15 AND Age <= 92) end";
            SqlCommand cmd4 = new SqlCommand(cmd4_query1, connection);
            try { cmd4.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //weekCardTbl tablosunda veri yoksa veri ekle
            SqlCommand cmd5_read_query = new SqlCommand($"Select *from weekCardTbl", connection);
            SqlDataReader read1 = cmd5_read_query.ExecuteReader();
            read1.Read();
            try { string result = read1["id"].ToString(); read1.Close(); } catch (Exception) { read1.Close(); PullAndWriteWeekDataToDB(connection);  }
            

            //monthCardTbl tablosunda veri yoksa veri ekle
            SqlCommand cmd6_read_query = new SqlCommand($"Select *from monthCardTbl", connection);
            SqlDataReader read2 = cmd6_read_query.ExecuteReader();
            read2.Read();
            try { string result = read2["id"].ToString(); read2.Close(); } catch (Exception) { read2.Close(); PullAndWriteMonthDataToDB(connection); }
            

            //yearCardTbl tablosunda veri yoksa veri ekle
            SqlCommand cmd7_read_query = new SqlCommand($"Select *from yearCardTbl", connection);
            SqlDataReader read3 = cmd7_read_query.ExecuteReader();
            read3.Read();
            try { string result = read3["id"].ToString(); read3.Close(); } catch (Exception) { read3.Close(); PullAndWriteYearDataToDB(connection); }


            //weekCardTbl tablosunda id ile selelect yapan fonksiyon
            string cmd8_query1 = "CREATE or alter FUNCTION SelectLocalWeekTable(@ID nvarchar(10)) " +
                                 "RETURNS TABLE as " +
                                 "RETURN (select *from weekCardTbl where registeredMuseum = @ID); ";
            SqlCommand cmd8 = new SqlCommand(cmd8_query1, connection);
            try { cmd8.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda id ile selelect yapan fonksiyon
            string cmd9_query1 = "CREATE or alter FUNCTION SelectLocalMonthTable(@ID nvarchar(10)) " +
                                 "RETURNS TABLE as " +
                                 "RETURN (select *from monthCardTbl where registeredMuseum = @ID); ";
            SqlCommand cmd9 = new SqlCommand(cmd9_query1, connection);
            try { cmd9.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda id ile selelect yapan fonksiyon
            string cmd10_query1 = "CREATE or alter FUNCTION SelectLocalYearTable(@ID nvarchar(10)) " +
                                 "RETURNS TABLE as " +
                                 "RETURN (select *from yearCardTbl where registeredMuseum = @ID); ";
            SqlCommand cmd10 = new SqlCommand(cmd10_query1, connection);
            try { cmd10.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            ////
            // İSME GÖRE (LOCAL)
            ////
            
            //weekCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd11_query1 = "create or alter proc SearchLocalWeekTableByName " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where registeredMuseum = @ID and firstName like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd11 = new SqlCommand(cmd11_query1, connection);
            try { cmd11.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd12_query1 = "create or alter proc SearchLocalMonthTableByName " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where registeredMuseum = @ID and firstName like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age desc end " +
                                  "end ";
            SqlCommand cmd12 = new SqlCommand(cmd12_query1, connection);
            try { cmd12.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd13_query1 = "create or alter proc SearchLocalYearTableByName " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where registeredMuseum = @ID and firstName like @likeWord + '%' end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and firstName like @likeWord + '%') order by age desc end " +
                                  "end ";
            SqlCommand cmd13 = new SqlCommand(cmd13_query1, connection);
            try { cmd13.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            ////
            // ÜLKEYE GÖRE (LOCAL)
            ////

            //weekCardTbl tablosunda ülkeye göre arama ve sıralama yapan procedure
            string cmd14_query1 = "create or alter proc SearchLocalWeekTableByCountry " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where registeredMuseum = @ID and country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd14 = new SqlCommand(cmd14_query1, connection);
            try { cmd14.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda ülkeye göre arama ve sıralama yapan procedure
            string cmd15_query1 = "create or alter proc SearchLocalMonthTableByCountry " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where registeredMuseum = @ID and country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd15 = new SqlCommand(cmd15_query1, connection);
            try { cmd15.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda ülkeye göre arama ve sıralama yapan procedure
            string cmd16_query1 = "create or alter proc SearchLocalYearTableByCountry " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where registeredMuseum = @ID and country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and country like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd16 = new SqlCommand(cmd16_query1, connection);
            try { cmd16.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            ////
            // KART TİPİNE GÖRE (LOCAL)
            ////

            //weekCardTbl tablosunda kart tipine göre arama ve sıralama yapan procedure
            string cmd17_query1 = "create or alter proc SearchLocalWeekTableByCardType " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where registeredMuseum = @ID and cardType like @likeWord + '%' end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd17 = new SqlCommand(cmd17_query1, connection);
            try { cmd17.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda kart tipine göre arama ve sıralama yapan procedure
            string cmd18_query1 = "create or alter proc SearchLocalMonthTableByCardType " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where registeredMuseum = @ID and cardType like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd18 = new SqlCommand(cmd18_query1, connection);
            try { cmd18.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda kart tipine göre arama ve sıralama yapan procedure
            string cmd19_query1 = "create or alter proc SearchLocalYearTableByCardType " +
                                  "@ID nvarchar(10),@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where registeredMuseum = @ID and cardType like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where (registeredMuseum = @ID and cardType like @likeWord + '%') order by age desc end " +
                                  "end; ";
            SqlCommand cmd19 = new SqlCommand(cmd19_query1, connection);
            try { cmd19.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            ////
            // İSME GÖRE (TÜM)
            ////

            //weekCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd20_query1 = "create or alter proc SearchAllWeekTableByName " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where firstName like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where firstName like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where firstName like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where firstName like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where firstName like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd20 = new SqlCommand(cmd20_query1, connection);
            try { cmd20.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd21_query1 = "create or alter proc SearchAllMonthTableByName " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where firstName like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where firstName like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where firstName like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where firstName like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where firstName like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd21 = new SqlCommand(cmd21_query1, connection);
            try { cmd21.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd22_query1 = "create or alter proc SearchAllYearTableByName " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where firstName like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where firstName like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where firstName like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where firstName like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where firstName like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd22 = new SqlCommand(cmd22_query1, connection);
            try { cmd22.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            ////
            // ÜLKEYE GÖRE (TÜM)
            ////

            //weekCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd23_query1 = "create or alter proc SearchAllWeekTableByCountry " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where country like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where country like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where country like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where country like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd23 = new SqlCommand(cmd23_query1, connection);
            try { cmd23.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd24_query1 = "create or alter proc SearchAllMonthTableByCountry " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where country like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where country like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where country like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where country like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd24 = new SqlCommand(cmd24_query1, connection);
            try { cmd24.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd25_query1 = "create or alter proc SearchAllYearTableByCountry " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where country like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where country like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where country like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where country like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where country like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd25 = new SqlCommand(cmd25_query1, connection);
            try { cmd25.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            ////
            // KART TİPİNE GÖRE (TÜM)
            ////

            //weekCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd26_query1 = "create or alter proc SearchAllWeekTableByCardType " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from weekCardTbl where cardType like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from weekCardTbl where cardType like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from weekCardTbl where cardType like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from weekCardTbl where cardType like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from weekCardTbl where cardType like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd26 = new SqlCommand(cmd26_query1, connection);
            try { cmd26.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //monthCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd27_query1 = "create or alter proc SearchAllMonthTableByCardType " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from monthCardTbl where cardType like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from monthCardTbl where cardType like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from monthCardTbl where cardType like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from monthCardTbl where cardType like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from monthCardTbl where cardType like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd27 = new SqlCommand(cmd27_query1, connection);
            try { cmd27.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //yearCardTbl tablosunda isme göre arama ve sıralama yapan procedure
            string cmd28_query1 = "create or alter proc SearchAllYearTableByCardType " +
                                  "@likeWord nvarchar(100),@sortNO int " +
                                  "as begin " +
                                  "if(@sortNO = 0) " +
                                  "begin select *from yearCardTbl where cardType like @likeWord + '%'  end " +
                                  "else if(@sortNO = 1) " +
                                  "begin select *from yearCardTbl where cardType like @likeWord + '%' order by firstName asc end " +
                                  "else if(@sortNO = 2) " +
                                  "begin select *from yearCardTbl where cardType like @likeWord + '%' order by firstName desc end " +
                                  "else if(@sortNO = 3) " +
                                  "begin select *from yearCardTbl where cardType like @likeWord + '%' order by age asc end " +
                                  "else if(@sortNO = 4) " +
                                  "begin select *from yearCardTbl where cardType like @likeWord + '%' order by age desc end " +
                                  "end; ";
            SqlCommand cmd28 = new SqlCommand(cmd28_query1, connection);
            try { cmd28.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //
            // INSERT fonksiyonları
            //

            //week table
            string cmd29_query1 = "create or alter procedure InsertDataInWeekTable " +
                                  "@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(day,7,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "insert into weekCardTbl values(@firstName,@lastName,@age,@gender,@country,@city,@isTurkishCitizen,@registeredMuseum,@isCardActive,@cardType,@startDate,@endDate) " +
                                  "end ";
            SqlCommand cmd29 = new SqlCommand(cmd29_query1, connection);
            try { cmd29.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //month table
            string cmd30_query1 = "create or alter procedure InsertDataInMonthTable " +
                                  "@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(month,1,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "insert into monthCardTbl values(@firstName,@lastName,@age,@gender,@country,@city,@isTurkishCitizen,@registeredMuseum,@isCardActive,@cardType,@startDate,@endDate) " +
                                  "end ";
            SqlCommand cmd30 = new SqlCommand(cmd30_query1, connection);
            try { cmd30.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //year table
            string cmd31_query1 = "create or alter procedure InsertDataInYearTable " +
                                  "@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(year,1,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "insert into yearCardTbl values(@firstName,@lastName,@age,@gender,@country,@city,@isTurkishCitizen,@registeredMuseum,@isCardActive,@cardType,@startDate,@endDate) " +
                                  "end ";
            SqlCommand cmd31 = new SqlCommand(cmd31_query1, connection);
            try { cmd31.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //
            // UPDATE fonksiyonları
            //

            //week table
            string cmd32_query1 = "CREATE OR ALTER PROCEDURE UpdateDataInWeekTable " +
                                  "@ID int,@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(year,1,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "UPDATE weekCardTbl SET firstName = @firstName, lastName = @lastName, age = @age, gender = @gender, country =@country, city =@city,isTurkishCitizen = @isTurkishCitizen, registeredMuseum = @registeredMuseum, isCardActive = @isCardActive, cardType = @cardType, cardStartDate = @startDate, cardEndDate = @endDate where id = @ID " +
                                  "end ";
            SqlCommand cmd32 = new SqlCommand(cmd32_query1, connection);
            try { cmd32.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //month table
            string cmd33_query1 = "CREATE OR ALTER PROCEDURE UpdateDataInMonthTable " +
                                  "@ID int,@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(year,1,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "UPDATE monthCardTbl SET firstName = @firstName, lastName = @lastName, age = @age, gender = @gender, country =@country, city =@city,isTurkishCitizen = @isTurkishCitizen, registeredMuseum = @registeredMuseum, isCardActive = @isCardActive, cardType = @cardType, cardStartDate = @startDate, cardEndDate = @endDate where id = @ID " +
                                  "end ";
            SqlCommand cmd33 = new SqlCommand(cmd33_query1, connection);
            try { cmd33.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //year table
            string cmd34_query1 = "CREATE OR ALTER PROCEDURE UpdateDataInYearTable " +
                                  "@ID int,@firstName nvarchar(100),@lastName nvarchar(100),@age int,@gender nvarchar(20),@country nvarchar(100), " +
                                  "@city nvarchar(100),@isTurkishCitizen bit,@registeredMuseum int,@isCardActive BIT,@cardType INT " +
                                  "as begin " +
                                  "Declare @startDate date " +
                                  "Declare @endDate date " +
                                  "set @startDate = CONVERT(varchar, getdate(), 23) " +
                                  "set @endDate = DATEADD(year,1,@startDate) " +
                                  "if(len(@firstName) >= 2) begin set @firstName = upper(substring(@firstName,1,1)) + lower(substring(@firstName,2,len(@firstName) - 1)) end " +
                                  "if(len(@lastName) >= 2) begin set @lastName = upper(substring(@lastName,1,1)) + lower(substring(@lastName,2,len(@lastName) - 1)) end " +
                                  "if(len(@country) >= 2) begin set @country = upper(substring(@country,1,1)) + lower(substring(@country,2,len(@country) - 1)) end " +
                                  "if(len(@city) >= 2) begin set @city = upper(substring(@city,1,1)) + lower(substring(@city,2,len(@city) - 1)) end " +
                                  "UPDATE yearCardTbl SET firstName = @firstName, lastName = @lastName, age = @age, gender = @gender, country =@country, city =@city,isTurkishCitizen = @isTurkishCitizen, registeredMuseum = @registeredMuseum, isCardActive = @isCardActive, cardType = @cardType, cardStartDate = @startDate, cardEndDate = @endDate where id = @ID " +
                                  "end ";
            SqlCommand cmd34 = new SqlCommand(cmd34_query1, connection);
            try { cmd34.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //cardTypeTbl tablosunu oluşturma 
            string cmd35_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='cardTypeTbl' and xtype='U') begin " +
                                  "CREATE TABLE cardTypeTbl(id int,typeName nchar(10))  " +
                                  "insert into cardTypeTbl values(1,'haftalık') " +
                                  "insert into cardTypeTbl values(2,'aylık') " +
                                  "insert into cardTypeTbl values(3,'yıllık') " +
                                  "end";
            SqlCommand cmd35 = new SqlCommand(cmd35_query1, connection);
            try { cmd35.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //
            // SELECT fonksiyonları
            //


            //week table
            string cmd36_query1 = "CREATE OR ALTER PROCEDURE SelectWeekTable  " +
                                  "@ID int  " +
                                  "AS BEGIN " +
                                  "select weekCardTbl.id,firstName,lastName,age,gender,country,COALESCE(city,'-') as city,isTurkishCitizen,registeredMuseum, isCardActive, typeName  from weekCardTbl " +
                                  "JOIN cardTypeTbl " +
                                  "ON weekCardTbl.cardType = cardTypeTbl.id " +
                                  "where weekCardTbl.id = @ID " +
                                  "END ";
            SqlCommand cmd36 = new SqlCommand(cmd36_query1, connection);
            try { cmd36.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //month table
            string cmd37_query1 = "CREATE OR ALTER PROCEDURE SelectMonthTable  " +
                                  "@ID int  " +
                                  "AS BEGIN " +
                                  "select monthCardTbl.id,firstName,lastName,age,gender,country,COALESCE(city,'-') as city,isTurkishCitizen,registeredMuseum, isCardActive, typeName  from monthCardTbl " +
                                  "JOIN cardTypeTbl " +
                                  "ON monthCardTbl.cardType = cardTypeTbl.id " +
                                  "where monthCardTbl.id = @ID " +
                                  "END ";
            SqlCommand cmd37 = new SqlCommand(cmd37_query1, connection);
            try { cmd37.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //year table
            string cmd38_query1 = "CREATE OR ALTER PROCEDURE SelectYearTable  " +
                                  "@ID int  " +
                                  "AS BEGIN " +
                                  "select yearCardTbl.id,firstName,lastName,age,gender,country,COALESCE(city,'-') as city,isTurkishCitizen,registeredMuseum, isCardActive, typeName  from yearCardTbl " +
                                  "JOIN cardTypeTbl " +
                                  "ON yearCardTbl.cardType = cardTypeTbl.id " +
                                  "where yearCardTbl.id = @ID " +
                                  "END ";
            SqlCommand cmd38 = new SqlCommand(cmd38_query1, connection);
            try { cmd38.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //
            // DELETE fonksiyonları
            //

            //week table
            string cmd39_query1 = "CREATE OR ALTER PROCEDURE DeleteInWeekTable  " +
                                  "@ID int " +
                                  "AS BEGIN " +
                                  "DELETE FROM weekCardTbl WHERE id = @ID; " +
                                  "END ";
            SqlCommand cmd39 = new SqlCommand(cmd39_query1, connection);
            try { cmd39.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //month table
            string cmd40_query1 = "CREATE OR ALTER PROCEDURE DeleteInMonthTable  " +
                                  "@ID int " +
                                  "AS BEGIN " +
                                  "DELETE FROM monthCardTbl WHERE id = @ID; " +
                                  "END ";
            SqlCommand cmd40 = new SqlCommand(cmd40_query1, connection);
            try { cmd40.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //year table
            string cmd41_query1 = "CREATE OR ALTER PROCEDURE DeleteInYearTable  " +
                                  "@ID int " +
                                  "AS BEGIN " +
                                  "DELETE FROM yearCardTbl WHERE id = @ID; " +
                                  "END ";
            SqlCommand cmd41 = new SqlCommand(cmd41_query1, connection);
            try { cmd41.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            //
            // TRIGGER fonksiyonları
            //


            //week table
            string cmd42_query1 = "CREATE or alter TRIGGER InsertInWeek_Trigger  " +
                                  "ON weekCardTbl " +
                                  "for INSERT " +
                                  "AS " +
                                  "Begin " +
                                  "Declare @_fName nvarchar(100),@_lName nvarchar(100),@_age nvarchar(100),@_gender nvarchar(100),@_country nvarchar(100) " +
                                  "select *into #TempTable from inserted " +
                                  "select top 1 @_fName = firstName, @_lName = lastName, @_age = age, @_gender = gender, @_country = country  from #TempTable " +
                                  "Declare @_id int,@count int " +
                                  "select @_id = monthCardTbl.id,@count = Count(id) from monthCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from monthCardTbl where id = @_id end " +
                                  "select @_id = yearCardTbl.id,@count = Count(id) from yearCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from yearCardTbl where id = @_id end " +
                                  "end ";
            SqlCommand cmd42 = new SqlCommand(cmd42_query1, connection);
            try { cmd42.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //month table
            string cmd43_query1 = "CREATE or alter TRIGGER InsertInMonth_Trigger  " +
                                  "ON monthCardTbl " +
                                  "for INSERT " +
                                  "AS " +
                                  "Begin " +
                                  "Declare @_fName nvarchar(100),@_lName nvarchar(100),@_age nvarchar(100),@_gender nvarchar(100),@_country nvarchar(100) " +
                                  "select *into #TempTable from inserted " +
                                  "select top 1 @_fName = firstName, @_lName = lastName, @_age = age, @_gender = gender, @_country = country  from #TempTable " +
                                  "Declare @_id int,@count int " +
                                  "select @_id = weekCardTbl.id,@count = Count(id) from weekCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from weekCardTbl where id = @_id end " +
                                  "select @_id = yearCardTbl.id,@count = Count(id) from yearCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from yearCardTbl where id = @_id end " +
                                  "end ";
            SqlCommand cmd43 = new SqlCommand(cmd43_query1, connection);
            try { cmd43.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //year table
            string cmd44_query1 = "CREATE or alter TRIGGER InsertInYear_Trigger  " +
                                  "ON yearCardTbl " +
                                  "for INSERT " +
                                  "AS " +
                                  "Begin " +
                                  "Declare @_fName nvarchar(100),@_lName nvarchar(100),@_age nvarchar(100),@_gender nvarchar(100),@_country nvarchar(100) " +
                                  "select *into #TempTable from inserted " +
                                  "select top 1 @_fName = firstName, @_lName = lastName, @_age = age, @_gender = gender, @_country = country  from #TempTable " +
                                  "Declare @_id int,@count int " +
                                  "select @_id = weekCardTbl.id,@count = Count(id) from weekCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from weekCardTbl where id = @_id end " +
                                  "select @_id = monthCardTbl.id,@count = Count(id) from monthCardTbl where (firstName = @_fName and lastName = @_lName and age = @_age and gender = @_gender and country = @_country) group by id " +
                                  "if(@count >= 1) begin delete from monthCardTbl where id = @_id end " +
                                  "end ";
            SqlCommand cmd44 = new SqlCommand(cmd44_query1, connection);
            try { cmd44.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            connection.Close();
        }

        public string GetMuseumNameFromID(string id)
        {
            SqlConnection connection;
            connection = loginConnection;
            connection.Open();
            
            try
            {

                //id'den müze adı almak için procedure
                string cmd1_query1 = $"GetMuseumNameFromID {id}";
                SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
                SqlDataReader read = cmd1_read_query.ExecuteReader();
                read.Read();
                string result = read["MuseumName"].ToString();
                read.Close();
                connection.Close();
                return result;
            }
            catch (Exception ex) { MessageBox.Show("girdi"); connection.Close(); MessageBox.Show(ex.Message); return "-"; }
        }

        public void PullAndWriteWeekDataToDB(SqlConnection connection)
        {
            string add_column_q = "ALTER TABLE weekCardTbl ADD  cardEndDate DATE null;";
            SqlCommand add_column = new SqlCommand(add_column_q, connection);
            try { add_column.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            string path = Application.StartupPath + "\\Data\\weekData.txt";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string query = "";
            string text = sw.ReadLine();
            while (text != null)
            {
                try
                {
                    text = sw.ReadLine(); if (text == null) { continue; }
                    query += text + " ";
                }
                catch (Exception) { }
            }
            sw.Close();
            fs.Close();

            SqlCommand cmd1 = new SqlCommand(query, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void PullAndWriteMonthDataToDB(SqlConnection connection)
        {
            string add_column_q = "ALTER TABLE monthCardTbl ADD  cardEndDate DATE null;";
            SqlCommand add_column = new SqlCommand(add_column_q, connection);
            try { add_column.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            string path = Application.StartupPath + "\\Data\\monthData.txt";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string query = "";
            string text = sw.ReadLine();
            while (text != null)
            {
                try
                {
                    text = sw.ReadLine(); if (text == null) { continue; }
                    query += text + " ";
                }
                catch (Exception) { }
                
            }
            sw.Close();
            fs.Close();

            SqlCommand cmd1 = new SqlCommand(query, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void PullAndWriteYearDataToDB(SqlConnection connection)
        {
            string add_column_q = "ALTER TABLE yearCardTbl ADD  cardEndDate DATE null;";
            SqlCommand add_column = new SqlCommand(add_column_q, connection);
            try { add_column.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }


            string path = Application.StartupPath + "\\Data\\yearData.txt";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string query = "";
            string text = "";
            while (text != null)
            {
                try
                {
                    text = sw.ReadLine(); if(text == null) { continue; }
                    query += text + " ";
                    text = text.Substring(0, text.Length - 2);
                }
                catch (Exception) { }
            }
            sw.Close();
            fs.Close();

            SqlCommand cmd1 = new SqlCommand(query, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public Person GetSpecificWeekData(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"SelectWeekTable {id}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string firstName = read["firstName"].ToString();
            string lastName = read["lastName"].ToString();
            string age = read["age"].ToString();
            string gender = read["gender"].ToString();
            string country = read["country"].ToString();
            string city = read["city"].ToString();
            string isTurkishCitizen = read["isTurkishCitizen"].ToString();
            string registeredMuseum = read["registeredMuseum"].ToString();
            string isCardActive = read["isCardActive"].ToString();
            string cardType = read["typeName"].ToString().Trim();

            Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, "", "");
            read.Close();
            connection.Close();

            return person;
        }
        public Person GetSpecificMonthData(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"SelectMonthTable {id}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string firstName = read["firstName"].ToString();
            string lastName = read["lastName"].ToString();
            string age = read["age"].ToString();
            string gender = read["gender"].ToString();
            string country = read["country"].ToString();
            string city = read["city"].ToString();
            string isTurkishCitizen = read["isTurkishCitizen"].ToString();
            string registeredMuseum = read["registeredMuseum"].ToString();
            string isCardActive = read["isCardActive"].ToString();
            string cardType = read["typeName"].ToString().Trim();

            Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, "", "");
            read.Close();
            connection.Close();

            return person;
        }
        public Person GetSpecificYearData(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"SelectYearTable {id}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string firstName = read["firstName"].ToString();
            string lastName = read["lastName"].ToString();
            string age = read["age"].ToString();
            string gender = read["gender"].ToString();
            string country = read["country"].ToString();
            string city = read["city"].ToString();
            string isTurkishCitizen = read["isTurkishCitizen"].ToString();
            string registeredMuseum = read["registeredMuseum"].ToString();
            string isCardActive = read["isCardActive"].ToString();
            string cardType = read["typeName"].ToString().Trim();

            Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, "", "");
            read.Close();
            connection.Close();

            return person;
        }

        public List<Person> GetLocalData(string museumId, string likeWord, string searchNO, string sortNO)
        {
            List<Person> allPersonList = new List<Person>();
            List<Person> weekPersonList = GetLocalWeekData(museumId, likeWord, searchNO, sortNO);
            List<Person> monthPersonList = GetLocalMonthData(museumId, likeWord, searchNO, sortNO);
            List<Person> yearPersonList = GetLocalYearData(museumId, likeWord, searchNO, sortNO);

            allPersonList.AddRange(weekPersonList);
            allPersonList.AddRange(monthPersonList);
            allPersonList.AddRange(yearPersonList);

            return allPersonList;

        }
        public List<Person> GetLocalWeekData(string museumId,string likeWord,string searchNO,string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchLocalWeekTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchLocalWeekTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchLocalWeekTableByCardType"; }
            
            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{museumId}','{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {
                
                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id,firstName,lastName,age,gender,country,city,isTurkishCitizen,registeredMuseum,isCardActive,cardType,cardStartDate,cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }
        public List<Person> GetLocalMonthData(string museumId, string likeWord, string searchNO, string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchLocalMonthTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchLocalMonthTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchLocalMonthTableByCardType"; }

            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{museumId}','{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {

                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }
        public List<Person> GetLocalYearData(string museumId, string likeWord, string searchNO, string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchLocalYearTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchLocalYearTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchLocalYearTableByCardType"; }

            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{museumId}','{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {

                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }

        public List<Person> GetAllData(string likeWord, string searchNO, string sortNO)
        {
            List<Person> allPersonList = new List<Person>();
            List<Person> weekPersonList = GetAllWeekData(likeWord, searchNO, sortNO);
            List<Person> monthPersonList = GetAllMonthData(likeWord, searchNO, sortNO);
            List<Person> yearPersonList = GetAllYearData(likeWord, searchNO, sortNO);

            allPersonList.AddRange(weekPersonList);
            allPersonList.AddRange(monthPersonList);
            allPersonList.AddRange(yearPersonList);

            return allPersonList;

        }
        public List<Person> GetAllWeekData(string likeWord, string searchNO, string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchAllWeekTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchAllWeekTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchAllWeekTableByCardType"; }

            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {

                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }
        public List<Person> GetAllMonthData(string likeWord, string searchNO, string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchAllMonthTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchAllMonthTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchAllMonthTableByCardType"; }

            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {

                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }
        public List<Person> GetAllYearData(string likeWord, string searchNO, string sortNO)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string procedureName = "";

            if (searchNO == "0") { procedureName = "SearchAllYearTableByName"; }
            else if (searchNO == "1") { procedureName = "SearchAllYearTableByCountry"; }
            else if (searchNO == "2") { procedureName = "SearchAllYearTableByCardType"; }

            List<Person> personList = new List<Person>();
            string cmd1_query1 = $"{procedureName} '{likeWord}',{sortNO}";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            while (read.Read())
            {

                string id = read["id"].ToString();
                string firstName = read["firstName"].ToString();
                string lastName = read["lastName"].ToString();
                string age = read["age"].ToString();
                string gender = read["gender"].ToString();
                string country = read["country"].ToString();
                string city = read["city"].ToString();
                string isTurkishCitizen = read["isTurkishCitizen"].ToString();
                string registeredMuseum = read["registeredMuseum"].ToString();
                string isCardActive = read["isCardActive"].ToString();
                string cardType = read["cardType"].ToString();
                string cardStartDate = read["cardStartDate"].ToString();
                string cardEndDate = read["cardEndDate"].ToString();

                Person person = new Person(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType, cardStartDate, cardEndDate);
                personList.Add(person);
            }
            read.Close();
            connection.Close();

            return personList;
        }

        public string InsertDataInWeekTable(string firstName,string lastName,string age,string gender,string country,string city,string isTurkishCitizen,string registeredMuseum,string isCardActive,string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"InsertDataInWeekTable {firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }
        public string InsertDataInMonthTable(string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"InsertDataInMonthTable {firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }
        public string InsertDataInYearTable(string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"InsertDataInYearTable {firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }

        public string UpdateDataInWeekTable(string id,string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            id = $"'{id}'";
            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"UpdateDataInWeekTable {id},{firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }
        public string UpdateDataInMonthTable(string id,string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            id = $"'{id}'";
            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"UpdateDataInMonthTable {id},{firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }
        public string UpdateDataInYearTable(string id,string firstName, string lastName, string age, string gender, string country, string city, string isTurkishCitizen, string registeredMuseum, string isCardActive, string cardType)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            id = $"'{id}'";
            if (firstName.Length < 1) { firstName = "null"; } else { firstName = $"'{firstName}'"; }
            if (lastName.Length < 1) { lastName = "null"; } else { lastName = $"'{lastName}'"; }
            if (age.Length < 1) { age = "null"; } else { age = $"'{age}'"; }
            if (gender.Length < 1) { gender = "null"; } else { gender = $"'{gender}'"; }
            if (country.Length < 1) { country = "null"; } else { country = $"'{country}'"; }
            if (city.Length < 1) { city = "null"; } else { city = $"'{city}'"; }
            if (isTurkishCitizen.Length < 1) { isTurkishCitizen = "null"; } else { isTurkishCitizen = $"'{isTurkishCitizen}'"; }
            if (registeredMuseum.Length < 1) { registeredMuseum = "null"; } else { registeredMuseum = $"'{registeredMuseum}'"; }
            if (isCardActive.Length < 1) { isCardActive = "null"; } else { isCardActive = $"'{isCardActive}'"; }
            if (cardType.Length < 1) { cardType = "null"; } else { cardType = $"'{cardType}'"; }

            try
            {
                string cmd1_query1 = $"UpdateDataInYearTable {id},{firstName},{lastName},{age},{gender},{country},{city},{isTurkishCitizen},{registeredMuseum},{isCardActive},{cardType}";
                SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
                try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); return ex.Message; }

                connection.Close();

                return "1";
            }
            catch (Exception ex)
            {
                connection.Close();
                return ex.Message;
            }
        }
    
        public void DeleteInWeekTable(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"DeleteInWeekTable {id}";
            SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close();}

            connection.Close();
        }
        public void DeleteInMonthTable(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"DeleteInMonthTable {id}";
            SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); connection.Close(); }

            connection.Close();
        }
        public void DeleteInYearTable(string id)
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"DeleteInYearTable {id}";
            SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { connection.Close(); }

            connection.Close();
        }

        public string GetLastIdInWeekTable()
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"select IDENT_CURRENT ('weekCardTbl') as last_id";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string last_id = read["last_id"].ToString();

            read.Close();
            connection.Close();

            return last_id;
        }
        public string GetLastIdInMonthTable()
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"select IDENT_CURRENT ('monthCardTbl') as last_id";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string last_id = read["last_id"].ToString();

            read.Close();
            connection.Close();

            return last_id;
        }
        public string GetLastIdInYearTable()
        {
            SqlConnection connection;
            connection = mainConnection;
            connection.Open();

            string cmd1_query1 = $"select IDENT_CURRENT ('yearCardTbl') as last_id";
            SqlCommand cmd1_read_query = new SqlCommand(cmd1_query1, connection);
            SqlDataReader read = cmd1_read_query.ExecuteReader();
            read.Read();

            string last_id = read["last_id"].ToString();

            read.Close();
            connection.Close();

            return last_id;
        }

    }
}
