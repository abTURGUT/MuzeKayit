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
    class LoginDB
    {
        public SqlConnection loginConnection = new SqlConnection("Data Source=.;Initial Catalog=master;Integrated Security=True");

        public LoginDB()
        {
            GetBasicSettings();
        }

        private void GetBasicSettings()
        {

            SqlConnection connection;
            connection = loginConnection;
            connection.Open();

            //LoginDB veritabanını oluşturma
            string cmd1_query1 = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LoginDB') BEGIN ";
            string cmd1_query2 = "CREATE DATABASE LoginDB; END;";
            SqlCommand cmd1 = new SqlCommand(cmd1_query1 + cmd1_query2, connection);
            try { cmd1.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //use fonksiyonu
            string cmd_use = "use [LoginDB]"; cmd1 = new SqlCommand(cmd_use, connection); cmd1.ExecuteNonQuery();

            //loginMuseumTbl1 tablosunu oluşturma (identity ve unique kullanımı)
            string cmd2_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='loginMuseumTbl1' and xtype='U') begin ";
            string cmd2_query2 = "Create Table loginMuseumTbl1(Museum_ID int NOT NULL identity(1,1) Primary Key,Email nvarchar(50) NOT NULL, Password nvarchar(50) NOT NULL) ";
            string cmd2_query3 = "alter table loginMuseumTbl1 add constraint loginMuseumTbl1_unique_mail unique(Email) end;";
            SqlCommand cmd2 = new SqlCommand(cmd2_query1 + cmd2_query2 + cmd2_query3, connection);
            try { cmd2.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //loginMuseumTbl2 tablosunu oluşturma (identity ve unique kullanımı)
            string cmd3_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='loginMuseumTbl2' and xtype='U') begin ";
            string cmd3_query2 = "Create Table loginMuseumTbl2(ID int NOT NULL identity(1,1), MuseumName nvarchar(50) NOT NULL); ";
            string cmd3_query3 = "alter table loginMuseumTbl2 add constraint loginMuseumTbl1_unique_museumname unique(MuseumName) ";
            string cmd3_query4 = "Create clustered Index loginMuseumTbl2 on loginMuseumTbl2(ID asc) end";
            SqlCommand cmd3 = new SqlCommand(cmd3_query1 + cmd3_query2 + cmd3_query3 + cmd3_query4, connection);
            try { cmd3.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //giriş durumu tablosu oluşturma (default kullanımı)
            string cmd11_query1 = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='loginStatusTbl' and xtype='U') begin Create Table loginStatusTbl(ID int primary key, loginStatus int default 0)  " +
                                  "insert into loginStatusTbl(ID) values(-3) end;";
            SqlCommand cmd11 = new SqlCommand(cmd11_query1, connection);
            try { cmd11.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //mail oluşturma procedure'ü (lower,replace ve concat kullanımı)
            string cmd4_query1 = "create or alter procedure CreateMuseumMail " +
                                 "@MuseumID1 nvarchar(100), " +
                                 "@Email nvarchar(100) output " +
                                 "as begin " +
                                 "declare @MuseumName1 varchar(100) " +
                                 "select @MuseumName1 = MuseumName from loginMuseumTbl2 where ID = @MuseumID1 " +
                                 "set @Email = concat(replace(lower(@MuseumName1),' ', '_'),'@kultur.gov.tr') end;";
            SqlCommand cmd4 = new SqlCommand(cmd4_query1, connection);
            try { cmd4.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //şifre oluşturma procedure'ü (lower,substring, charindex ve concat kullanımı)
            string cmd5_query1 = "create or alter procedure CreateMuseumPassword " +
                                 "@MuseumID2 nvarchar(100), " +
                                 "@Password nvarchar(100) output " +
                                 "as begin " +
                                 "declare @MuseumName varchar(100) " +
                                 "select @MuseumName = MuseumName from loginMuseumTbl2 where ID = @MuseumID2 " +
                                 "set @Password = concat(substring(lower(@MuseumName),0,charindex(' ',@MuseumName)),'123') end;";
            SqlCommand cmd5 = new SqlCommand(cmd5_query1, connection);
            try { cmd5.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //mail syntax kontrol procedure'ü
            // 0 -> hatalı syntax
            // 1 -> doğru syntax
            string cmd6_query1 = "create or alter procedure CheckMailSyntax " +
                                 "@Mail2 nvarchar(100) " +
                                 "as begin " +
                                 "if(CHARINDEX('@',@Mail2,0) = 0 or PATINDEX('%.gov.tr',@Mail2) = 0) " +
                                 "begin return 0 end " +
                                 "else " +
                                 "begin return 1 end " +
                                 "end; ";
            SqlCommand cmd6 = new SqlCommand(cmd6_query1, connection);
            try { cmd6.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //mail ve şifre kontrol procedure'ü
            // -3 -> default
            // -2 -> hatalı syntax
            // -1 -> hatalı mail
            //  0 -> hatalı şifre
            //  1 -> doğru bilgiler
            string cmd7_query1 = "create or alter procedure CheckMailAndPassword " +
                                 "@MuseumID nvarchar(100), " +
                                 "@Mail nvarchar(100), " +
                                 "@Password nvarchar(100) " +
                                 "as begin " +
                                 "Declare @mailSyntax int " +
                                 "exec @mailSyntax = CheckMailSyntax @Mail " +
                                 "if(@mailSyntax = 0) begin update loginStatusTbl set loginStatus = -2 return -2 end " +
                                 "Declare @returnMail nvarchar(100) " +
                                 "select @returnMail = Email from loginMuseumTbl1 where Museum_ID = @MuseumID " +
                                 "if(@returnMail = @Mail) " +
                                 "begin " +
                                 "Declare @returnPassword nvarchar(100) " +
                                 "select @returnPassword = Password from loginMuseumTbl1 where Museum_ID = @MuseumID " +
                                 "if(@returnPassword = @Password) " +
                                 "begin update loginStatusTbl set loginStatus = 1; return 1 end;  " +
                                 "else begin update loginStatusTbl set loginStatus = 0; return 0 end;  " +
                                 "end " +
                                 "else begin update loginStatusTbl set loginStatus = -1; return -1 end; " +
                                 "end ";
            SqlCommand cmd7 = new SqlCommand(cmd7_query1, connection);
            try { cmd7.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //loginMuseumTbl2 tablsundaki verileri giriyoruz.
            string cmd8_query1 = "if((select count(ID) from loginMuseumTbl2) < 1) begin " +
                                 "insert into loginMuseumTbl2 values ('Topkapı Sarayı') " +
                                 "insert into loginMuseumTbl2 values ('İstanbul Arkeoloji Muzesi') " +
                                 "insert into loginMuseumTbl2 values ('Buyuk Saray Mozaikleri Muzesi') " +
                                 "insert into loginMuseumTbl2 values ('Beylerbeyi Sarayı') " +
                                 "insert into loginMuseumTbl2 values ('Dolmabahce Sarayı') " +
                                 "insert into loginMuseumTbl2 values ('Deniz Muzesi') " +
                                 "insert into loginMuseumTbl2 values ('Havacilik Muzesi') " +
                                 "insert into loginMuseumTbl2 values ('Panorama 1453 Fetih Muzesi') " +
                                 "insert into loginMuseumTbl2 values ('Yerebatan Sarnici') " +
                                 "insert into loginMuseumTbl2 values ('Yildiz Sarayi') end";
            SqlCommand cmd8 = new SqlCommand(cmd8_query1, connection);
            try { cmd8.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //loginMuseumTbl1 tablosundaki değerleri yukarıda oluşturduğum mail ve şifre oluşturma procedure'lerini kullanarak atıyoruz. (dbcc checkident kullanımı)
            string cmd9_query1 = "if((select count(Museum_ID) from loginMuseumTbl1) < 1) begin " +
                                 "declare @MuseumMail varchar(100) " +
                                 "declare @MuseumPassword varchar(100) " +
                                 "dbcc checkident(loginMuseumTbl1,reseed,1) " +
                                 "Declare @loopIX int " +
                                 "Set @loopIX = 1" +
                                 "While (@loopIX <=10) Begin " +
                                 "Execute CreateMuseumMail @loopIX,@MuseumMail output " +
                                 "Execute CreateMuseumPassword @loopIX,@MuseumPassword output " +
                                 "insert into loginMuseumTbl1 values (@MuseumMail,@MuseumPassword) " +
                                 "set @loopIX = @loopIX + 1; end; end;";
            SqlCommand cmd9 = new SqlCommand(cmd9_query1, connection);
            try { cmd9.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //foreign key kullanımı
            string cmd10_query1 = "IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'dbo.loginMuseumTbl2')) " +
                                "Alter Table loginMuseumTbl2 add constraint loginMuseumTbl2_ID_FK Foreign Key (ID) references loginMuseumTbl1(Museum_ID)";
            SqlCommand cmd10 = new SqlCommand(cmd10_query1, connection);
            try { cmd10.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            //id'den müze adı almak için procedure
            string cmd12_query1 = "create or alter procedure GetMuseumNameFromID " +
                                  "@MuseumID1 nvarchar(100) " +
                                  "as begin " +
                                  "Select Museum_ID,MuseumName " +
                                  "From loginMuseumTbl1 " +
                                  "JOIN loginMuseumTbl2 " +
                                  "ON loginMuseumTbl1.Museum_ID = loginMuseumTbl2.ID " +
                                  "where Museum_ID = @MuseumID1 " +
                                  "end";
            SqlCommand cmd12 = new SqlCommand(cmd12_query1, connection);
            try { cmd12.ExecuteNonQuery(); } catch (Exception ex) { MessageBox.Show(ex.Message); }

            connection.Close();
        }
        public int CheckLogin(int museumId, string mail, string password)
        {
            SqlConnection connection;
            connection = loginConnection;
            connection.Open();

            string cmd1_query1 = "use [LoginDB]"; SqlCommand cmd1 = new SqlCommand(cmd1_query1, connection); cmd1.ExecuteNonQuery();


            string cmd2_query1 = $"exec CheckMailAndPassword '{museumId.ToString()}','{mail}','{password}' ";
            SqlCommand cmd2 = new SqlCommand(cmd2_query1, connection);
            try
            {
                cmd2.ExecuteNonQuery();

                SqlCommand cmd1_read_query = new SqlCommand($"Select *from loginStatusTbl", connection);
                SqlDataReader read = cmd1_read_query.ExecuteReader();
                read.Read();
                int result = int.Parse(read["loginStatus"].ToString());
                read.Close();

                connection.Close();
                return result; 
            } 
            catch (Exception ex) { connection.Close(); MessageBox.Show(ex.Message); return -3; }
            

        }
    }
}
