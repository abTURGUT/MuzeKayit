using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeriTabaniProje
{
    public partial class EditForm : Form
    {
        string id, cardType;
        MainDB mainDB = new MainDB();
        Person person;
        int mov, movX, movY;
        string selectedCardType = "1";
        string old_firstName, old_lastName, old_age, old_gender, old_country, old_city, old_isTurkishCitizen, old_registeredMuseum, old_isCardActive, old_cardType;
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        public EditForm(string id,string cardType)
        {
            this.id = id;
            this.cardType = cardType;
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

            ShowEditForm();
        }

        public void ShowEditForm()
        {
            SendMessage(r_firstNameTXT.Handle, EM_SETCUEBANNER, 0, "AD");
            SendMessage(r_lastNameTXT.Handle, EM_SETCUEBANNER, 0, "SOYAD");
            SendMessage(r_ageTXT.Handle, EM_SETCUEBANNER, 0, "YAŞ");
            SendMessage(r_countryTXT.Handle, EM_SETCUEBANNER, 0, "ÜLKE");
            SendMessage(r_cityTXT.Handle, EM_SETCUEBANNER, 0, "ŞEHİR");


            
            if (cardType == "1") { person = mainDB.GetSpecificWeekData(id); }
            else if (cardType == "2") { person = mainDB.GetSpecificMonthData(id); }
            else if (cardType == "3") { person = mainDB.GetSpecificYearData(id); }

            old_firstName = person.firstName;
            old_lastName = person.lastName;
            old_age = person.age;
            old_gender = person.gender;
            old_country = person.country;
            old_city = person.city;
            old_isTurkishCitizen = person.isTurkishCitizen;
            old_registeredMuseum = person.registeredMuseum;
            old_isCardActive = person.isCardActive;
            if(person.cardType == "haftalık") { old_cardType = "1"; }else if(person.cardType == "aylık") { old_cardType = "2"; } else { old_cardType = "3"; }


            r_firstNameTXT.Text = person.firstName;
            r_lastNameTXT.Text = person.lastName;
            r_ageTXT.Text = person.age;
            r_countryTXT.Text = person.country;
            r_cityTXT.Text = person.city;
            if(person.gender == "Male") { r_genderMaleBOX.Checked = true; r_genderFemaleBOX.Checked = false; }
            else { r_genderMaleBOX.Checked = false; r_genderFemaleBOX.Checked = true; }
            if (person.isTurkishCitizen == "True") { r_citizenTurkishBOX.Checked = true; r_citizenNonTurkishBOX.Checked = false; }
            else { r_citizenTurkishBOX.Checked = false; r_citizenNonTurkishBOX.Checked = true; }
            r_museumNameTXT.Text = person.registeredMuseum;
            if (person.cardType.Trim() == "haftalık") { r_weekCardTypeBOX.Checked = true; r_monthCardTypeCHECK.Checked = false; r_yearCardTypeCHECK.Checked = false; }
            else if (person.cardType.Trim() == "aylık") { r_weekCardTypeBOX.Checked = false; r_monthCardTypeCHECK.Checked = true; r_yearCardTypeCHECK.Checked = false; }
            else {  r_weekCardTypeBOX.Checked = false; r_monthCardTypeCHECK.Checked = false; r_yearCardTypeCHECK.Checked = true; }

        }
        public void SaveRecord()
        {
            
            string id = person.id.Trim();
            string firstName = r_firstNameTXT.Text.Trim();
            string lastName = r_lastNameTXT.Text.Trim();
            string age = r_ageTXT.Text;
            string gender = r_genderMaleBOX.Checked ? "Male" : "Female";
            string country = r_countryTXT.Text.Trim();
            string city = r_cityTXT.Text.Trim();
            string isTurkishCitizen = r_citizenTurkishBOX.Checked ? "True" : "False";
            string registeredMuseum = person.registeredMuseum.ToString().Trim();
            string isCardActive = "True";
            string cardType = selectedCardType.ToString().Trim();

            string result = "";

            //MessageBox.Show(old_cardType + " , " + old_cardType.Length.ToString() + "\n" + selectedCardType + " , " + selectedCardType.Length.ToString()); 
            if (old_cardType == selectedCardType)
            {
                if (cardType == "1") { result = mainDB.UpdateDataInWeekTable(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
                else if (cardType == "2") { result = mainDB.UpdateDataInMonthTable(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
                else if (cardType == "3") { result = mainDB.UpdateDataInYearTable(id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
            }
            else
            {
                if (selectedCardType == "1") { result = mainDB.InsertDataInWeekTable(old_firstName, old_lastName, old_age, old_gender, old_country, old_city, old_isTurkishCitizen, old_registeredMuseum, old_isCardActive, old_cardType); }
                else if (selectedCardType == "2") { result = mainDB.InsertDataInMonthTable(old_firstName, old_lastName, old_age, old_gender, old_country, old_city, old_isTurkishCitizen, old_registeredMuseum, old_isCardActive, old_cardType); }
                else if (selectedCardType == "3") { result = mainDB.InsertDataInYearTable(old_firstName, old_lastName, old_age, old_gender, old_country, old_city, old_isTurkishCitizen, old_registeredMuseum, old_isCardActive, old_cardType); }

                string last_id = "";
                if (selectedCardType == "1") { last_id = mainDB.GetLastIdInWeekTable(); }
                else if (selectedCardType == "2") { last_id = mainDB.GetLastIdInMonthTable(); }
                else if (selectedCardType == "3") { last_id = mainDB.GetLastIdInYearTable(); }


                if (selectedCardType == "1") { result = mainDB.UpdateDataInWeekTable(last_id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
                else if (selectedCardType == "2") { result = mainDB.UpdateDataInMonthTable(last_id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
                else if (selectedCardType == "3") { result = mainDB.UpdateDataInYearTable(last_id, firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
            }

            if (result != "1")
            {
                if (result.Contains("firstName")) { MessageBox.Show("İsim Boş Olamaz !"); }
                else if (result.Contains("lastName")) { MessageBox.Show("Soyisim Boş Olamaz !"); }
                else if (result.Contains("age")) { MessageBox.Show("Yaş 15-92 Arasında Girilmelidir !"); }
                else if (result.Contains("country")) { MessageBox.Show("Ülke Boş Olamaz !"); }
            }
            else { ((MainForm)Application.OpenForms["MainForm"]).GetLocalData(0); ((MainForm)Application.OpenForms["MainForm"]).GetAllData(0); this.Close(); }
        }
        public void DeleteRecord()
        {
            if (cardType == "1") { mainDB.DeleteInWeekTable(id); }
            else if (cardType == "2") { mainDB.DeleteInMonthTable(id); }
            else if (cardType == "3") { mainDB.DeleteInMonthTable(id); }
            else { MessageBox.Show("girmedi"); }
            ((MainForm)Application.OpenForms["MainForm"]).GetLocalData(0); ((MainForm)Application.OpenForms["MainForm"]).GetAllData(0); this.Close();
        }
        private void saveRecordBTN_Click(object sender, EventArgs e)
        {
            SaveRecord();
        }

        private void deleteRecordBTN_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void backBTN_Click(object sender, EventArgs e) { this.Close(); }

        private void r_weekCardTypeBOX_Click(object sender, EventArgs e)
        {
            selectedCardType = "1";
            r_weekCardTypeBOX.Checked = true;
            r_monthCardTypeCHECK.Checked = false;
            r_yearCardTypeCHECK.Checked = false;
        }

        private void r_monthCardTypeCHECK_Click(object sender, EventArgs e)
        {
            selectedCardType = "2";
            r_weekCardTypeBOX.Checked = false;
            r_monthCardTypeCHECK.Checked = true;
            r_yearCardTypeCHECK.Checked = false;
        }

        private void r_yearCardTypeCHECK_Click(object sender, EventArgs e)
        {
            selectedCardType = "3";
            r_weekCardTypeBOX.Checked = false;
            r_monthCardTypeCHECK.Checked = false;
            r_yearCardTypeCHECK.Checked = true;
        }

        private void r_citizenTurkishBOX_Click(object sender, EventArgs e)
        {
            r_citizenTurkishBOX.Checked = true;
            r_citizenNonTurkishBOX.Checked = false;
        }

        private void r_citizenNonTurkishBOX_Click(object sender, EventArgs e)
        {
            r_citizenTurkishBOX.Checked = false;
            r_citizenNonTurkishBOX.Checked = true;
        }

        private void r_genderMaleBOX_Click(object sender, EventArgs e)
        {
            r_genderMaleBOX.Checked = true;
            r_genderFemaleBOX.Checked = false;
        }

        private void r_genderFemaleBOX_Click(object sender, EventArgs e)
        {
            r_genderMaleBOX.Checked = false;
            r_genderFemaleBOX.Checked = true;

        }

        private void topBar_MouseDown(object sender, MouseEventArgs e) { mov = 1; movX = e.X; movY = e.Y; }

        private void topBar_MouseMove(object sender, MouseEventArgs e) { if (mov == 1) { this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY); } }

        private void topBar_MouseUp(object sender, MouseEventArgs e) { mov = 0; }
    }
}
