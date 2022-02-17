
namespace serverAppConnect4
{
    partial class Server
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            this.startServerBtn = new System.Windows.Forms.Button();
            this.playerList = new System.Windows.Forms.ListBox();
            this.roomList = new System.Windows.Forms.ListBox();
            this.fillList = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // startServerBtn
            // 
            this.startServerBtn.Location = new System.Drawing.Point(215, 379);
            this.startServerBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.startServerBtn.Name = "startServerBtn";
            this.startServerBtn.Size = new System.Drawing.Size(163, 28);
            this.startServerBtn.TabIndex = 1;
            this.startServerBtn.Text = "Start Server";
            this.startServerBtn.UseVisualStyleBackColor = true;
            this.startServerBtn.Click += new System.EventHandler(this.startServerBtn_Click);
            // 
            // playerList
            // 
            this.playerList.FormattingEnabled = true;
            this.playerList.ItemHeight = 16;
            this.playerList.Location = new System.Drawing.Point(320, 31);
            this.playerList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.playerList.Name = "playerList";
            this.playerList.Size = new System.Drawing.Size(256, 340);
            this.playerList.TabIndex = 2;
            // 
            // roomList
            // 
            this.roomList.FormattingEnabled = true;
            this.roomList.ItemHeight = 16;
            this.roomList.Location = new System.Drawing.Point(16, 31);
            this.roomList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.roomList.Name = "roomList";
            this.roomList.Size = new System.Drawing.Size(256, 340);
            this.roomList.TabIndex = 3;
            // 
            // fillList
            // 
            this.fillList.Location = new System.Drawing.Point(433, 377);
            this.fillList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fillList.Name = "fillList";
            this.fillList.Size = new System.Drawing.Size(97, 28);
            this.fillList.TabIndex = 4;
            this.fillList.Text = "fill list";
            this.fillList.UseVisualStyleBackColor = true;
            this.fillList.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 420);
            this.Controls.Add(this.fillList);
            this.Controls.Add(this.roomList);
            this.Controls.Add(this.playerList);
            this.Controls.Add(this.startServerBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Server";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "server  ";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button startServerBtn;
        private System.Windows.Forms.ListBox playerList;
        private System.Windows.Forms.ListBox roomList;
        private System.Windows.Forms.Button fillList;
        private System.Windows.Forms.Timer timer1;
    }
}

