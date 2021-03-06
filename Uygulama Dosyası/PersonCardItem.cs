using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeriTabaniProje
{
    public partial class PersonCardItem : UserControl
    {
        private Person person;
        public PersonCardItem(Person person)
        {
            this.person = person;
            InitializeComponent();
        }

        private void UserCardItem_Load(object sender, EventArgs e)
        {
            SetBasics();
        }

        public void SetBasics()
        {
            nameAndSurnameLBL.Text = person.firstName + " " + person.lastName;
            ageLBL.Text = person.age;
            if (person.cardType == "1") { cardTypeLBL.Text = "Haftalık"; } else if (person.cardType == "2") { cardTypeLBL.Text = "Aylık"; } else { cardTypeLBL.Text = "Yıllık"; }
            if (person.isCardActive == "True") { cardActiveLBL.Text = "Aktif"; } else { cardActiveLBL.Text = "Aktif Değil"; }
            

            string genderImagePath = "";
            if(person.gender == "Male") { genderImagePath = Application.StartupPath + "\\Pictures\\male1.png"; }
            else { genderImagePath = Application.StartupPath + "\\Pictures\\female1.png"; }
            Image img1 = Image.FromFile(genderImagePath);
            genderPIC.Image = img1;
        }

        private void detailBTN_Click(object sender, EventArgs e)
        {
            EditForm editForm = new EditForm(person.id,person.cardType);
            editForm.ShowDialog();
        }
    }
}
