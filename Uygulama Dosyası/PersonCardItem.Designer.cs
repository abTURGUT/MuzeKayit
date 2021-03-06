namespace VeriTabaniProje
{
    partial class PersonCardItem
    {
        /// <summary> 
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Bileşen Tasarımcısı üretimi kod

        /// <summary> 
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonCardItem));
            this.genderPIC = new System.Windows.Forms.PictureBox();
            this.detailBTN = new System.Windows.Forms.Button();
            this.nameAndSurnameLBL = new System.Windows.Forms.Label();
            this.ageLBL = new System.Windows.Forms.Label();
            this.cardTypeLBL = new System.Windows.Forms.Label();
            this.cardActiveLBL = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.genderPIC)).BeginInit();
            this.SuspendLayout();
            // 
            // genderPIC
            // 
            this.genderPIC.BackColor = System.Drawing.Color.White;
            this.genderPIC.Image = ((System.Drawing.Image)(resources.GetObject("genderPIC.Image")));
            this.genderPIC.Location = new System.Drawing.Point(14, 12);
            this.genderPIC.Name = "genderPIC";
            this.genderPIC.Size = new System.Drawing.Size(80, 74);
            this.genderPIC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.genderPIC.TabIndex = 0;
            this.genderPIC.TabStop = false;
            // 
            // detailBTN
            // 
            this.detailBTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.detailBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.detailBTN.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailBTN.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.detailBTN.Location = new System.Drawing.Point(515, 12);
            this.detailBTN.Name = "detailBTN";
            this.detailBTN.Size = new System.Drawing.Size(96, 74);
            this.detailBTN.TabIndex = 74;
            this.detailBTN.Text = "DETAY";
            this.detailBTN.UseVisualStyleBackColor = true;
            this.detailBTN.Click += new System.EventHandler(this.detailBTN_Click);
            // 
            // nameAndSurnameLBL
            // 
            this.nameAndSurnameLBL.AutoSize = true;
            this.nameAndSurnameLBL.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameAndSurnameLBL.Location = new System.Drawing.Point(201, 12);
            this.nameAndSurnameLBL.Name = "nameAndSurnameLBL";
            this.nameAndSurnameLBL.Size = new System.Drawing.Size(71, 16);
            this.nameAndSurnameLBL.TabIndex = 75;
            this.nameAndSurnameLBL.Text = "AD-SOYAD";
            // 
            // ageLBL
            // 
            this.ageLBL.AutoSize = true;
            this.ageLBL.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ageLBL.Location = new System.Drawing.Point(160, 31);
            this.ageLBL.Name = "ageLBL";
            this.ageLBL.Size = new System.Drawing.Size(32, 16);
            this.ageLBL.TabIndex = 76;
            this.ageLBL.Text = "YAŞ";
            // 
            // cardTypeLBL
            // 
            this.cardTypeLBL.AutoSize = true;
            this.cardTypeLBL.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardTypeLBL.Location = new System.Drawing.Point(194, 50);
            this.cardTypeLBL.Name = "cardTypeLBL";
            this.cardTypeLBL.Size = new System.Drawing.Size(63, 16);
            this.cardTypeLBL.TabIndex = 78;
            this.cardTypeLBL.Text = "KART TİPİ";
            // 
            // cardActiveLBL
            // 
            this.cardActiveLBL.AutoSize = true;
            this.cardActiveLBL.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cardActiveLBL.Location = new System.Drawing.Point(232, 69);
            this.cardActiveLBL.Name = "cardActiveLBL";
            this.cardActiveLBL.Size = new System.Drawing.Size(101, 16);
            this.cardActiveLBL.TabIndex = 79;
            this.cardActiveLBL.Text = "KART AKTİFLİĞİ ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(111, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 16);
            this.label1.TabIndex = 84;
            this.label1.Text = "KART AKTİFLİĞİ :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(111, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 83;
            this.label2.Text = "KART TİPİ :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(111, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 16);
            this.label3.TabIndex = 82;
            this.label3.Text = "YAŞ :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(111, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 16);
            this.label4.TabIndex = 81;
            this.label4.Text = "AD-SOYAD :";
            // 
            // PersonCardItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cardActiveLBL);
            this.Controls.Add(this.cardTypeLBL);
            this.Controls.Add(this.ageLBL);
            this.Controls.Add(this.nameAndSurnameLBL);
            this.Controls.Add(this.detailBTN);
            this.Controls.Add(this.genderPIC);
            this.Name = "PersonCardItem";
            this.Size = new System.Drawing.Size(628, 100);
            this.Load += new System.EventHandler(this.UserCardItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.genderPIC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox genderPIC;
        private System.Windows.Forms.Button detailBTN;
        private System.Windows.Forms.Label nameAndSurnameLBL;
        private System.Windows.Forms.Label ageLBL;
        private System.Windows.Forms.Label cardTypeLBL;
        private System.Windows.Forms.Label cardActiveLBL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
