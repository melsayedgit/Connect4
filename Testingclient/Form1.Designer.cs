
namespace client
{
    partial class Form1
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
            this.connect = new System.Windows.Forms.Button();
            this.sendRequest = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.nametextbox = new System.Windows.Forms.TextBox();
            this.receiveBtn = new System.Windows.Forms.Button();
            this.requestsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(127, 71);
            this.connect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(100, 28);
            this.connect.TabIndex = 0;
            this.connect.Text = "connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // sendRequest
            // 
            this.sendRequest.Location = new System.Drawing.Point(127, 159);
            this.sendRequest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sendRequest.Name = "sendRequest";
            this.sendRequest.Size = new System.Drawing.Size(100, 28);
            this.sendRequest.TabIndex = 1;
            this.sendRequest.Text = "send request";
            this.sendRequest.UseVisualStyleBackColor = true;
            this.sendRequest.Click += new System.EventHandler(this.sendRequest_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 127);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(132, 22);
            this.textBox1.TabIndex = 2;
            // 
            // nametextbox
            // 
            this.nametextbox.Location = new System.Drawing.Point(112, 39);
            this.nametextbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nametextbox.Name = "nametextbox";
            this.nametextbox.Size = new System.Drawing.Size(132, 22);
            this.nametextbox.TabIndex = 3;
            // 
            // receiveBtn
            // 
            this.receiveBtn.Location = new System.Drawing.Point(348, 235);
            this.receiveBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.receiveBtn.Name = "receiveBtn";
            this.receiveBtn.Size = new System.Drawing.Size(100, 28);
            this.receiveBtn.TabIndex = 4;
            this.receiveBtn.Text = "receive";
            this.receiveBtn.UseVisualStyleBackColor = true;
            this.receiveBtn.Click += new System.EventHandler(this.receiveBtn_Click);
            // 
            // requestsList
            // 
            this.requestsList.FormattingEnabled = true;
            this.requestsList.ItemHeight = 16;
            this.requestsList.Location = new System.Drawing.Point(316, 34);
            this.requestsList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.requestsList.Name = "requestsList";
            this.requestsList.Size = new System.Drawing.Size(159, 180);
            this.requestsList.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 282);
            this.Controls.Add(this.requestsList);
            this.Controls.Add(this.receiveBtn);
            this.Controls.Add(this.nametextbox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.sendRequest);
            this.Controls.Add(this.connect);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "TestingClient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button sendRequest;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox nametextbox;
        private System.Windows.Forms.Button receiveBtn;
        private System.Windows.Forms.ListBox requestsList;
    }
}

