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
    public partial class MainForm : Form
    {
        LoginDB loginDB;
        MainDB mainDB;
        List<Person> personList;
        int minimumPicPanelX = 0, maximumPicPanelX = -3600, currentPicPanelX = 0, targetPicPanelX = 0, picPanelX = 400;
        bool isPicAnimationActive = false;
        int mov, movX, movY;
        int selectedMuseumId = 1, selectedPageNO = 1, selectedCardType = 1;
        string searchNo = "0";
        Label[] picTextList = new Label[10];
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);

        public MainForm()
        {
            
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            loginDB = new LoginDB();
            SetBasics();
            ShowMainPage();
            mainPagePANEL.Visible = false;
        }


        ////////////////
        //
        // Login Page
        //
        ////////////////

        private void SetBasics()
        {
            //this.Size = new Size(863, 617);
            //loginPanel.Location = new Point(30,50);

            SendMessage(mailTXT.Handle, EM_SETCUEBANNER, 0, "MAIL ADRESİ");
            SendMessage(passwordTXT.Handle, EM_SETCUEBANNER, 0, "ŞİFRE");

            picTextList[0] = topkapiLBL;
            picTextList[1] = arkeolojiLBL;
            picTextList[2] = saraymozaikleriLBL;
            picTextList[3] = beylerbeyiLBL;
            picTextList[4] = dolmabahceLBL;
            picTextList[5] = denizLBL;
            picTextList[6] = havacilikLBL;
            picTextList[7] = panaromaLBL;
            picTextList[8] = yerebatanLBL;
            picTextList[9] = yildizLBL;
            PictureTextAnimation();
        }

        //animasyon
        private void PictureSlideAnimation()
        {
            isPicAnimationActive = true;

            while (true)
            {
                if (targetPicPanelX < currentPicPanelX) { currentPicPanelX--; }
                if (targetPicPanelX > currentPicPanelX) { currentPicPanelX++; }
                if (targetPicPanelX == currentPicPanelX) { break; }

                pictureBox.Location = new Point(currentPicPanelX, pictureBox.Location.Y);

            }
            PictureTextAnimation();
            isPicAnimationActive = false;
        }
        private void PictureTextAnimation()
        {
            int ix = 0;

            if (currentPicPanelX == 0) { ix = 0; }
            else if (currentPicPanelX == -400) { ix = 1; }
            else if (currentPicPanelX == -800) { ix = 2; }
            else if (currentPicPanelX == -1200) { ix = 3; }
            else if (currentPicPanelX == -1600) { ix = 4; }
            else if (currentPicPanelX == -2000) { ix = 5; }
            else if (currentPicPanelX == -2400) { ix = 6; }
            else if (currentPicPanelX == -2800) { ix = 7; }
            else if (currentPicPanelX == -3200) { ix = 8; }
            else if (currentPicPanelX == -3600) { ix = 9; }

            for (int i = 0; i < 10; i++)
            {
                picTextList[i].Location = new Point(picTextList[i].Location.X, -70);
            }

            picTextList[ix].Location = new Point(picTextList[ix].Location.X, 50);
        }
        private void SuccessLoginAnimation()
        {
            int a = 863;
            int b = 617;
            int c = 863;
            int d = 829;
            int f = 792;
            while (a <= 1024)
            {
                this.Size = new Size(a, b);
                a++;
                if (b <= 732) { b++; }
                if (c <= 1024) { topBar.Size = new Size(c, topBar.Height); c++; }
                if (d <= 990) { closeFormBTN.Location = new Point(d, 0); d++; }
                if (f <= 953) { hideWindowBTN.Location = new Point(f, 0); f++; }
            }

            Task.Run(() =>
            {
                SetMainPageBasics();
                leftPANEL.Location = new Point(0,20);
                mainPagePANEL.Visible = true;
            });
        }

        //resim animasyon
        private void nextPicBTN_Click(object sender, EventArgs e)
        {
            if (targetPicPanelX - picPanelX >= maximumPicPanelX) { targetPicPanelX -= picPanelX; if (!isPicAnimationActive) { selectedMuseumId++; PictureSlideAnimation(); } }
            if (currentPicPanelX == -3600) { nextPicBTN.Enabled = false; } else { nextPicBTN.Enabled = true; }
            backPicBTN.Enabled = true;
        }
        private void backPicBTN_Click(object sender, EventArgs e)
        {
            if (targetPicPanelX + picPanelX <= minimumPicPanelX) { targetPicPanelX += picPanelX; if (!isPicAnimationActive) { selectedMuseumId--; PictureSlideAnimation(); } }
            if (currentPicPanelX == 0) { backPicBTN.Enabled = false; } else { backPicBTN.Enabled = true; }
            nextPicBTN.Enabled = true;
        }

        // kullanıcı giriş işlemleri
        private void Login()
        {
            int result = loginDB.CheckLogin(selectedMuseumId, mailTXT.Text, passwordTXT.Text);
            if (result == -2) { incorrectMailFormatLBL.Visible = true; wrongMailLBL.Visible = false; wrongPasswordLBL.Visible = false; successfulLoginLBL.Visible = false; }
            else if (result == -1) { incorrectMailFormatLBL.Visible = false; wrongMailLBL.Visible = true; wrongPasswordLBL.Visible = false; successfulLoginLBL.Visible = false; }
            else if (result == 0) { incorrectMailFormatLBL.Visible = false; wrongMailLBL.Visible = false; wrongPasswordLBL.Visible = true; successfulLoginLBL.Visible = false; }
            else if (result == 1)
            {
                incorrectMailFormatLBL.Visible = false; wrongMailLBL.Visible = false; wrongPasswordLBL.Visible = false; successfulLoginLBL.Visible = true;
                Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    loginPANEL.Visible = false;
                    SuccessLoginAnimation();
                });
            }

        }
        private void loginBTN_Click(object sender, EventArgs e) { Login(); }
        private void showPasswordBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (showPasswordBOX.Checked) { passwordTXT.UseSystemPasswordChar = false; }
            else { passwordTXT.UseSystemPasswordChar = true; }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (mailTXT.Text.Length >= 5 && passwordTXT.Text.Length >= 5) { loginBTN.Enabled = true; loginBTN.ForeColor = Color.Green; loginBTN.Font = new Font(loginBTN.Font.FontFamily, 14); }
            else { loginBTN.Enabled = false; loginBTN.ForeColor = Color.Black; loginBTN.Font = new Font(loginBTN.Font.FontFamily, 12); }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (mailTXT.Text.Length >= 5 && passwordTXT.Text.Length >= 5) { loginBTN.Enabled = true; loginBTN.ForeColor = Color.Green; loginBTN.Font = new Font(loginBTN.Font.FontFamily, 14); }
            else { loginBTN.Enabled = false; loginBTN.ForeColor = Color.Black; loginBTN.Font = new Font(loginBTN.Font.FontFamily, 12); }
        }

        // top bar işlemleri
        private void closeFormBTN_Click(object sender, EventArgs e) { this.Close(); }
        private void hideWindowBTN_Click(object sender, EventArgs e) { this.WindowState = FormWindowState.Minimized; }
        private void topBar_MouseDown(object sender, MouseEventArgs e) { mov = 1; movX = e.X; movY = e.Y; }
        private void topBar_MouseMove(object sender, MouseEventArgs e) { if (mov == 1) { this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY); } }
        private void topBar_MouseUp(object sender, MouseEventArgs e) { mov = 0; }



        ////////////////
        //
        // Main Page
        //
        ////////////////

        public void ShowMainPage()
        {
            SetMainPageBasics();

            mainPageBTN.BackColor = Color.White; mainPageBTN.ForeColor = Color.MediumSeaGreen;
            newRecordBTN.BackColor = Color.MediumSeaGreen; newRecordBTN.ForeColor = Color.White;
            localRecordsBTN.BackColor = Color.MediumSeaGreen; localRecordsBTN.ForeColor = Color.White;
            allRecordsBTN.BackColor = Color.MediumSeaGreen; allRecordsBTN.ForeColor = Color.White;
            statisticsBTN.BackColor = Color.MediumSeaGreen; statisticsBTN.ForeColor = Color.White;

            mainPagePANEL.Visible = true;
            newRecordPANEL.Visible = false;
            localRecordsPANEL.Visible = false;
            allRecordsPANEL.Visible = false;
            statisticPANEL.Visible = false;

            mainPagePANEL.Location = new Point(243,26);
            newRecordPANEL.Location = new Point(243, 26);
            localRecordsPANEL.Location = new Point(243, 26);
            allRecordsPANEL.Location = new Point(243, 26);
            statisticPANEL.Location = new Point(243,26);

            weekCardIMG = Image.FromFile(Application.StartupPath + "\\Pictures\\haftalikCard.png");
            monthCardIMG = Image.FromFile(Application.StartupPath + "\\Pictures\\aylikCard.png");
            yearCardIMG = Image.FromFile(Application.StartupPath + "\\Pictures\\yillikCard.png");

            grayWeekCardIMG = GrayScaleFilter((Bitmap)weekCardIMG);
            grayMonthCardIMG = GrayScaleFilter((Bitmap)monthCardIMG);
            grayYearCardIMG = GrayScaleFilter((Bitmap)yearCardIMG);

            weekCardBTN.Image = weekCardIMG;
            monthCardBTN.Image = grayMonthCardIMG;
            yearCardBTN.Image = grayYearCardIMG;
        }
        public void SetMainPageBasics()
        {
            mainDB = new MainDB();
            string museumName = mainDB.GetMuseumNameFromID(selectedMuseumId.ToString());
            museumNameTXT.Text = museumName.ToUpper();
            int museumNamePositionX = int.Parse(((775 - museumNameTXT.Size.Width) / 2).ToString());
            museumNameTXT.Location = new Point(museumNamePositionX, museumNameTXT.Location.Y);
        }
        private void mainPageBTN_Click(object sender, EventArgs e)
        {
            ShowMainPage();
        }



        ////////////////
        //
        // New Record Page
        //
        ////////////////

        Image weekCardIMG, monthCardIMG, yearCardIMG;
        Image grayWeekCardIMG, grayMonthCardIMG, grayYearCardIMG;



        public void ShowNewRecordPage()
        {
            mainPageBTN.BackColor = Color.MediumSeaGreen; mainPageBTN.ForeColor = Color.White;
            newRecordBTN.BackColor = Color.White; newRecordBTN.ForeColor = Color.MediumSeaGreen;
            localRecordsBTN.BackColor = Color.MediumSeaGreen; localRecordsBTN.ForeColor = Color.White;
            allRecordsBTN.BackColor = Color.MediumSeaGreen; allRecordsBTN.ForeColor = Color.White;
            statisticsBTN.BackColor = Color.MediumSeaGreen; statisticsBTN.ForeColor = Color.White;

            mainPagePANEL.Visible = false;
            newRecordPANEL.Visible = true;
            localRecordsPANEL.Visible = false;
            allRecordsPANEL.Visible = false;
            statisticPANEL.Visible = false;

            weekCardBTN.Image = weekCardIMG;
            monthCardBTN.Image = grayMonthCardIMG;
            yearCardBTN.Image = grayYearCardIMG;

            SendMessage(r_firstNameTXT.Handle, EM_SETCUEBANNER, 0, "AD");
            SendMessage(r_lastNameTXT.Handle, EM_SETCUEBANNER, 0, "SOYAD");
            SendMessage(r_ageTXT.Handle, EM_SETCUEBANNER, 0, "YAŞ");
            SendMessage(r_countryTXT.Handle, EM_SETCUEBANNER, 0, "ÜLKE");
            SendMessage(r_cityTXT.Handle, EM_SETCUEBANNER, 0, "ŞEHİR");

            r_museumNameTXT.Text = mainDB.GetMuseumNameFromID(selectedMuseumId.ToString());
            r_cardTypeTXT.Text = "HAFTALIK";
            r_cardEndDateTXT.Text = GetCardEndDate(1);

        }
        private void saveRecordBTN_Click(object sender, EventArgs e)
        {
            string firstName = r_firstNameTXT.Text;
            string lastName = r_lastNameTXT.Text; 
            string age = r_ageTXT.Text; 
            string gender = r_genderMaleBOX.Checked ? "Male" : "Female"; 
            string country = r_countryTXT.Text; 
            string city = r_cityTXT.Text; 
            string isTurkishCitizen = r_citizenTurkishBOX.Checked ? "True" : "False"; 
            string registeredMuseum = selectedMuseumId.ToString();
            string isCardActive = "True"; 
            string cardType = selectedCardType.ToString();

            string result = "";
            if (cardType == "1") { result = mainDB.InsertDataInWeekTable(firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
            if (cardType == "2") { result = mainDB.InsertDataInMonthTable(firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }
            if (cardType == "3") { result = mainDB.InsertDataInYearTable(firstName, lastName, age, gender, country, city, isTurkishCitizen, registeredMuseum, isCardActive, cardType); }

            if (result != "1")
            {
                if (result.Contains("firstName")) { MessageBox.Show("İsim Boş Olamaz !"); }
                else if (result.Contains("lastName")) { MessageBox.Show("Soyisim Boş Olamaz !"); }
                else if (result.Contains("age")) { MessageBox.Show("Yaş 15-92 Arasında Girilmelidir !"); }
                else if (result.Contains("country")) { MessageBox.Show("Ülke Boş Olamaz !"); }
            }
            else { MessageBox.Show("Kayıt Başarıyla Eklendi !"); }
            
        }
        private void newRecordBTN_Click(object sender, EventArgs e)
        {
            ShowNewRecordPage();
        }
        private void weekCardBTN_Click(object sender, EventArgs e) { selectedCardType = 1; r_cardTypeTXT.Text = "HAFTALIK"; r_cardEndDateTXT.Text = GetCardEndDate(1); weekCardBTN.Image = weekCardIMG; monthCardBTN.Image = grayMonthCardIMG; yearCardBTN.Image = grayYearCardIMG; }
        private void monthCardBTN_Click(object sender, EventArgs e) { selectedCardType = 2; r_cardTypeTXT.Text = "AYLIK"; r_cardEndDateTXT.Text = GetCardEndDate(2); weekCardBTN.Image = grayWeekCardIMG; monthCardBTN.Image = monthCardIMG; yearCardBTN.Image = grayYearCardIMG; }
        private void yearCardBTN_Click(object sender, EventArgs e) { selectedCardType = 3; r_cardTypeTXT.Text = "YILLIK"; r_cardEndDateTXT.Text = GetCardEndDate(3); weekCardBTN.Image = grayWeekCardIMG; monthCardBTN.Image = grayMonthCardIMG; yearCardBTN.Image = yearCardIMG; }

        private void r_genderMaleBOX_Click(object sender, EventArgs e) { r_genderMaleBOX.Checked = true; r_genderFemaleBOX.Checked = false; }
        private void r_genderFemaleBOX_Click(object sender, EventArgs e) { r_genderMaleBOX.Checked = false; r_genderFemaleBOX.Checked = true; }
        private void r_citizenTurkishBOX_Click(object sender, EventArgs e) { r_citizenTurkishBOX.Checked = true; r_citizenNonTurkishBOX.Checked = false; }
        private void r_citizenNonTurkishBOX_Click(object sender, EventArgs e) { r_citizenTurkishBOX.Checked = false; r_citizenNonTurkishBOX.Checked = true; }
        
        public string GetCardEndDate(int cardType)
        {
            DateTime dt = DateTime.Now;

            if (cardType == 1) { dt = dt.AddDays(7); }
            else if (cardType == 2) { dt = dt.AddMonths(1); }
            else if (cardType == 3) { dt = dt.AddYears(1); }

            string stringDateTime = dt.ToString("dd MMMM yyyy");
            return stringDateTime;
        }
        public Bitmap GrayScaleFilter(Bitmap image)
        {
            Bitmap grayScale = new Bitmap(image.Width, image.Height);

            for (Int32 y = 0; y < grayScale.Height; y++)
                for (Int32 x = 0; x < grayScale.Width; x++)
                {
                    Color c = image.GetPixel(x, y);

                    Int32 gs = (Int32)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);

                    grayScale.SetPixel(x, y, Color.FromArgb(gs, gs, gs));
                }
            return grayScale;
        }



        ////////////////
        //
        // Local Records Page
        //
        ////////////////

        bool localLock1 = false;
        public void ShowLocalRecordsPage()
        {
            mainPageBTN.BackColor = Color.MediumSeaGreen; mainPageBTN.ForeColor = Color.White;
            newRecordBTN.BackColor = Color.MediumSeaGreen; newRecordBTN.ForeColor = Color.White;
            localRecordsBTN.BackColor = Color.White; localRecordsBTN.ForeColor = Color.MediumSeaGreen;
            allRecordsBTN.BackColor = Color.MediumSeaGreen; allRecordsBTN.ForeColor = Color.White;
            statisticsBTN.BackColor = Color.MediumSeaGreen; statisticsBTN.ForeColor = Color.White;

            mainPagePANEL.Visible = false;
            newRecordPANEL.Visible = false;
            localRecordsPANEL.Visible = true;
            allRecordsPANEL.Visible = false;
            statisticPANEL.Visible = false;
            localPageBOX.Text = "1";
        }
        public void GetLocalData(int sortIndex)
        {
            personList = mainDB.GetLocalData(selectedMuseumId.ToString(),localSearchBOX.Text, searchNo, sortIndex.ToString());
            WriteLocalData(selectedPageNO);
        }
        public void WriteLocalData(int index)
        {
            int pageCount = 1;

            if (personList.Count % 20 == 0) { pageCount = personList.Count / 20; }
            else { pageCount = personList.Count / 20 + 1; }

            localPageBOX.Items.Clear();
            for (int i = 1; i <= pageCount; i++) { localPageBOX.Items.Add(i.ToString()); }

            localFlowPanel.Controls.Clear();

            int startIndex = ((index - 1) * 20);
            int endIndex = (index * 20); if (endIndex > personList.Count) { endIndex = personList.Count; }
            for (int i = startIndex; i < endIndex; i++)
            {
                Person person = personList[i];
                PersonCardItem personCardItem = new PersonCardItem(person);
                localFlowPanel.Controls.Add(personCardItem);
            }
        }
        private void localRecordsBTN_Click(object sender, EventArgs e)
        {
            ShowLocalRecordsPage();

            localLock1 = true; localSortingBOX.SelectedIndex = 1;
            localLock1 = false; localSortingBOX.SelectedIndex = 0;
        }
        private void localPageBOX_SelectedIndexChanged(object sender, EventArgs e) { selectedPageNO = int.Parse(localPageBOX.Text); WriteLocalData(int.Parse(localPageBOX.Text)); }

        private void localSortingBOX_SelectedIndexChanged(object sender, EventArgs e) { if(!localLock1){ GetLocalData(localSortingBOX.SelectedIndex); } }
        private void localSearchBTN_Click(object sender, EventArgs e)
        {
            selectedPageNO = 1;
            GetLocalData(localSortingBOX.SelectedIndex);
        }
        private void byNameCHECK_Click(object sender, EventArgs e) { searchNo = "0"; localByNameCHECK.Checked = true; localByCountryCHECK.Checked = false; localByCardTypeCHECK.Checked = false; }

        private void byCountryCHECK_Click(object sender, EventArgs e) { searchNo = "1"; localByNameCHECK.Checked = false; localByCountryCHECK.Checked = true; localByCardTypeCHECK.Checked = false; }

        private void byCardTypeCHECK_Click(object sender, EventArgs e) { searchNo = "2"; localByNameCHECK.Checked = false; localByCountryCHECK.Checked = false; localByCardTypeCHECK.Checked = true; }


        ////////////////
        //
        // All Records Page
        //
        ////////////////
        
        bool allLock1 = false;
        public void GetAllData(int sortIndex)
        {
            personList = mainDB.GetAllData(allSearchBOX.Text, searchNo, sortIndex.ToString());
            WriteAllData(selectedPageNO);
        }
        public void WriteAllData(int index)
        {
            int pageCount = 1;

            if (personList.Count % 20 == 0) { pageCount = personList.Count / 20; }
            else { pageCount = personList.Count / 20 + 1; }

            allPageBOX.Items.Clear();
            for (int i = 1; i <= pageCount; i++) { allPageBOX.Items.Add(i.ToString()); }

            allFlowPanel.Controls.Clear();

            int startIndex = ((index - 1) * 20);
            int endIndex = (index * 20); if (endIndex > personList.Count) { endIndex = personList.Count; }
            for (int i = startIndex; i < endIndex; i++)
            {
                Person person = personList[i];
                PersonCardItem personCardItem = new PersonCardItem(person);
                allFlowPanel.Controls.Add(personCardItem);
            }
        }
        public void ShowAllRecordsPage()
        {
            mainPageBTN.BackColor = Color.MediumSeaGreen; mainPageBTN.ForeColor = Color.White;
            newRecordBTN.BackColor = Color.MediumSeaGreen; newRecordBTN.ForeColor = Color.White;
            localRecordsBTN.BackColor = Color.MediumSeaGreen; localRecordsBTN.ForeColor = Color.White;
            allRecordsBTN.BackColor = Color.White; allRecordsBTN.ForeColor = Color.MediumSeaGreen;
            statisticsBTN.BackColor = Color.MediumSeaGreen; statisticsBTN.ForeColor = Color.White;

            mainPagePANEL.Visible = false;
            newRecordPANEL.Visible = false;
            localRecordsPANEL.Visible = false;
            allRecordsPANEL.Visible = true;
            statisticPANEL.Visible = false;

            allPageBOX.Text = "1";
        }
        private void allRecordsBTN_Click(object sender, EventArgs e)
        {
            ShowAllRecordsPage();

            allLock1 = true; allSortingBOX.SelectedIndex = 1;
            allLock1 = false; allSortingBOX.SelectedIndex = 0;
        }
        private void allPageBOX_SelectedIndexChanged(object sender, EventArgs e) { selectedPageNO = int.Parse(allPageBOX.Text); WriteAllData(int.Parse(allPageBOX.Text)); }
        private void allSortingBOX_SelectedIndexChanged(object sender, EventArgs e) { if (!allLock1) { selectedPageNO = 1; GetAllData(allSortingBOX.SelectedIndex); } }
        private void allSearchBTN_Click(object sender, EventArgs e)
        {
            selectedPageNO = 1;
            GetAllData(allSortingBOX.SelectedIndex);
        }
        private void allByNameCHECK_Click(object sender, EventArgs e) { searchNo = "0"; allByNameCHECK.Checked = true; allByCountryCHECK.Checked = false; allByCardTypeCHECK.Checked = false; }
        private void allByCountryCHECK_Click(object sender, EventArgs e) { searchNo = "1"; allByNameCHECK.Checked = false; allByCountryCHECK.Checked = true; allByCardTypeCHECK.Checked = false; }
        private void allByCardTypeCHECK_Click(object sender, EventArgs e) { searchNo = "2"; allByNameCHECK.Checked = false; allByCountryCHECK.Checked = false; allByCardTypeCHECK.Checked = true; }


        ////////////////
        //
        // Statistics Page
        //
        ////////////////
        public void ShowStatisticPage()
        {
            mainPageBTN.BackColor = Color.MediumSeaGreen; mainPageBTN.ForeColor = Color.White;
            newRecordBTN.BackColor = Color.MediumSeaGreen; newRecordBTN.ForeColor = Color.White;
            localRecordsBTN.BackColor = Color.MediumSeaGreen; localRecordsBTN.ForeColor = Color.White;
            allRecordsBTN.BackColor = Color.MediumSeaGreen; allRecordsBTN.ForeColor = Color.White;
            statisticsBTN.BackColor = Color.White; statisticsBTN.ForeColor = Color.MediumSeaGreen;

            mainPagePANEL.Visible = false;
            newRecordPANEL.Visible = false;
            localRecordsPANEL.Visible = false;
            allRecordsPANEL.Visible = false;
            statisticPANEL.Visible = true;


            StatisticsDB sqlObjesi = new StatisticsDB();
            //tablo

            sqlObjesi.CreateBasicTable();

            //indexler

            sqlObjesi.IndexOlusturucu();

            //Viewlar

            sqlObjesi.UyeDagilimiView();
            sqlObjesi.TuristUlkeDagilimView();
            sqlObjesi.UyeYasDagilimiView();
            sqlObjesi.VatandasilDagilimiView();
            sqlObjesi.MuzeUyeSayilariView();
            sqlObjesi.BirHaftalikKayitView();
            sqlObjesi.VatandasUyeCinsiyetDagilimiView();
            sqlObjesi.TuristUyeCinsiyetDagilimiView();

            //inline ve multi statementlar

            sqlObjesi.Inlinestmnt_anabilgiweekcrd();
            sqlObjesi.Inlinestmnt_anabilgimonthcrd();
            sqlObjesi.Inlinestmnt_anabilgiyearcrd();
            sqlObjesi.Multistmnt_kartbilgiweekcrd();
            sqlObjesi.Multistmnt_kartbilgimonthcrd();
            sqlObjesi.Multistmnt_kartbilgiyearcrd();

            // genel üye grafiği oluşturma

            foreach (var series in chrtGenelUye.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtYabancıUyeUlke.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtYasDagilimi.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtVatandasUyeilDagilimi.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtMuzeUyeSayisi.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtHaftaKayitYogunluk.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtVatandasCinsiyetDagilim.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chrtTuristCinsiyetDagilim.Series)
            {
                series.Points.Clear();
            }


            dataGridView1.DataSource = sqlObjesi.GenelUyeGrafigi();

            chrtGenelUye.ChartAreas["ChartArea1"].AxisX.Title = "Kart Tipi";
            chrtGenelUye.ChartAreas["ChartArea1"].AxisY.Title = "Üye Sayısı";

            chrtGenelUye.Series["turist"].Points.AddXY(dataGridView1.Rows[0].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[0].Cells["totalMember"].Value));
            chrtGenelUye.Series["turist"].Points.AddXY(dataGridView1.Rows[2].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[2].Cells["totalMember"].Value));
            chrtGenelUye.Series["turist"].Points.AddXY(dataGridView1.Rows[4].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[4].Cells["totalMember"].Value));

            chrtGenelUye.Series["vatandaş"].Points.AddXY(dataGridView1.Rows[1].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[1].Cells["totalMember"].Value));
            chrtGenelUye.Series["vatandaş"].Points.AddXY(dataGridView1.Rows[3].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[3].Cells["totalMember"].Value));
            chrtGenelUye.Series["vatandaş"].Points.AddXY(dataGridView1.Rows[5].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView1.Rows[5].Cells["totalMember"].Value));



            //yabancı üye ülke dağılımı oluşturma
            chrtYabancıUyeUlke.DataSource = sqlObjesi.YabancıUyeUlkeDağılımı();

            chrtYabancıUyeUlke.ChartAreas["ChartArea1"].AxisX.Title = "Ülkeler";
            chrtYabancıUyeUlke.ChartAreas["ChartArea1"].AxisY.Title = "Üye Sayısı";

            chrtYabancıUyeUlke.Series["Series1"].XValueMember = "country";
            chrtYabancıUyeUlke.Series["Series1"].YValueMembers = "totalMember";


            //üye yaş dağılımı oluşturma
            dataGridView3.DataSource = sqlObjesi.UyeYasDagilimi();
            chrtVatandasUyeilDagilimi.DataSource = dataGridView3.DataSource;

            chrtYasDagilimi.ChartAreas["ChartArea1"].AxisX.Title = "Yaş";
            chrtYasDagilimi.ChartAreas["ChartArea1"].AxisY.Title = "Üye sayısı";

            for (int i = 0; i < 152; i++)
            {
                if (i < 76)
                {
                    chrtYasDagilimi.Series["Turist"].Points.AddXY(Convert.ToInt32(dataGridView3.Rows[i].Cells["age"].Value), Convert.ToInt32(dataGridView3.Rows[i].Cells["totalMember"].Value));
                }
                else
                {
                    chrtYasDagilimi.Series["Vatandaş"].Points.AddXY(Convert.ToInt32(dataGridView3.Rows[i].Cells["age"].Value), Convert.ToInt32(dataGridView3.Rows[i].Cells["totalMember"].Value));

                }
            }

            // vatandaş üye il dağılımı oluşturma
            chrtVatandasUyeilDagilimi.DataSource = sqlObjesi.VatandasUyeIlDagilimi();

            chrtVatandasUyeilDagilimi.ChartAreas["ChartArea1"].AxisX.Title = "Şehir";
            chrtVatandasUyeilDagilimi.ChartAreas["ChartArea1"].AxisY.Title = "Üye Sayısı";

            chrtVatandasUyeilDagilimi.Series["Üye Sayisi"].XValueMember = "city";
            chrtVatandasUyeilDagilimi.Series["Üye Sayisi"].YValueMembers = "totalMember ";

            //chrtGenelUye.Series["Üye Sayisi"].Points[0].


            //muze üye sayiları dağılımı oluşturma
            chrtMuzeUyeSayisi.DataSource = sqlObjesi.MuzeUyeSayilari();

            chrtMuzeUyeSayisi.ChartAreas["ChartArea1"].AxisX.Title = "Müzeler";
            chrtMuzeUyeSayisi.ChartAreas["ChartArea1"].AxisY.Title = "Üye sayısı";

            chrtMuzeUyeSayisi.Series["Series1"].XValueMember = "museumName";
            chrtMuzeUyeSayisi.Series["Series1"].YValueMembers = "totalMember";

            // bir haftalık kayıt yoğunluğu
            dataGridView6.DataSource = sqlObjesi.HaftaKayitYogunlugu();
            chrtHaftaKayitYogunluk.DataSource = dataGridView6.DataSource;

            chrtHaftaKayitYogunluk.ChartAreas["ChartArea1"].AxisX.Title = "Tarih";
            chrtHaftaKayitYogunluk.ChartAreas["ChartArea1"].AxisY.Title = "Yoğunluk";

            for (int i = 0; i < 14; i++)
            {
                if (i % 2 == 0)
                {
                    chrtHaftaKayitYogunluk.Series["Turist"].Points.AddXY(dataGridView6.Rows[i].Cells["cardStartDate"].Value.ToString().Substring(0, 10), Convert.ToInt32(dataGridView6.Rows[i].Cells["totalMember"].Value));
                }
                else
                {
                    chrtHaftaKayitYogunluk.Series["Vatandaş"].Points.AddXY((dataGridView6.Rows[i].Cells["cardStartDate"].Value.ToString().Substring(0, 10)), Convert.ToInt32(dataGridView6.Rows[i].Cells["totalMember"].Value));

                }
            }

            //vatandaş üye cinsiyet dağılımı

            dataGridView7.DataSource = sqlObjesi.VatandasUyeCinsiyetDagilimi();

            chrtVatandasCinsiyetDagilim.ChartAreas["ChartArea1"].AxisX.Title = "Kart Tipi";
            chrtVatandasCinsiyetDagilim.ChartAreas["ChartArea1"].AxisY.Title = "Üye Sayısı";

            chrtVatandasCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView7.Rows[0].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[0].Cells["totalMember"].Value));
            chrtVatandasCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView7.Rows[2].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[2].Cells["totalMember"].Value));
            chrtVatandasCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView7.Rows[4].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[4].Cells["totalMember"].Value));

            chrtVatandasCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView7.Rows[1].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[1].Cells["totalMember"].Value));
            chrtVatandasCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView7.Rows[3].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[3].Cells["totalMember"].Value));
            chrtVatandasCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView7.Rows[5].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView7.Rows[5].Cells["totalMember"].Value));

            //turist uye cinsiyet dağılımı

            dataGridView8.DataSource = sqlObjesi.TuristUyeCinsiyetDagilimi();

            chrtTuristCinsiyetDagilim.ChartAreas["ChartArea1"].AxisX.Title = "Kart Tipi";
            chrtTuristCinsiyetDagilim.ChartAreas["ChartArea1"].AxisY.Title = "Üye Sayısı";

            chrtTuristCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView8.Rows[0].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[0].Cells["totalMember"].Value));
            chrtTuristCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView8.Rows[2].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[2].Cells["totalMember"].Value));
            chrtTuristCinsiyetDagilim.Series["Bayan"].Points.AddXY(dataGridView8.Rows[4].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[4].Cells["totalMember"].Value));

            chrtTuristCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView8.Rows[1].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[1].Cells["totalMember"].Value));
            chrtTuristCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView8.Rows[3].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[3].Cells["totalMember"].Value));
            chrtTuristCinsiyetDagilim.Series["Bay"].Points.AddXY(dataGridView8.Rows[5].Cells["typeName"].Value.ToString(), Convert.ToInt32(dataGridView8.Rows[5].Cells["totalMember"].Value));




        }
        private void statisticsBTN_Click(object sender, EventArgs e)
        {
            ShowStatisticPage();
        }


       
    }
}
