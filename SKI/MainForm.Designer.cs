namespace SKI
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.модульToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.математическаяМодельToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.определениеНештатныхСитуацийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.модульToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // модульToolStripMenuItem
            // 
            this.модульToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.математическаяМодельToolStripMenuItem,
            this.определениеНештатныхСитуацийToolStripMenuItem});
            this.модульToolStripMenuItem.Name = "модульToolStripMenuItem";
            this.модульToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.модульToolStripMenuItem.Text = "Модуль";
            // 
            // математическаяМодельToolStripMenuItem
            // 
            this.математическаяМодельToolStripMenuItem.Name = "математическаяМодельToolStripMenuItem";
            this.математическаяМодельToolStripMenuItem.Size = new System.Drawing.Size(335, 26);
            this.математическаяМодельToolStripMenuItem.Text = "Математическая модель";
            this.математическаяМодельToolStripMenuItem.Click += new System.EventHandler(this.математическаяМодельToolStripMenuItem_Click);
            // 
            // определениеНештатныхСитуацийToolStripMenuItem
            // 
            this.определениеНештатныхСитуацийToolStripMenuItem.Name = "определениеНештатныхСитуацийToolStripMenuItem";
            this.определениеНештатныхСитуацийToolStripMenuItem.Size = new System.Drawing.Size(335, 26);
            this.определениеНештатныхСитуацийToolStripMenuItem.Text = "Определение нештатных ситуаций";
            this.определениеНештатныхСитуацийToolStripMenuItem.Click += new System.EventHandler(this.определениеНештатныхСитуацийToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SKI";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem модульToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem математическаяМодельToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem определениеНештатныхСитуацийToolStripMenuItem;
    }
}

